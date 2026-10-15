/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： MCP 工具 —— unity_refresh：触发 Unity 重新导入/编译

* 关键在时序：响应必须**先写回**，刷新才执行 ——
* 否则刷新触发的域重载会把这条响应一起干掉。
* 所以这里把 AssetDatabase.Refresh() 放进 Post（服务器在响应写回后才跑它）。

****************************************/

using Newtonsoft.Json.Linq;
using UnityEditor;

namespace UnityMCP.Editor
{
	/// <summary>
	/// 触发 AssetDatabase.Refresh()
	/// </summary>
	public class UnityRefreshTool : UnityMcpTool
	{
		public override string Name { get { return "unity_refresh"; } }

		public override string Description
		{
			get
			{
				return "触发 AssetDatabase.Refresh() 让 Unity 重新导入并编译（改完 C# 后调用）。"
					+ "响应先返回、刷新在响应写回之后才执行 —— 因为刷新会触发域重载，本服务器会随之重启"
					+ "（重启后日志环形缓冲会清空，端口需几秒重新就绪）。";
			}
		}

		protected override JObject BuildSchema()
		{
			return ObjectSchema(new JObject());
		}

		protected override UnityMcpToolResult Run(JObject args)
		{
			return new UnityMcpToolResult
			{
				Text = "已排队：AssetDatabase.Refresh() 将在本次响应写回后执行（会触发域重载，服务器随后重启）",
				Post = () => AssetDatabase.Refresh(),
			};
		}
	}
}
