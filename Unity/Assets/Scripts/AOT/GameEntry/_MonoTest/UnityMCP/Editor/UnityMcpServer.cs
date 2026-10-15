/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： Unity 侧 MCP 服务器（编排层）

* 职责只有三件，其余都分出去了：
*   1. 生命周期：启动/停止监听（域重载会杀掉监听，这里负责重启），绑日志缓冲
*   2. 主线程编组：后台线程收到的请求排进队列，由 EditorApplication.update 在主线程消费
*   3. MCP 协议分发：initialize / ping / tools/list / tools/call

* 分层：
*   UnityMcpHttpTransport  传输（裸 TcpListener + 最小 HTTP，不懂 MCP）
*   UnityMcpServer         本文件（生命周期 + 编组 + 分发）
*   UnityMcpToolRegistry   工具注册表
*   UnityMcpTool / Tools/* 各工具（名字 / 描述 / schema / 执行）
*   CSharpExecutor         内存编译执行（unity_exec 用）
*   UnityMcpLogBuffer      日志环形缓冲（unity_logs 用）
*   UnityMcpPaths          运行时路径集中定义

* 关于"为什么不用 SSE / 不支持服务器推送"：GET 一律回 405，
* 客户端（OMP）会据此静默禁用后台监听，我们不需要实现服务端推送。

****************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace UnityMCP.Editor
{
	/// <summary>
	/// Unity 侧 MCP 服务器
	/// </summary>
	[InitializeOnLoad]
	public static class UnityMcpServer
	{
		/// <summary>
		/// 协议版本（initialize 时若客户端没报版本就用这个）
		/// </summary>
		public const string ProtocolVersion = "2025-11-25";

		/// <summary>
		/// 服务器版本
		/// </summary>
		public const string ServerVersion = "0.2.0";

		/// <summary>
		/// 单个请求等待主线程执行的上限（秒）
		/// </summary>
		public const int ExecTimeoutSec = 120;

		private static readonly ConcurrentQueue<Job> pending = new ConcurrentQueue<Job>();

		/// <summary>
		/// 一次请求的工作项：后台线程攒好，投到主线程执行，结果用 Done 回传
		/// </summary>
		private class Job
		{
			public JObject Request;
			public bool IsNotification;
			public Action Post;              // 响应写回之后才在主线程执行
			public string ResponseJson;
			public readonly ManualResetEventSlim Done = new ManualResetEventSlim(false);
		}

		static UnityMcpServer()
		{
			UnityMcpLogBuffer.Install();
			AssemblyReloadEvents.beforeAssemblyReload += Stop;
			EditorApplication.update += Drain;
			EditorApplication.delayCall += Start;
		}

		#region 生命周期

		/// <summary>
		/// 启动监听
		/// </summary>
		public static void Start()
		{
			if (UnityMcpHttpTransport.IsRunning) return;

			if (UnityMcpHttpTransport.Start(HandleHttp))
			{
				Debug.Log($"[UnityMcp] 服务器已启动: http://127.0.0.1:{UnityMcpHttpTransport.Port}{UnityMcpHttpTransport.Endpoint}  "
					+ (File.Exists(UnityMcpPaths.TokenFile) ? "（已启用 token 鉴权）" : "（无鉴权，仅本机回环）"));
			}
			else
			{
				Debug.LogError($"[UnityMcp] 端口 {UnityMcpHttpTransport.Port} 绑定失败，MCP 服务器未启动");
			}
		}

		/// <summary>
		/// 停止监听（域重载前 / 退出时）
		/// </summary>
		public static void Stop()
		{
			if (!UnityMcpHttpTransport.IsRunning) return;
			UnityMcpHttpTransport.Stop();
			Debug.Log("[UnityMcp] 服务器已停止（域重载或退出）");
		}

		#endregion

		#region 主线程编组

		/// <summary>
		/// 主线程消费队列：执行请求，或跑"响应后动作"
		/// </summary>
		private static void Drain()
		{
			Job job;
			while (pending.TryDequeue(out job))
			{
				try
				{
					if (job.Request != null) job.ResponseJson = Dispatch(job);
					else if (job.Post != null) job.Post();
				}
				catch (Exception e)
				{
					job.ResponseJson = Error(job.Request != null ? job.Request["id"] : null, -32603, "内部错误: " + e);
				}
				finally
				{
					if (!job.IsNotification) job.Done.Set();
				}
			}
		}

		#endregion

		#region MCP 协议

		/// <summary>
		/// HTTP 请求 → 等主线程执行 → 响应
		/// </summary>
		private static UnityMcpHttpTransport.Response HandleHttp(UnityMcpHttpTransport.Request req)
		{
			JObject json;
			try { json = JObject.Parse(req.Body); }
			catch (Exception e)
			{
				return new UnityMcpHttpTransport.Response { Json = Error(null, -32700, "JSON 解析失败: " + e.Message) };
			}

			JToken id = json["id"];
			bool isNotification = id == null || id.Type == JTokenType.Null;

			Job job = new Job { Request = json, IsNotification = isNotification };
			pending.Enqueue(job);

			if (isNotification)
			{
				// 通知不需要结果，立刻 202（客户端接受任意 2xx）
				return new UnityMcpHttpTransport.Response { Status = 202, Reason = "Accepted" };
			}

			if (!job.Done.Wait(TimeSpan.FromSeconds(ExecTimeoutSec)))
			{
				return new UnityMcpHttpTransport.Response
				{
					Status = 504,
					Reason = "Gateway Timeout",
					Json = Error(id, -32000,
						$"Unity 编辑器在 {ExecTimeoutSec}s 内没有执行该请求（可能正在编译、域重载，或被模态框阻塞）"),
				};
			}

			// 响应已经写回，这时才把"响应后动作"排进主线程（例如触发域重载的 Refresh）
			if (job.Post != null)
			{
				Action post = job.Post;
				pending.Enqueue(new Job { Post = post });
			}

			return new UnityMcpHttpTransport.Response { Json = job.ResponseJson };
		}

		/// <summary>
		/// JSON-RPC 方法分发（主线程）
		/// </summary>
		private static string Dispatch(Job job)
		{
			JObject req = job.Request;
			string method = (string)req["method"];
			JToken id = req["id"];
			JObject p = req["params"] as JObject;

			if (method == "initialize")
			{
				// 规范建议：客户端请求的版本我们支持就回同一个
				string v = p != null ? (string)p["protocolVersion"] : null;
				return Result(id, new JObject
				{
					["protocolVersion"] = string.IsNullOrEmpty(v) ? ProtocolVersion : v,
					["capabilities"] = new JObject { ["tools"] = new JObject() },
					["serverInfo"] = new JObject { ["name"] = "unity-mcp", ["version"] = ServerVersion },
				});
			}

			if (method == "ping") return Result(id, new JObject());

			if (method == "tools/list")
			{
				return Result(id, new JObject { ["tools"] = UnityMcpToolRegistry.Descriptors() });
			}

			if (method == "tools/call")
			{
				string name = p != null ? (string)p["name"] : null;
				JObject args = p != null ? p["arguments"] as JObject : null;

				UnityMcpTool tool = UnityMcpToolRegistry.Find(name);
				if (tool == null) return ToolResult(id, "未知工具: " + name, true);

				UnityMcpToolResult r = tool.Invoke(args);
				job.Post = r.Post;
				return ToolResult(id, r.Text, r.IsError);
			}

			// 通知类方法没有 id，本来就不该有响应
			if (id == null || id.Type == JTokenType.Null) return null;
			return Error(id, -32601, "未实现的方法: " + method);
		}

		private static string Result(JToken id, JToken result)
		{
			return new JObject
			{
				["jsonrpc"] = "2.0",
				["id"] = id != null ? id : JValue.CreateNull(),
				["result"] = result,
			}.ToString(Newtonsoft.Json.Formatting.None);
		}

		private static string Error(JToken id, int code, string message)
		{
			return new JObject
			{
				["jsonrpc"] = "2.0",
				["id"] = id != null ? id : JValue.CreateNull(),
				["error"] = new JObject { ["code"] = code, ["message"] = message },
			}.ToString(Newtonsoft.Json.Formatting.None);
		}

		private static string ToolResult(JToken id, string text, bool isError)
		{
			return Result(id, new JObject
			{
				["content"] = new JArray { new JObject { ["type"] = "text", ["text"] = text } },
				["isError"] = isError,
			});
		}

		#endregion
	}
}
