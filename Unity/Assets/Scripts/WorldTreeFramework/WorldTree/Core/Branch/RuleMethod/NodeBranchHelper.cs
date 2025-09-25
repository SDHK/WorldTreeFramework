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
			IBranch branch = null;
			// 第一个分支，直接创建单个分支
			if (self.BranchDict == null)
			{
				branch = self.Core.PoolGetUnit(type) as IBranch;
				self.BranchDict = branch;
				return branch;
			}

			// 已经是分支集合了,查找是否存在
			if (self.BranchDict.TryGetBranch(type, out branch)) return branch;

			// 需要添加第二个分支，升级为 BranchGroup
			if (self.BranchDict is not BranchGroup)
			{
				IBranch oldBranch = self.BranchDict as IBranch;
				self.BranchDict = self.Core.PoolGetUnit<BranchGroup>();
				self.BranchDict.TryAddBranch(oldBranch.Type, oldBranch);
			}
			branch = self.Core.PoolGetUnit(type) as IBranch;
			self.BranchDict.TryAddBranch(type, branch);
			return branch;
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
					if (self.Parent.BranchDict is BranchGroup branchGroup)
					{
						//移除分支
						self.Parent.BranchDict.RemoveBranch(self.BranchType);
						if (self.Parent.BranchDict.BranchCount == 0)
						{
							self.Parent.BranchDict.Dispose();
							self.Parent.BranchDict = null;
						}
						else if (self.Parent.BranchDict.BranchCount == 1)
						{
							//如果只剩一个分支了，就把分支集合给释放掉，节省内存
							var enumerator = self.Parent.BranchDict.GetEnumerator();
							enumerator.MoveNext();
							var lastBranch = enumerator.Current;
							self.Parent.BranchDict.Dispose();
							self.Parent.BranchDict = null;
							self.Parent.BranchDict = lastBranch;
						}
					}
					else
					{
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
		public static bool TryGetBranch(INode self, long branchType, out IBranch branch) => (branch = (self.BranchDict != null && self.BranchDict.TryGetBranch(branchType, out branch)) ? branch : null) != null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static IBranch GetBranch(INode self, long branchType) => (self.BranchDict != null && self.BranchDict.TryGetBranch(branchType, out IBranch iBranch)) ? iBranch : null;


		/// <summary>
		/// 尝试获取分支
		/// </summary>
		public static bool TryGetBranch<B>(INode self, out B branch) where B : class, IBranch
			=> (branch = (self.BranchDict != null && self.BranchDict.TryGetBranch(self.TypeToCode<B>(), out IBranch Ibranch)) ? Ibranch as B : null) != null;

		/// <summary>
		/// 获取分支
		/// </summary>
		public static B GetBranch<B>(INode self) where B : class, IBranch
			=> (self.BranchDict != null && self.BranchDict.TryGetBranch(self.TypeToCode<B>(), out IBranch iBranch)) ? iBranch as B : null;

		/// <summary>
		/// 尝试获取键值
		/// </summary>
		public static bool TryGetKey<K>(this INode self, out K key)
		{
			key = default;
			if (self.Parent == null) return false;
			if (!TryGetBranch(self.Parent, self.BranchType, out IBranch branch)) return false;
			if (branch is not IBranch<K> branchKey) return false;
			return branchKey.TryGetNodeKey(self.Id, out key);
		}

		/// <summary>
		/// 获取键值
		/// </summary>
		public static K GetKey<K>(this INode self)
		{
			if (self.Parent == null) return default;
			if (!TryGetBranch(self.Parent, self.BranchType, out IBranch branch)) return default;
			if (branch is not IBranch<K> keyBranch) return default;
			if (!keyBranch.TryGetNodeKey(self.Id, out K key)) return default;
			return key;
		}

		#endregion
	}
}