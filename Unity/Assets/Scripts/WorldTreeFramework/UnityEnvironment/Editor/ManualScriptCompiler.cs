using UnityEditor;
using UnityEngine;

namespace EditorTool
{
	/// <summary>
	/// 手动脚本编译器
	/// </summary>
	public class ManualScriptCompiler
	{
		[MenuItem("WorldTree/编译 _F5")] // F5
		public static void CompileScripts()
		{
			AssetDatabase.Refresh();
			EditorUtility.RequestScriptReload();
			Debug.Log("脚本编译完毕！");
		}
	}
}
