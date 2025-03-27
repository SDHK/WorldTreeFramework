/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

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
		private UnityWorldTreeNodeViewBuilder viewBuilder;

		/// <summary>
		/// 启动
		/// </summary>
		public void Start()
		{
			Core = new();//主框架

			if (Define.IsEditor)
			{
				ViewCore = new();//调试用的可视化框架
				ViewCore.Log = Debug.Log;
				ViewCore.LogWarning = Debug.LogWarning;
				ViewCore.LogError = Debug.LogError;
				ViewCore.Init(typeof(UnityWorldHeart), 0); //可视化框架初始化
				ViewCore.World.AddChild(out viewBuilder, (INode)Core, default(INode));
			}

			Core.Log = Debug.Log;
			Core.LogWarning = Debug.LogWarning;
			Core.LogError = Debug.LogError;

			//可视化生成器赋值给主框架
			Core.View = viewBuilder;

			//主框架初始化，添加Unity世界心跳，间隔毫秒为0
			Core.Init(typeof(UnityWorldHeart), 0);

			//主框架添加入口节点
			Core.World.AddComponent(out Entry _);
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
			viewBuilder = null;
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
			viewBuilder = null;
		}
	}
}