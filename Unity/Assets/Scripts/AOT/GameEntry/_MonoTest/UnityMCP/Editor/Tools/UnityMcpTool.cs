/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： MCP 工具基类

* 每个工具只声明四件事：名字 / 描述 / 入参 schema / 怎么执行。
* 信封（tools/list 的条目、tools/call 的返回包装）由这里统一负责，
* 工具本身只关心"返回一段文本"。
* 新增工具 = 继承 UnityMcpTool + 在 UnityMcpToolRegistry 里加一行。

****************************************/

using Newtonsoft.Json.Linq;
using System;

namespace UnityMCP.Editor
{
	/// <summary>
	/// 一次工具调用的结果
	/// </summary>
	public class UnityMcpToolResult
	{
		/// <summary>
		/// 返回给模型的文本
		/// </summary>
		public string Text;

		/// <summary>
		/// 是否算失败（MCP 的 isError）
		/// </summary>
		public bool IsError;

		/// <summary>
		/// 响应**写回之后**才在主线程执行的动作（例如触发域重载的 Refresh）
		/// </summary>
		public Action Post;

		public static UnityMcpToolResult Ok(string text) { return new UnityMcpToolResult { Text = text }; }
		public static UnityMcpToolResult Fail(string text) { return new UnityMcpToolResult { Text = text, IsError = true }; }
	}

	/// <summary>
	/// MCP 工具基类
	/// </summary>
	public abstract class UnityMcpTool
	{
		/// <summary>
		/// 工具名（tools/call 里的 name）
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// 工具描述（给模型看的，写清楚"能做什么 / 有什么坑"）
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// 入参 JSON Schema
		/// </summary>
		protected abstract JObject BuildSchema();

		private JObject schema;
		public JObject InputSchema { get { return schema != null ? schema : (schema = BuildSchema()); } }

		/// <summary>
		/// tools/list 里的一项
		/// </summary>
		public JObject Descriptor()
		{
			return new JObject
			{
				["name"] = Name,
				["description"] = Description,
				["inputSchema"] = InputSchema,
			};
		}

		/// <summary>
		/// 执行（统一兜异常，工具内部不必自己 try）
		/// </summary>
		public UnityMcpToolResult Invoke(JObject args)
		{
			try { return Run(args != null ? args : new JObject()); }
			catch (Exception e) { return UnityMcpToolResult.Fail(e.ToString()); }
		}

		/// <summary>
		/// 具体实现
		/// </summary>
		protected abstract UnityMcpToolResult Run(JObject args);

		#region schema 与参数 小工具

		/// <summary>
		/// 一个属性
		/// </summary>
		protected static JObject P(string type, string desc)
		{
			return new JObject { ["type"] = type, ["description"] = desc };
		}

		/// <summary>
		/// object 型 schema
		/// </summary>
		protected static JObject ObjectSchema(JObject properties, params string[] required)
		{
			JObject o = new JObject { ["type"] = "object", ["properties"] = properties };
			if (required != null && required.Length > 0) o["required"] = new JArray(required);
			return o;
		}

		protected static string Str(JObject a, string key, string def = null)
		{
			JToken t = a != null ? a[key] : null;
			if (t == null || t.Type == JTokenType.Null) return def;
			string v = t.Type == JTokenType.String ? (string)t : t.ToString();
			return v != null ? v : def;
		}

		protected static int Int(JObject a, string key, int def)
		{
			JToken t = a != null ? a[key] : null;
			if (t == null || t.Type == JTokenType.Null) return def;
			try { return t.Value<int>(); } catch (Exception) { return def; }
		}

		protected static bool Bool(JObject a, string key, bool def)
		{
			JToken t = a != null ? a[key] : null;
			if (t == null || t.Type == JTokenType.Null) return def;
			try { return t.Value<bool>(); } catch (Exception) { return def; }
		}

		#endregion
	}
}
