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
		public WorldLine MainLine;
		/// <summary>
		/// 可视化框架
		/// </summary>
		public WorldLine ViewLine;

		/// <summary>
		/// 可视化生成器
		/// </summary>
		private UnityWorldTreeNodeViewBuilder viewBuilder;

		/// <summary>
		/// 启动
		/// </summary>
		public void Start()
		{
			MainLine = new();//主框架

			if (Define.IsEditor)
			{
				ViewLine = new();
				//可视化框架初始化
				ViewLine.Init(typeof(UnityWorldHeart), 0);
				ViewLine.World.AddChild(out viewBuilder, (INode)MainLine, default(INode));
			}

			MainLine.ViewBuilder = viewBuilder;
			//主框架初始化，添加Unity世界心跳，间隔毫秒为0
			MainLine.Init(typeof(UnityWorldHeart), 0);

			//主框架添加入口节点
			MainLine.World.AddComponent(out Entry _);
		}

		/// <summary>
		/// 更新
		/// </summary>
		private void Update()
		{

		}

		/// <summary>
		/// 退出
		/// </summary>
		private void OnApplicationQuit()
		{
			MainLine?.Dispose();
			ViewLine?.Dispose();
			MainLine = null;
			ViewLine = null;
			viewBuilder = null;
		}

		/// <summary>
		/// 销毁
		/// </summary>
		private void OnDestroy()
		{
			MainLine?.Dispose();
			ViewLine?.Dispose();
			MainLine = null;
			ViewLine = null;
			viewBuilder = null;
		}
	}
}