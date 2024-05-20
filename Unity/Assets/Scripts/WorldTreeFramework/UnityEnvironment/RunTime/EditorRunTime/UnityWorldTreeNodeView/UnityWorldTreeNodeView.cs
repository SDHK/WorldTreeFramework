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
		public INode Node;
		public IWorldTreeNodeViewBuilder View;

		public void Start()
		{
			gameObject.name = Node.GetType().Name;
		}
	}
}
