/****************************************

* 作者：闪电黑客
* 日期：2024/9/3 12:08

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 序列化帮助类
	/// </summary>
	public static class TreeDataHelper
	{
		/// <summary>
		/// 序列化数据节点类型
		/// </summary>
		public static byte[] SerializeNode(INode self)
		{
			if (self.IsDisposed) return null;
			NodeBranchTraversalHelper.TraversalPostorder(self, current => current.Core.SerializeRuleGroup.Send(current));

			self.AddTemp(out TreeDataByteSequence sequence);
			if (self?.Parent == null) return null;
			sequence.Serialize(self);
			byte[] bytes = sequence.ToBytes();
			sequence.Dispose();
			return bytes;
		}

		/// <summary>
		/// 反序列化数据节点类型
		/// </summary>
		public static N DeseralizeNode<N>(INode self, byte[] bytes)
			where N : class, INode
		{
			self.AddTemp(out TreeDataByteSequence sequence).SetBytes(bytes);
			N treeSpade = null;
			sequence.Deserialize(ref treeSpade);
			sequence.Dispose();
			return treeSpade;
		}


		/// <summary>
		/// 序列化树数据
		/// </summary>
		public static byte[] SerializeTreeData(TreeData treeData)
		{
			treeData.Parent.AddTemp(out TreeDataByteSequence sequence);
			sequence.SerializeTreeData(treeData);
			byte[] bytes = sequence.ToBytes();
			sequence.Dispose();
			return bytes;
		}

		/// <summary>
		/// 反序列化为通用数据结构
		/// </summary>
		public static TreeData DeserializeTreeData(INode self, byte[] bytes)
		{
			self.AddTemp(out TreeDataByteSequence sequence).SetBytes(bytes);
			TreeData treeData = sequence.DeserializeTreeData();
			sequence.Dispose();
			return treeData;
		}
	}

}
