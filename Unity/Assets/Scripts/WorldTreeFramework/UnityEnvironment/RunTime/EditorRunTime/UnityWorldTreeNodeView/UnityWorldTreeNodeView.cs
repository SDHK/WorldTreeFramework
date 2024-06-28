/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/24 06:17:03

* 描述：世界树节点可视化组件

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 世界树节点可视化组件
	/// </summary>
	public class UnityWorldTreeNodeView : MonoBehaviour
	{
		/// <summary>
		/// 需要可视化的节点
		/// </summary>
		public INode Node;
		/// <summary>
		/// 可视化生成器
		/// </summary>
		public IWorldTreeNodeViewBuilder View;

		/// <summary>
		/// 启动
		/// </summary>
		public void Start()
		{
			gameObject.name = Node.GetType().Name;
		}
	}
}
