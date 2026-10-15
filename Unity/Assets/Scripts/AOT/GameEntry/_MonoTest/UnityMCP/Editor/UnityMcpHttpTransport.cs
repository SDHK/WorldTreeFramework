/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： MCP 的 HTTP 传输层（裸 TcpListener + 手写最小 HTTP/1.1）

* 这一层**完全不知道 JSON-RPC / MCP** —— 它只做：
*   收字节 → 解析请求行与请求头 → 可选鉴权 → 把 body 交给上层 Handler → 把响应写回去
* 上层给什么 JSON 就发什么 JSON。

* 为什么不用 HttpListener：它走 Windows http.sys，非管理员可能需要
* `netsh http add urlacl`；裸 TcpListener 没有这个门槛。

* 连接在**后台线程**处理（不依赖编辑器主循环），所以"编辑器忙/在编译"
* 时也能立刻回一个明确的错误，而不是让调用方干等。

****************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UnityMCP.Editor
{
	/// <summary>
	/// MCP 的 HTTP 传输层
	/// </summary>
	public static class UnityMcpHttpTransport
	{
		/// <summary>
		/// 监听端口
		/// </summary>
		public const int Port = 8099;

		/// <summary>
		/// 端点路径（其余路径也接受）
		/// </summary>
		public const string Endpoint = "/mcp";

		/// <summary>
		/// 一个已解析的 HTTP 请求
		/// </summary>
		public class Request
		{
			public string Method;
			public string Path;
			public string Body;
			public string Authorization;
		}

		/// <summary>
		/// 要写回的 HTTP 响应
		/// </summary>
		public class Response
		{
			public int Status = 200;
			public string Reason = "OK";
			public string Json;
		}

		/// <summary>
		/// 上层处理器：拿到请求，返回响应
		/// </summary>
		public delegate Response Handler(Request req);

		private static TcpListener listener;
		private static Thread acceptThread;
		private static Handler handler;
		private static volatile bool running;

		/// <summary>
		/// 是否在监听
		/// </summary>
		public static bool IsRunning { get { return running; } }

		/// <summary>
		/// 绑定端口并开始接收；失败返回 false（内部重试 3 次）
		/// </summary>
		public static bool Start(Handler h)
		{
			if (running) return true;
			handler = h;

			for (int attempt = 1; attempt <= 3 && !running; attempt++)
			{
				try
				{
					TcpListener l = new TcpListener(IPAddress.Loopback, Port);
					l.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
					l.Start();
					listener = l;
					running = true;
					acceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = "UnityMcpHttp" };
					acceptThread.Start();
				}
				catch (Exception e)
				{
					UnityEngine.Debug.LogWarning($"[UnityMcp] 第 {attempt} 次绑定端口 {Port} 失败: {e.Message}");
					Thread.Sleep(500);
				}
			}
			return running;
		}

		/// <summary>
		/// 停止监听
		/// </summary>
		public static void Stop()
		{
			if (!running) return;
			running = false;
			try { if (listener != null) listener.Stop(); } catch (Exception) { }
			listener = null;
			handler = null;
		}

		#region 接收

		private static void AcceptLoop()
		{
			while (running)
			{
				TcpClient client;
				try
				{
					client = listener.AcceptTcpClient();
				}
				catch (Exception)
				{
					break; // 监听被 Stop
				}
				TcpClient c = client;
				ThreadPool.QueueUserWorkItem(_ => HandleClientSafe(c));
			}
		}

		private static void HandleClientSafe(TcpClient client)
		{
			try { HandleClient(client); }
			catch (Exception e) { UnityEngine.Debug.LogWarning("[UnityMcp] 连接处理异常: " + e.Message); }
			finally { try { client.Close(); } catch (Exception) { } }
		}

		private static void HandleClient(TcpClient client)
		{
			NetworkStream stream = client.GetStream();
			string head = ReadHead(stream);
			if (head == null) return;

			string[] lines = head.Split('\n');
			string[] reqLine = lines[0].Trim().Split(' ');
			if (reqLine.Length < 2) return;
			string method = reqLine[0].ToUpperInvariant();

			// 不支持服务器主动推送 → 405，客户端会静默禁用它
			if (method == "GET")
			{
				Write(stream, new Response
				{
					Status = 405,
					Reason = "Method Not Allowed",
					Json = "{\"error\":\"server-initiated stream not supported\"}",
				});
				return;
			}
			// 会话终止请求 → 我们无会话，回 204
			if (method == "DELETE")
			{
				Write(stream, new Response { Status = 204, Reason = "No Content" });
				return;
			}
			if (method != "POST")
			{
				Write(stream, new Response { Status = 405, Reason = "Method Not Allowed" });
				return;
			}

			int contentLength = 0;
			string auth = null;
			for (int i = 0; i < lines.Length; i++)
			{
				int c = lines[i].IndexOf(':');
				if (c <= 0) continue;
				string k = lines[i].Substring(0, c).Trim();
				string v = lines[i].Substring(c + 1).Trim();
				if (k.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)) int.TryParse(v, out contentLength);
				else if (k.Equals("Authorization", StringComparison.OrdinalIgnoreCase)) auth = v;
			}

			if (!CheckAuth(auth))
			{
				// 鉴权是 HTTP 边界的事，这里直接回一个 JSON-RPC 形状的错误体，客户端好读
				Write(stream, new Response
				{
					Status = 401,
					Reason = "Unauthorized",
					Json = "{\"jsonrpc\":\"2.0\",\"id\":null,\"error\":{\"code\":-32001,"
						 + "\"message\":\"鉴权失败：需要 Authorization: Bearer <token>（见 .dshexec/mcp-token.txt）\"}}",
				});
				return;
			}

			byte[] bodyBytes = ReadBody(stream, contentLength);
			Request req = new Request
			{
				Method = method,
				Path = reqLine[1],
				Authorization = auth,
				Body = Encoding.UTF8.GetString(bodyBytes),
			};

			Response resp;
			try
			{
				resp = handler != null ? handler(req) : new Response { Status = 503, Reason = "Service Unavailable" };
			}
			catch (Exception e)
			{
				resp = new Response
				{
					Status = 500,
					Reason = "Internal Server Error",
					Json = "{\"jsonrpc\":\"2.0\",\"id\":null,\"error\":{\"code\":-32603,\"message\":\"服务器内部错误: "
						 + Escape(e.Message) + "\"}}",
				};
			}
			Write(stream, resp);
		}

		/// <summary>
		/// token 文件存在且非空时要求匹配；否则不鉴权（仅本机回环）
		/// </summary>
		private static bool CheckAuth(string authHeader)
		{
			try
			{
				string tokenFile = UnityMcpPaths.TokenFile;
				if (!File.Exists(tokenFile)) return true;
				string expect = File.ReadAllText(tokenFile).Trim();
				if (string.IsNullOrEmpty(expect)) return true;
				if (string.IsNullOrEmpty(authHeader)) return false;
				string v = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
					? authHeader.Substring(7).Trim()
					: authHeader.Trim();
				return string.Equals(v, expect, StringComparison.Ordinal);
			}
			catch (Exception)
			{
				return false;
			}
		}

		#endregion

		#region 最小 HTTP

		/// <summary>
		/// 读到 \r\n\r\n 为止，返回请求头文本（含空行）；失败返回 null
		/// </summary>
		private static string ReadHead(NetworkStream s)
		{
			MemoryStream buf = new MemoryStream(1024);
			byte[] one = new byte[1];
			while (true)
			{
				int n;
				try { n = s.Read(one, 0, 1); }
				catch (Exception) { return null; }
				if (n <= 0) return null;
				buf.WriteByte(one[0]);
				byte[] a = buf.GetBuffer();
				int c = (int)buf.Length;
				if (c >= 4 && a[c - 4] == 13 && a[c - 3] == 10 && a[c - 2] == 13 && a[c - 1] == 10)
					return Encoding.ASCII.GetString(a, 0, c);
				if (c > 64 * 1024) return null;
			}
		}

		private static byte[] ReadBody(NetworkStream s, int length)
		{
			if (length <= 0) return new byte[0];
			if (length > 16 * 1024 * 1024) length = 16 * 1024 * 1024;
			byte[] body = new byte[length];
			int off = 0;
			while (off < length)
			{
				int n;
				try { n = s.Read(body, off, length - off); }
				catch (Exception) { break; }
				if (n <= 0) break;
				off += n;
			}
			return body;
		}

		private static void Write(NetworkStream s, Response r)
		{
			if (r == null) r = new Response();
			byte[] body = string.IsNullOrEmpty(r.Json) ? new byte[0] : Encoding.UTF8.GetBytes(r.Json);
			StringBuilder h = new StringBuilder();
			h.Append("HTTP/1.1 ").Append(r.Status).Append(' ').Append(r.Reason).Append("\r\n");
			h.Append("Content-Type: application/json\r\n");
			h.Append("Content-Length: ").Append(body.Length).Append("\r\n");
			h.Append("Connection: close\r\n\r\n");
			byte[] hb = Encoding.ASCII.GetBytes(h.ToString());
			try
			{
				s.Write(hb, 0, hb.Length);
				if (body.Length > 0) s.Write(body, 0, body.Length);
				s.Flush();
			}
			catch (Exception) { }
		}

		private static string Escape(string s)
		{
			if (string.IsNullOrEmpty(s)) return "";
			return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ");
		}

		#endregion
	}
}
