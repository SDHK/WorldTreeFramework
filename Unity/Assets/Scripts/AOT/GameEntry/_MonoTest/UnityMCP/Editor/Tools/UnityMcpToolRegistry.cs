/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： MCP 工具注册表

* 唯一的"工具清单"—— 新增工具就在这里加一行。
* 顺序即 tools/list 的顺序。

****************************************/

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace UnityMCP.Editor
{
	/// <summary>
	/// MCP 工具注册表
	/// </summary>
	public static class UnityMcpToolRegistry
	{
		private static readonly List<UnityMcpTool> ordered = new List<UnityMcpTool>
		{
			new UnityExecTool(),
			new UnityRefreshTool(),
			new UnityLogsTool(),
			new UnitySelectTool(),
		};

		// 静态字段按文本顺序初始化，ordered 已就绪
		private static readonly Dictionary<string, UnityMcpTool> byName = BuildIndex();

		private static Dictionary<string, UnityMcpTool> BuildIndex()
		{
			Dictionary<string, UnityMcpTool> d = new Dictionary<string, UnityMcpTool>();
			for (int i = 0; i < ordered.Count; i++) d[ordered[i].Name] = ordered[i];
			return d;
		}

		/// <summary>
		/// tools/list 的 tools 数组
		/// </summary>
		public static JArray Descriptors()
		{
			JArray arr = new JArray();
			for (int i = 0; i < ordered.Count; i++) arr.Add(ordered[i].Descriptor());
			return arr;
		}

		/// <summary>
		/// 按名字找工具；没有返回 null
		/// </summary>
		public static UnityMcpTool Find(string name)
		{
			if (string.IsNullOrEmpty(name)) return null;
			UnityMcpTool t;
			return byName.TryGetValue(name, out t) ? t : null;
		}
	}
}
