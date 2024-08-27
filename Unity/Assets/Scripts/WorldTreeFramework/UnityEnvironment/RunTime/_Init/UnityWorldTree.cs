/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述：世界树框架驱动器，一切从这里开始

*/

using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 世界树框架驱动器，一切从这里开始
	/// </summary>
	public class UnityWorldTree : MonoBehaviour
	{
		/// <summary>
		/// 主框架
		/// </summary>
		public WorldTreeCore Core;
		/// <summary>
		/// 可视化框架
		/// </summary>
		public WorldTreeCore ViewCore;

		/// <summary>
		/// 可视化生成器
		/// </summary>
		private UnityWorldTreeNodeViewBuilder treeView;

		/// <summary>
		/// 启动
		/// </summary>
		public void Start()
		{
			Core = new();//主框架

#if UNITY_EDITOR
			ViewCore = new();//调试用的可视化框架
			ViewCore.Log = Debug.Log;
			ViewCore.LogWarning = Debug.LogWarning;
			ViewCore.LogError = Debug.LogError;
			ViewCore.Awake(); //可视化框架初始化

			ViewCore.Root.AddChild(out treeView, (INode)Core, default(INode));
#endif

			Core.Log = Debug.Log;
			Core.LogWarning = Debug.LogWarning;
			Core.LogError = Debug.LogError;

			//可视化生成器赋值给主框架
			Core.View = treeView;

			//主框架初始化
			Core.Awake();

			//主框架添加Unity世界心跳，间隔毫秒为0
			Core.Root.AddComponent(out UnityWorldHeart _, 0).Run();

			//主框架添加入口节点
			Core.Root.AddComponent(out Entry _);
		}

		/// <summary>
		/// 更新
		/// </summary>
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return)) Debug.Log(NodeRule.ToStringDrawTree(Core));
		}

		/// <summary>
		/// 退出
		/// </summary>
		private void OnApplicationQuit()
		{
			Core?.Dispose();
			ViewCore?.Dispose();
			Core = null;
			ViewCore = null;
			treeView = null;
		}

		/// <summary>
		/// 销毁
		/// </summary>
		private void OnDestroy()
		{
			Core?.Dispose();
			ViewCore?.Dispose();
			Core = null;
			ViewCore = null;
			treeView = null;
		}
	}
}