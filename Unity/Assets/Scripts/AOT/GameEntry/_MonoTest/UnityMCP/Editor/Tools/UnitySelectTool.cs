/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： MCP 工具 —— unity_select：在编辑器里选中一个 GameObject

* 语义沿用旧 UnityMCP 通道的 selectGameObject（GameObject.Find + Selection），
* 另外补了三点：未命中时按名字搜已加载场景对象（含未激活）、命中多个时列候选、
* 返回解析结果（路径 / instanceID / 激活状态 / 场景）便于确认。

****************************************/

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityMCP.Editor
{
	/// <summary>
	/// 选中 GameObject
	/// </summary>
	public class UnitySelectTool : UnityMcpTool
	{
		public override string Name { get { return "unity_select"; } }

		public override string Description
		{
			get
			{
				return "在编辑器里选中一个 GameObject（名称或层级路径，如 Canvas/Panel/Button），并在 Hierarchy 里 Ping 高亮。"
					+ "先走 GameObject.Find（只命中激活对象）；未命中时按名字搜索已加载的**场景**对象（含未激活，排除预制体等资源），"
					+ "命中多个时返回候选路径，请改用完整层级路径重试。";
			}
		}

		protected override JObject BuildSchema()
		{
			return ObjectSchema(new JObject
			{
				["target"] = P("string", "GameObject 名称或层级路径，如 Canvas/Panel/Button"),
				["ping"] = P("boolean", "是否在 Hierarchy 里 Ping 高亮，默认 true"),
			}, "target");
		}

		protected override UnityMcpToolResult Run(JObject args)
		{
			string target = Str(args, "target");
			if (string.IsNullOrEmpty(target))
				return UnityMcpToolResult.Fail("缺少参数 target（GameObject 名称或层级路径，如 Canvas/Panel/Button）");

			bool ping = Bool(args, "ping", true);

			GameObject go = GameObject.Find(target);
			if (go == null)
			{
				List<GameObject> cands = new List<GameObject>();
				GameObject[] loaded = Resources.FindObjectsOfTypeAll<GameObject>();
				for (int i = 0; i < loaded.Length; i++)
				{
					GameObject c = loaded[i];
					if (c == null || !c.scene.IsValid()) continue;   // 排除预制体等资源
					if (c.name != target) continue;
					cands.Add(c);
				}

				if (cands.Count == 1)
				{
					go = cands[0];
				}
				else if (cands.Count > 1)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("按名字命中 ").Append(cands.Count).Append(" 个（含未激活），请用完整层级路径重试：\n");
					for (int i = 0; i < cands.Count && i < 20; i++) sb.Append("  ").Append(PathOf(cands[i])).Append('\n');
					return UnityMcpToolResult.Fail(sb.ToString());
				}
				else
				{
					return UnityMcpToolResult.Fail("未找到 GameObject: " + target);
				}
			}

			Selection.activeGameObject = go;
			if (ping) EditorGUIUtility.PingObject(go);

			return UnityMcpToolResult.Ok("已选中: " + PathOf(go)
				+ "\ninstanceID=" + go.GetInstanceID()
				+ "  activeInHierarchy=" + go.activeInHierarchy
				+ "  scene=" + go.scene.name);
		}

		/// <summary>
		/// 层级路径
		/// </summary>
		private static string PathOf(GameObject go)
		{
			StringBuilder sb = new StringBuilder(go.name);
			Transform t = go.transform.parent;
			while (t != null)
			{
				sb.Insert(0, t.name + "/");
				t = t.parent;
			}
			return sb.ToString();
		}
	}
}
