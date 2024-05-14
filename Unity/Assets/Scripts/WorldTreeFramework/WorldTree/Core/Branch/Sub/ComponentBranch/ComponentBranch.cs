/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/24 22:00

* 描述： 组件分支
*
* 主要分支之一
* 设定根据类型为键挂载，
* 所以在同一节点下，同一类型的组件只能有一个
*

*/

namespace WorldTree
{
	public interface AsComponentBranch : AsBranch<ComponentBranch>
	{ }

	/// <summary>
	/// 组件节点约束
	/// </summary>
	/// <typeparam name="P">父节点类型</typeparam>
	/// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>
	public interface ComponentOf<in P> : NodeOf<P, ComponentBranch> where P : class, INode
	{ }

	/// <summary>
	/// 组件分支
	/// </summary>
	public class ComponentBranch : Branch<long>
	{ }

	public static class NodeComponentBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取组件
		/// </summary>
		public static bool TryGetComponent(this INode self, long type, out INode component)
			=> (component = self.GetBranch<ComponentBranch>()?.GetNode(type)) != null;

		/// <summary>
		/// 尝试获取组件
		/// </summary>
		public static bool TryGetComponent<N, T>(this N self, out T component)
			where N : class, INode where T : class, INode, NodeOf<N, ComponentBranch>
		=> (component = (self.GetBranch<ComponentBranch>()?.GetNode(TypeInfo<T>.TypeCode)) as T) != null;

		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪组件
		/// </summary>
		public static bool TryCutComponent<T>(this INode self, out T node)
			where T : class, INode
		=> (node = self.GetBranch<ComponentBranch>()?.GetNode(TypeInfo<T>.TypeCode)?.CutSelf() as T) != null;

		/// <summary>
		/// 裁剪组件
		/// </summary>
		public static bool TryCutComponent(this INode self, long type, out INode node)
			=> (node = self.GetBranch<ComponentBranch>()?.GetNode(type)?.CutSelf()) != null;

		#endregion

		#region 嫁接

		/// <summary>
		/// 尝试外部接入组件
		/// </summary>
		public static bool TryGraftComponent<N, T>(this N self, T component) where N : class, INode where T : class, INode, NodeOf<N, ComponentBranch> => component.TryGraftSelfToTree<ComponentBranch, long>(TypeInfo<T>.TypeCode, self);

		/// <summary>
		/// 尝试外部接入组件
		/// </summary>
		public static bool TryGraftComponent(this INode self, long type, INode component) => component.TryGraftSelfToTree<ComponentBranch, long>(type, self);

		#endregion

		#region 移除

		/// <summary>
		/// 移除组件
		/// </summary>
		public static void RemoveComponent<T>(this INode self)
			where T : class, INode
		=> self.GetBranch<ComponentBranch>()?.GetNode(TypeInfo<T>.TypeCode)?.Dispose();

		/// <summary>
		/// 移除组件
		/// </summary>
		public static void RemoveComponent(this INode self, long type)
			=> self.GetBranch<ComponentBranch>()?.GetNode(type)?.Dispose();

		/// <summary>
		/// 移除全部组件
		/// </summary>
		public static void RemoveAllComponent(this INode self)
			=> self.RemoveAllNode(TypeInfo<ComponentBranch>.TypeCode);

		#endregion

		#region 添加

		#region 类型

		/// <summary>
		/// 添加组件
		/// </summary>
		public static INode AddComponent(this INode self, long type, out INode Component, bool isPool = true)
		=> self.AddNode<ComponentBranch, long>(type, type, out Component, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static INode AddComponent<T1>(this INode self, long type, out INode Component, T1 arg1, bool isPool = true)
		=> self.AddNode<ComponentBranch, long, T1>(type, type, out Component, arg1, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static INode AddComponent<T1, T2>(this INode self, long type, out INode Component, T1 arg1, T2 arg2, bool isPool = true)
		=> self.AddNode<ComponentBranch, long, T1, T2>(type, type, out Component, arg1, arg2, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static INode AddComponent<T1, T2, T3>(this INode self, long type, out INode Component, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
		=> self.AddNode<ComponentBranch, long, T1, T2, T3>(type, type, out Component, arg1, arg2, arg3, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static INode AddComponent<T1, T2, T3, T4>(this INode self, long type, out INode Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
		=> self.AddNode<ComponentBranch, long, T1, T2, T3, T4>(type, type, out Component, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static INode AddComponent<T1, T2, T3, T4, T5>(this INode self, long type, out INode Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
		=> self.AddNode<ComponentBranch, long, T1, T2, T3, T4, T5>(type, type, out Component, arg1, arg2, arg3, arg4, arg5, isPool);

		#endregion

		#region 泛型

		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponent<N, T>(this N self, out T Component, bool isPool = true)
			where N : class, INode, AsComponentBranch
			where T : class, INode, NodeOf<N, ComponentBranch>, AsRule<Awake>
		=> self.AddNode<N, ComponentBranch, long, T>(TypeInfo<T>.TypeCode, out Component, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponent<N, T, T1>(this N self, out T Component, T1 arg1, bool isPool = true)
		  where N : class, INode, AsComponentBranch
		  where T : class, INode, NodeOf<N, ComponentBranch>, AsRule<Awake<T1>>
		=> self.AddNode<N, ComponentBranch, long, T, T1>(TypeInfo<T>.TypeCode, out Component, arg1, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponent<N, T, T1, T2>(this N self, out T Component, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode, AsComponentBranch
			where T : class, INode, NodeOf<N, ComponentBranch>, AsRule<Awake<T1, T2>>
		=> self.AddNode<N, ComponentBranch, long, T, T1, T2>(TypeInfo<T>.TypeCode, out Component, arg1, arg2, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponent<N, T, T1, T2, T3>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode, AsComponentBranch
			where T : class, INode, NodeOf<N, ComponentBranch>, AsRule<Awake<T1, T2, T3>>
		=> self.AddNode<N, ComponentBranch, long, T, T1, T2, T3>(TypeInfo<T>.TypeCode, out Component, arg1, arg2, arg3, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponent<N, T, T1, T2, T3, T4>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode, AsComponentBranch
			where T : class, INode, NodeOf<N, ComponentBranch>, AsRule<Awake<T1, T2, T3, T4>>
		=> self.AddNode<N, ComponentBranch, long, T, T1, T2, T3, T4>(TypeInfo<T>.TypeCode, out Component, arg1, arg2, arg3, arg4, isPool);

		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponent<N, T, T1, T2, T3, T4, T5>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode, AsComponentBranch
			where T : class, INode, NodeOf<N, ComponentBranch>, AsRule<Awake<T1, T2, T3, T4, T5>>
		=> self.AddNode<N, ComponentBranch, long, T, T1, T2, T3, T4, T5>(TypeInfo<T>.TypeCode, out Component, arg1, arg2, arg3, arg4, arg5, isPool);

		#endregion

		#endregion
	}
}