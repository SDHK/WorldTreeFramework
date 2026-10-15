/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： MCP 工具 —— unity_logs：读 Unity 控制台日志（常驻环形缓冲）

****************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityMCP.Editor
{
	/// <summary>
	/// 读控制台日志
	/// </summary>
	public class UnityLogsTool : UnityMcpTool
	{
		/// <summary>
		/// 单次返回的文本上限（防爆上下文）
		/// </summary>
		private const int MaxTextChars = 12000;

		public override string Name { get { return "unity_logs"; } }

		public override string Description
		{
			get
			{
				return "读取 Unity 控制台日志。日志进的是常驻环形缓冲（上限 " + UnityMcpLogBuffer.Max + " 条），"
					+ "所以能拿到本次调用**之前**的日志；但域重载（重编译）会清空它，之后只剩新日志。"
					+ "可按类型过滤，也可读取后清空缓冲，便于只看新增日志。";
			}
		}

		protected override JObject BuildSchema()
		{
			return ObjectSchema(new JObject
			{
				["count"] = P("integer", "显示最近多少条，默认 50，上限 " + UnityMcpLogBuffer.Max),
				["type"] = P("string", "过滤：all(默认) / log / warning / error"),
				["clear"] = P("boolean", "读取后是否清空缓冲，默认 false"),
			});
		}

		protected override UnityMcpToolResult Run(JObject args)
		{
			int count = Int(args, "count", 50);
			string filter = Str(args, "type", "all");
			bool clear = Bool(args, "clear", false);

			if (count <= 0) count = 50;
			if (count > UnityMcpLogBuffer.Max) count = UnityMcpLogBuffer.Max;
			if (filter == null) filter = "all";
			filter = filter.ToLowerInvariant();

			List<UnityMcpLogBuffer.Item> all = UnityMcpLogBuffer.Snapshot();
			if (clear) UnityMcpLogBuffer.Clear();

			List<UnityMcpLogBuffer.Item> pick = new List<UnityMcpLogBuffer.Item>();
			for (int i = 0; i < all.Count; i++)
			{
				LogType t = all[i].Type;
				if (filter == "log" && t != LogType.Log) continue;
				if (filter == "warning" && t != LogType.Warning) continue;
				if (filter == "error" && t != LogType.Error && t != LogType.Exception && t != LogType.Assert) continue;
				pick.Add(all[i]);
			}

			int skip = Math.Max(0, pick.Count - count);
			StringBuilder sb = new StringBuilder();
			sb.Append("缓冲 ").Append(all.Count).Append(" 条，命中 ").Append(pick.Count)
			  .Append(" 条，显示最后 ").Append(pick.Count - skip).Append(" 条")
			  .Append(clear ? "（缓冲已清空）" : "").Append('\n');

			for (int i = skip; i < pick.Count; i++)
			{
				UnityMcpLogBuffer.Item it = pick[i];
				sb.Append('[').Append(it.Time.ToString("HH:mm:ss.fff")).Append("] [").Append(it.Type).Append("] ")
				  .Append(it.Message).Append('\n');
				if (it.Type != LogType.Log && !string.IsNullOrEmpty(it.Stack))
				{
					string[] sl = it.Stack.Split('\n');
					for (int k = 0; k < sl.Length && k < 3; k++) sb.Append("    ").Append(sl[k].Trim()).Append('\n');
				}
			}

			string text = sb.ToString();
			if (text.Length > MaxTextChars) text = text.Substring(text.Length - MaxTextChars);
			return UnityMcpToolResult.Ok(text);
		}
	}
}
