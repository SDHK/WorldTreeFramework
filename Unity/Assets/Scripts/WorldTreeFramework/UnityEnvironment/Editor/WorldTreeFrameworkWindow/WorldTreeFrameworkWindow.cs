
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 11:46

* 描述： 世界树框架编辑器驱动窗口

*/

using System;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
	/// <summary>
	/// 世界树框架编辑器驱动窗口
	/// </summary>
	public class WorldTreeFrameworkWindow : Node


		//, AsRule<IGUIDrawSystem>
		, AsRule<IEditorWindowInspectorUpdateSystem>

	{
		public static WorldTreeCore root;

		[MenuItem("WorldTree/Window")]
		static void ShowWindow()
		{
			//if (Core == null)
			//{
			//    Core = new WorldTreeCore();
			//    World.Log = Debug.Log;
			//    World.LogWarning = Debug.LogWarning;
			//    World.LogError = Debug.LogError;
			//}
			//Core.AddComponent<WorldTreeFrameworkWindow>();
		}


		public EditorHomePage HomePage;
	}

	class WorldTreeFrameworkWindowAddSystem : AddRule<WorldTreeFrameworkWindow>
	{
		protected override void Execute(WorldTreeFrameworkWindow self)
		{
			self.AddComponent(out EditorGUIWindow _);
			//self.GetOrNewGlobalRuleActuator(out self.enable);
			//self.GetOrNewGlobalRuleActuator(out self.update);
			//self.GetOrNewGlobalRuleActuator(out self.disable);
			//self.GetOrNewGlobalRuleActuator(out self.onGUI);
			self.AddComponent(out EditorHomePage _);
		}
	}


	class WorldTreeFrameworkWindowEditorWindowInspectorUpdateSystem : EditorWindowInspectorUpdateSystem<WorldTreeFrameworkWindow>
	{
		protected override void Execute(WorldTreeFrameworkWindow self)
		{
			//self.enable.Send();
			//self.update.Send(TimeSpan.FromSeconds(0.02));
			//self.disable.Send();
		}
	}

}
