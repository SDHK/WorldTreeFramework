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
	///// <summary>
	///// 组件：父节点限制
	///// </summary>
	///// <typeparam name="N">父节点类型</typeparam>
	///// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>
	//public interface ComponentOf<in N> { }

	/// <summary>
	/// 组件节点
	/// </summary>
	public interface AsComponent<in N> : AsNode<ComponentBranch, N> where N : class, INode { }


	/// <summary>
	/// 组件分支
	/// </summary>
	public class ComponentBranch : Branch<long> { }


	public static class NodeComponentBranchRule
	{
		#region 获取

		/// <summary>
		/// 尝试获取组件
		/// </summary>
		public static bool TryGetComponent(this INode self, long type, out INode component) => self.TryGetNode<ComponentBranch, long>(type, out component);

		/// <summary>
		/// 尝试获取组件
		/// </summary>
		public static bool TryGetComponent<N, T>(this N self, out T component) where N : class, INode, AsNode<ComponentBranch, T> where T : class, INode
		=> (component = self.TryGetNode<ComponentBranch, long>(TypeInfo<T>.HashCode64, out INode node) ? node as T : null) != null;


		#endregion

		#region 裁剪

		/// <summary>
		/// 裁剪组件
		/// </summary>
		public static bool TryCutComponent<T>(this INode self, out T node) where T : class, INode => (node = self.TryCutNode<ComponentBranch, long>(TypeInfo<T>.HashCode64, out INode Inode) ? Inode as T : null) != null;

		/// <summary>
		/// 裁剪组件
		/// </summary>
		public static bool TryCutComponent(this INode self, long type, out INode node) => self.TryCutNode<ComponentBranch, long>(type, out node);

		#endregion

		#region 嫁接

		/// <summary>
		/// 外部接入组件
		/// </summary>
		public static void GraftComponent<N, T>(this N self, T component)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode
		=> self.TreeGraftNode<ComponentBranch, long>(TypeInfo<T>.HashCode64, component);

		#endregion

		#region 移除

		/// <summary>
		/// 移除组件
		/// </summary>
		public static void RemoveComponent<T>(this INode self) where T : class, INode => self.RemoveNode<ComponentBranch, long>(TypeInfo<T>.HashCode64);

		/// <summary>
		/// 移除组件
		/// </summary>
		public static void RemoveComponent(this INode self, long type) => self.RemoveNode<ComponentBranch, long>(type);

		/// <summary>
		/// 移除全部组件
		/// </summary>
		public static void RemoveAllComponent(this INode self) => self.RemoveAllNode<ComponentBranch>();

		#endregion

		#region 添加

		#region 池

		/// <summary>
		/// 添加组件
		/// </summary>
		public static T AddComponent<N, T>(this N self, out T Component)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule>
		{
			return self.TreeAddNode<ComponentBranch, long, T>(TypeInfo<T>.HashCode64, out Component);
		}

		public static T AddComponent<N, T, T1>(this N self, out T Component, T1 arg1)
		  where N : class, INode, AsNode<ComponentBranch, T>
		  where T : class, INode, AsRule<IAwakeRule<T1>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1>(TypeInfo<T>.HashCode64, out Component, arg1);
		}
		public static T AddComponent<N, T, T1, T2>(this N self, out T Component, T1 arg1, T2 arg2)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2>(TypeInfo<T>.HashCode64, out Component, arg1, arg2);
		}

		public static T AddComponent<N, T, T1, T2, T3>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2, T3>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2, T3>(TypeInfo<T>.HashCode64, out Component, arg1, arg2, arg3);
		}

		public static T AddComponent<N, T, T1, T2, T3, T4>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2, T3, T4>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2, T3, T4>(TypeInfo<T>.HashCode64, out Component, arg1, arg2, arg3, arg4);
		}
		public static T AddComponent<N, T, T1, T2, T3, T4, T5>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2, T3, T4, T5>(TypeInfo<T>.HashCode64, out Component, arg1, arg2, arg3, arg4, arg5);
		}

		#endregion


		#region 非池

		public static T AddNewComponent<N, T>(this N self, out T Component)
		  where N : class, INode, AsNode<ComponentBranch, T>
		  where T : class, INode, AsRule<IAwakeRule>
		{
			return self.TreeAddNode<ComponentBranch, long, T>(TypeInfo<T>.HashCode64, out Component, isPool: false);
		}

		public static T AddNewComponent<N, T, T1>(this N self, out T Component, T1 arg1)
		 where N : class, INode, AsNode<ComponentBranch, T>
		 where T : class, INode, AsRule<IAwakeRule<T1>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1>(TypeInfo<T>.HashCode64, out Component, arg1, isPool: false);
		}
		public static T AddNewComponent<N, T, T1, T2>(this N self, out T Component, T1 arg1, T2 arg2)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2>(TypeInfo<T>.HashCode64, out Component, arg1, arg2, isPool: false);
		}

		public static T AddNewComponent<N, T, T1, T2, T3>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2, T3>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2, T3>(TypeInfo<T>.HashCode64, out Component, arg1, arg2, arg3, isPool: false);
		}

		public static T AddNewComponent<N, T, T1, T2, T3, T4>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2, T3, T4>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2, T3, T4>(TypeInfo<T>.HashCode64, out Component, arg1, arg2, arg3, arg4, isPool: false);
		}
		public static T AddNewComponent<N, T, T1, T2, T3, T4, T5>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		{
			return self.TreeAddNode<ComponentBranch, long, T, T1, T2, T3, T4, T5>(TypeInfo<T>.HashCode64, out Component, arg1, arg2, arg3, arg4, arg5, isPool: false);
		}

		#endregion

		#endregion

	}

}
