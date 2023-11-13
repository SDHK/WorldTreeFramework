/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/24 22:00

* 描述： 组件分支
* 
* 主要分支之一
* 设定为根据类型为键进行存储，
* 所以在同一节点下，同一类型的组件只能有一个
* 

*/


namespace WorldTree
{
	/// <summary>
	/// 组件：父节点限制
	/// </summary>
	/// <typeparam name="N">父节点类型</typeparam>
	/// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>
	public interface ComponentOf<in N> { }


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
		public static bool TryGetComponent(this INode self, long type, out INode component)
		{
			if (self.TryGetBranch(out ComponentBranch branch))
			{
				return branch.TryGetNode(type, out component);
			}
			else
			{
				component = null;
				return false;
			}
		}

		/// <summary>
		/// 尝试获取组件
		/// </summary>
		public static bool TryGetComponent<N, T>(this N self, out T component)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode
		{
			if (self.TryGetBranch(out ComponentBranch branch))
			{
				if (branch.TryGetNode(TypeInfo<T>.HashCode64, out INode node))
				{
					component = node as T;
					return true;
				}
				else
				{
					component = null;
					return false;
				}
			}
			else
			{
				component = null;
				return false;
			}
		}

		#endregion

		#region 裁剪

		#endregion

		#region 嫁接

		/// <summary>
		/// 外部接入组件
		/// </summary>
		public static void GraftComponent<N, T>(this N self, T component)
			where N : class, INode, AsNode<ComponentBranch, T>
			where T : class, INode
		{
			self.TreeGraftNode<ComponentBranch, long, T>(TypeInfo<T>.HashCode64, component);
		}

		#endregion

		#region 移除

		/// <summary>
		/// 移除组件
		/// </summary>
		public static void RemoveComponent<T>(this INode self)
			where T : class, INode
		{
			self.RemoveComponent(TypeInfo<T>.HashCode64);
		}

		/// <summary>
		/// 移除组件
		/// </summary>
		public static void RemoveComponent(this INode self, long type)
		{
			if (self.TryGetBranch(out ComponentBranch branch))
			{
				if (branch.TryGetNode(type, out INode node))
				{
					node.Dispose();
				}
			}
		}

		/// <summary>
		/// 移除全部组件
		/// </summary>
		public static void RemoveAllComponent(this INode self)
		{
			if (self.TryGetBranch(out ComponentBranch branch))
			{
				var nodes = self.PoolGet<UnitStack<INode>>();
				foreach (var item in branch) nodes.Push(item);

				int length = nodes.Count;
				for (int i = 0; i < length; i++)
				{
					nodes.Pop().Dispose();
				}
				nodes.Dispose();
			}
		}

		#endregion

		#region AddComponent

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
