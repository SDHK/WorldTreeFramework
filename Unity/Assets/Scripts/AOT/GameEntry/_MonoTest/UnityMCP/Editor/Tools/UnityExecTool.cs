/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： MCP 工具 —— unity_exec：在主线程执行一段 C#

****************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityMCP.Editor
{
	/// <summary>
	/// 执行 C# 语句
	/// </summary>
	public class UnityExecTool : UnityMcpTool
	{
		public override string Name { get { return "unity_exec"; } }

		public override string Description
		{
			get
			{
				return "在 Unity 编辑器主线程执行一段 C# 语句（填方法体，可以 return 一个值，返回值会被 ToString）。"
					+ "可用 UnityEngine / UnityEditor / System / System.Linq / System.Collections.Generic，"
					+ "以及所有已加载程序集（含 WorldTree 框架）的类型。"
					+ "代码跑在编辑器主线程，死循环会卡死编辑器，长任务请拆小；"
					+ "要返回结构化数据请自己在代码里序列化（例如 Newtonsoft.Json.JsonConvert）。";
			}
		}

		protected override JObject BuildSchema()
		{
			return ObjectSchema(new JObject
			{
				["code"] = P("string", "C# 语句，可 return 一个值"),
			}, "code");
		}

		protected override UnityMcpToolResult Run(JObject args)
		{
			string code = Str(args, "code");
			if (string.IsNullOrEmpty(code)) return UnityMcpToolResult.Fail("缺少参数 code");

			// 只捕获本次执行期间的日志（与 UnityMcpLogBuffer 的常驻缓冲互补）
			List<string> logs = new List<string>();
			Application.LogCallback handler = (msg, stack, type) => logs.Add("[" + type + "] " + msg);
			Application.logMessageReceived += handler;
			try
			{
				object ret = CSharpExecutor.Execute(code);
				StringBuilder sb = new StringBuilder();
				sb.Append("[ok] ").Append(ret == null ? "null" : ret.ToString());
				if (logs.Count > 0) sb.Append("\n--- unity log ---\n").Append(string.Join("\n", logs));
				return UnityMcpToolResult.Ok(sb.ToString());
			}
			catch (Exception e)
			{
				return UnityMcpToolResult.Fail(e.Message);
			}
			finally
			{
				Application.logMessageReceived -= handler;
			}
		}
	}
}
