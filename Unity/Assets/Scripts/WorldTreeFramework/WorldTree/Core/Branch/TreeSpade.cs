/****************************************

* 作者：闪电黑客
* 日期：2024/11/8 11:50

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 树节点移植器基类
	/// </summary>
	public abstract class TreeSpade : Unit
	{
		/// <summary>
		/// 分支类型码
		/// </summary>
		public long BranchType;

		/// <summary>
		/// 节点嫁接到树上
		/// </summary>
		public abstract bool TryGraftSelfToTree(INode parent);
	}

	/// <summary>
	/// 树节点移植器
	/// </summary>
	public class TreeSpade<K> : TreeSpade
	{
		/// <summary>
		/// 键值
		/// </summary>
		public K Key;

		/// <summary>
		/// 节点
		/// </summary>
		public INode Node;

		public override bool TryGraftSelfToTree(INode parent)
		=> Node.TryGraftSelfToTree(BranchType, Key, parent);

		public override void OnDispose()
		{
			BranchType = default;
			Key = default;
			Node = default;
			base.OnDispose();
		}
	}

}