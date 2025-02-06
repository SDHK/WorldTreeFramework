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
		/// 序列化节点
		/// </summary>
		public static byte[] SerializeNode(INode self)
		{
			if (self.IsDisposed) return null;
			NodeBranchTraversalHelper.TraversalPostorder(self, current => current.Core.SerializeRuleGroup.Send(current));

			self.AddTemp(out TreeDataByteSequence sequence);
			if (self?.Parent == null) return null;
			if (NodeBranchHelper.TryGetBranch(self.Parent, self.BranchType, out IBranch branch))
			{
				branch.RemoveNode(self.Id);
			}
			sequence.Serialize(self);
			byte[] bytes = sequence.ToBytes();
			sequence.Dispose();
			return bytes;
		}

		/// <summary>
		/// 反序列化节点
		/// </summary>
		public static N DeseralizeNode<N>(INode self, byte[] bytes)
			where N : class, INode
		{
			self.AddTemp(out TreeDataByteSequence sequence).SetBytes(bytes);
			INode treeSpade = null;
			sequence.Deserialize(ref treeSpade);
			sequence.Dispose();
			return treeSpade as N;
		}


		/// <summary>
		/// 反序列化节点
		/// </summary>
		public static TreeData GetTreeData(INode self, byte[] bytes)
		{
			self.AddTemp(out TreeDataByteSequence sequence).SetBytes(bytes);
			TreeData treeData = sequence.GetTreeData();
			sequence.Dispose();
			return treeData;
		}
	}

	/// <summary>
	/// 树数据节点
	/// </summary>
	public class TreeData : Node
		, NumberNodeOf<TreeData>
		, TempOf<INode>
		, AsNumberNodeBranch
		, AsAwake
	{
		/// <summary>
		/// 是否为数组
		/// </summary>
		public bool IsArray = false;

		/// <summary>
		/// 类型名称
		/// </summary>
		public string TypeName;

		/// <summary>
		/// 是否默认值
		/// </summary>
		public bool IsDefault = false;

		public override string ToString()
		{
			return $"[TreeData] [{(IsArray ? "Array" : "Class")}: {this.TypeName}] -";
		}
	}

	/// <summary>
	/// 树数值
	/// </summary>
	public class TreeValue : TreeData
	{
		/// <summary>
		/// 数据
		/// </summary>
		public object Value;

		public override string ToString()
		{
			return $"{base.ToString()} Value: {Value}";
		}
	}

	public static class TreeDataRule
	{
		class Remove : RemoveRule<TreeData>
		{
			protected override void Execute(TreeData self)
			{
				self.IsArray = false;
				self.TypeName = null;
				self.IsDefault = false;
			}
		}
	}

}
