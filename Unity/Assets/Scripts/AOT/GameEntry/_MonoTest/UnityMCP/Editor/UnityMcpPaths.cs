/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： Unity MCP 的运行时路径集中定义

* 只有一个地方拼路径，改名 / 换位置都只动这里。

****************************************/

using System.IO;
using UnityEngine;

namespace UnityMCP.Editor
{
	/// <summary>
	/// Unity MCP 运行时路径
	/// </summary>
	public static class UnityMcpPaths
	{
		/// <summary>
		/// 工程根目录（Assets 的父目录）
		/// </summary>
		public static readonly string ProjectRoot = Directory.GetParent(Application.dataPath).FullName;

		/// <summary>
		/// 运行时目录（token、诊断用 refs.json 都放这里）
		/// 按需存在即可：目录不存在 = 无 token、无引用白名单
		/// </summary>
		public static readonly string RuntimeDir = Path.Combine(ProjectRoot, ".unitymcp");

		/// <summary>
		/// 可选的访问 token 文件：存在且非空时要求 Authorization: Bearer &lt;token&gt;
		/// </summary>
		public static string TokenFile { get { return Path.Combine(RuntimeDir, "mcp-token.txt"); } }

		/// <summary>
		/// 可选的引用白名单（诊断用，见 CSharpExecutor 头部注释）
		/// </summary>
		public static string RefsFile { get { return Path.Combine(RuntimeDir, "refs.json"); } }
	}
}
