/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：节点分支处理帮助类

*/

namespace WorldTree

{
	/// <summary>
	/// 节点分支处理帮助类
	/// </summary>
	/// <remarks>无约束不安全的用法</remarks>
	public static partial class NodeBranchHelper
	{
		#region 添加

		/// <summary>
		/// 添加分支
		/// </summary>
		public static B AddBranch<B>(INode self) where B : class, IBranch => AddBranch(self, self.TypeToCode<B>()) as B;

		/// <summary>
		/// 添加分支
		/// </summary>
		public static IBranch AddBranch(INode self, long type)
		{
			// 拿到分支字典
			var branchDict = self.GetBranchDict;
			if (!branchDict.TryGetValue(type, out IBranch iBranch))
			{
				branchDict.Add(type, iBranch = self.Core.PoolGetUnit(type) as IBranch);
			}
			return iBranch;
		}

		#endregion

		#region 移除

		/// <summary>
		/// 移除分支中的节点
		/// </summary>
		public static void RemoveNode(INode self)
		{
			if (self?.Parent == null) return;
			if (TryGetBranch(self.Parent, self.BranchType, out IBranch branch))
			{
				branch.RemoveNode(self.Id);
				if (branch.Count == 0)
				{
					//移除分支
					self.Parent.BranchDict.Remove(self.BranchType);
					if (self.Parent.BranchDict.Count == 0)
					{
						self.Parent.BranchDict.Dispose();
						self.Parent.BranchDict = null;
					}
					//释放分支
					branch.Dispose();
				}
			}
		}

		/// <summary>
		/// 释放分支的所有节点
		/// </summary>
		public static void RemoveAllNode<B>(INode self) where B : class, IBranch
			=> self.RemoveAllNode(self.TypeToCode<B>());

		#endregion

		#region 获取

		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch(INode self, long branchType, out IBranch branch) => (branch = (self.BranchDict != null && self.BranchDict.TryGetValue(branchType, out branch)) ? branch : null) != null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static IBranch GetBranch(INode self, long branchType) => (self.BranchDict != null && self.BranchDict.TryGetValue(branchType, out IBranch iBranch)) ? iBranch : null;


		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch<B>(INode self, out B branch) where B : class, IBranch
			=> (branch = (self.BranchDict != null && self.BranchDict.TryGetValue(self.TypeToCode<B>(), out IBranch Ibranch)) ? Ibranch as B : null) != null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static B GetBranch<B>(INode self) where B : class, IBranch
			=> (self.BranchDict != null && self.BranchDict.TryGetValue(self.TypeToCode<B>(), out IBranch iBranch)) ? iBranch as B : null;

		/// <summary>
		/// 尝试获取键值
		/// </summary>
		public static bool TryGetKey<K>(this INode self, out K key)
		{
			key = default;
			if (self.Parent != null) return false;
			if (!TryGetBranch(self.Parent, self.BranchType, out IBranch branch)) return false;
			if (branch is not IBranch<K> branchKey) return false;
			return branchKey.TryGetNodeKey(self.Id, out key);
		}

		/// <summary>
		/// 获取键值
		/// </summary>
		public static K GetKey<K>(this INode self)
		{
			if (self.Parent != null) return default;
			if (!TryGetBranch(self.Parent, self.BranchType, out IBranch branch)) return default;
			if (branch is not IBranch<K> keyBranch) return default;
			if (!keyBranch.TryGetNodeKey(self.Id, out K key)) return default;
			return key;
		}

		#endregion
	}
}