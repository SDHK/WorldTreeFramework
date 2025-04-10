using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{

	[CustomEditor(typeof(UnityWorldTree))]
	public class UnityWorldTreeEditor : Editor
	{
		private static UnityWorldTree script;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (!Application.isPlaying) return;

			script = target as UnityWorldTree;
			//GUILayout.Label(NodeRule.ToStringDrawTree(script.WorldLineManager.ma));
		}
	}

	[InitializeOnLoad]
	public static class MyAATest
	{
		static MyAATest()
		{
			//重点！！！后续处理
			Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
		}

		static bool markable = false;
		static void OnPostHeaderGUI(Editor editor)
		{
			//if (!editor.target.IsPrefabDefinition()) return;
			markable = GUILayout.Toggle(markable, new GUIContent("MyAA", "tips"));
			GUILayout.Label(editor.target.name);
			if (!markable) return;
			EditorGUILayout.DropdownButton(new GUIContent("MyAA", "tips"), FocusType.Keyboard);
		}
	}
}
