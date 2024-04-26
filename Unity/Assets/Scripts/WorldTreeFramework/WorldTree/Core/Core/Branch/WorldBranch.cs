/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/02 03:08:51

* 描述： 世界分支
*
* 框架核心分支
* 根据类型为键挂载，只能挂在核心节点上
*
*

*/

namespace WorldTree
{
	public interface AsWorldBranch : AsBranch<WorldBranch>
	{ }

	/// <summary>
	/// 世界树核心约束
	/// </summary>
	/// <typeparam name="P"></typeparam>
	public interface WorldOf<in P> : NodeOf<P, WorldBranch> where P : class, INode
	{ }

	/// <summary>
	/// 世界分支
	/// </summary>
	public class WorldBranch : Branch<long>
	{ }

	public static class NodeWorldBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取世界
		/// </summary>
		public static bool TryGetWorld(this INode self, long type, out INode core)
			=> (core = self.GetBranch<WorldBranch>()?.GetNode(type)) != null;

		/// <summary>
		/// 尝试获取世界
		/// </summary>
		public static bool TryGetWorld<N, T>(this N self, out T core)
			where N : class, INode where T : class, INode, NodeOf<N, WorldBranch>
		=> (core = self.GetBranch<WorldBranch>()?.GetNode(TypeInfo<T>.TypeCode) as T) != null;

		#endregion

		#region 移除

		/// <summary>
		/// 移除世界
		/// </summary>
		public static void RemoveWorld<T>(this INode self) where T : class, INode
			=> self.GetBranch<WorldBranch>()?.GetNode(TypeInfo<T>.TypeCode)?.Dispose();

		/// <summary>
		/// 移除世界
		/// </summary>
		public static void RemoveWorld(this INode self, long type)
			=> self.GetBranch<WorldBranch>()?.GetNode(type)?.Dispose();

		/// <summary>
		/// 移除全部组件
		/// </summary>
		public static void RemoveAllWorld(this INode self)
			=> self.RemoveAllNode(TypeInfo<WorldBranch>.TypeCode);

		#endregion

		#region 添加

		/// <summary>
		/// 添加世界
		/// </summary>
		public static T AddWorld<N, T>(this N self, out T core)
			where N : class, INode, AsBranch<WorldBranch>
			where T : class, INode, NodeOf<N, WorldBranch>, AsRule<IAwakeRule>
		=> self.AddNode<N, WorldBranch, long, T>(TypeInfo<T>.TypeCode, out core, isPool: false);

		/// <summary>
		/// 添加世界
		/// </summary>
		public static T AddWorld<N, T, T1>(this N self, out T core, T1 arg1)
		  where N : class, INode, AsBranch<WorldBranch>
		  where T : class, INode, NodeOf<N, WorldBranch>, AsRule<IAwakeRule<T1>>
		=> self.AddNode<N, WorldBranch, long, T, T1>(TypeInfo<T>.TypeCode, out core, arg1, isPool: false);

		#endregion
	}
}