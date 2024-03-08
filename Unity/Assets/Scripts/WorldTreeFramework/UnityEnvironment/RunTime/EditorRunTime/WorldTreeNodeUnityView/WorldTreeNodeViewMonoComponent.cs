/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/24 06:17:03

* 描述：世界树框架节点显示Mono面板组件

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 世界树框架节点显示Mono面板组件
	/// </summary>
	public class WorldTreeNodeViewMonoComponent : MonoBehaviour
	{
		public INode Node;
		public IWorldTreeNodeView View;

		public void Start()
		{
			gameObject.name = Node.GetType().Name;
		}
	}
}
