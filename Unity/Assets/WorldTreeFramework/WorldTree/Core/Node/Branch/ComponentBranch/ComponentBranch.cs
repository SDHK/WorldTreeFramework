using System.Collections.Generic;
using System.ComponentModel;

namespace WorldTree
{

	/// <summary>
	/// 组件：父节点限制
	/// </summary>
	/// <typeparam name="N">父节点类型</typeparam>
	/// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>
	public interface ComponentOf<in N> : BranchOf<N, ComponentBranch> where N : class, INode { }


	/// <summary>
	/// 组件分支
	/// </summary>
	public class ComponentBranch : UnitDictionary<long, INode>, IBranch
	{
		public INode Self { get; set; }
		public void RemoveNode(INode node)
		{
			this.Remove(node.Type);
		}
		public void RemoveAllNode()
		{
			if (this.Count == 0) return;
			using (Self.PoolGet(out UnitStack<INode> nodes))
			{
				foreach (var item in this) nodes.Push(item.Value);
				int length = nodes.Count;
				for (int i = 0; i < length; i++) nodes.Pop().Dispose();
			}
		}
		IEnumerator<INode> IEnumerable<INode>.GetEnumerator()
		{
			return this.Values.GetEnumerator();
		}

		/// <summary>
		/// 尝试添加组件
		/// </summary>
		public bool TryAddNode<T>(out T Node, bool isPool = true)
			where T : class, INode
		{
			var type = TypeInfo<T>.HashCode64;
			if (this.TryGetValue(type, out INode node))
			{
				node = isPool ? Self.Core.GetNode(type) : Self.Core.NewNodeLifecycle(type);
				node.BranchType = TypeInfo<ComponentBranch>.HashCode64;
				node.Parent = Self;
				this.Add(type, node);
			}
			Node = node as T;
			return Node != null;
		}

		/// <summary>
		/// 尝试接入外部树结构
		/// </summary>
		public bool TryGraftNode(long key, INode node)
		{
			if (this.TryAdd(key, node))
			{
				node.BranchType = TypeInfo<ComponentBranch>.HashCode64;
				node.Parent = Self;
				node.TreeGraftSelf();
				return true;
			}
			return false;
		}
	}

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
				return branch.TryGetValue(type, out component);
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
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>
		{
			if (self.TryGetBranch(out ComponentBranch branch))
			{
				if (branch.TryGetValue(TypeInfo<T>.HashCode64, out INode node))
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

		#region 接入

		/// <summary>
		/// 外部接入组件
		/// </summary>
		public static void GraftComponent<N, T>(this N self, T component)
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>
		{
			if (component != null)
			{
				self.AddBranch<ComponentBranch>().TryGraftNode(TypeInfo<T>.HashCode64, component);
			}
		}

		#endregion

		#region 移除

		/// <summary>
		/// 移除组件,未完
		/// </summary>
		public static void RemoveComponent<T>(this INode self)
			where T : class, INode
		{
			if (self.TryGetBranch(out ComponentBranch branch))
			{
				branch.Remove(TypeInfo<T>.HashCode64);
			}
		}

		/// <summary>
		/// 移除组件,未完
		/// </summary>
		public static void RemoveComponent(this INode self, long type)
		{
			if (self.TryGetBranch(out ComponentBranch branch))
			{
				branch.Remove(type);
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
				foreach (var item in branch) nodes.Push(item.Value);

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
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>, AsRule<IAwakeRule>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component))
			{
				Component.SendRule(DefaultType<IAwakeRule>.Default);
				Component.TreeAddSelf();
			}
			return Component;
		}

		public static T AddComponent<N, T, T1>(this N self, out T Component, T1 arg1)
		  where N : class, INode
		  where T : class, INode, BranchOf<N, ComponentBranch>, AsRule<IAwakeRule<T1>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1>>.Default, arg1);
				Component.TreeAddSelf();
			}
			return Component;
		}
		public static T AddComponent<N, T, T1, T2>(this N self, out T Component, T1 arg1, T2 arg2)
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>, AsRule<IAwakeRule<T1, T2>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2>>.Default, arg1, arg2);
				Component.TreeAddSelf();
			}
			return Component;
		}

		public static T AddComponent<N, T, T1, T2, T3>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3)
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>, AsRule<IAwakeRule<T1, T2, T3>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3>>.Default, arg1, arg2, arg3);
				Component.TreeAddSelf();
			}
			return Component;
		}

		public static T AddComponent<N, T, T1, T2, T3, T4>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4>>.Default, arg1, arg2, arg3, arg4);
				Component.TreeAddSelf();
			}
			return Component;
		}
		public static T AddComponent<N, T, T1, T2, T3, T4, T5>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			where N : class, INode
			where T : class, INode, BranchOf<N, ComponentBranch>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4, T5>>.Default, arg1, arg2, arg3, arg4, arg5);
				Component.TreeAddSelf();
			}
			return Component;
		}

		#endregion


		#region 非池

		public static T AddNewComponent<N, T>(this N self, out T Component)
		  where N : class, INode
		  where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component, false))
			{
				Component.SendRule(DefaultType<IAwakeRule>.Default);
				Component.TreeAddSelf();
			}
			return Component;
		}

		public static T AddNewComponent<N, T, T1>(this N self, out T Component, T1 arg1)
		 where N : class, INode
		 where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component, false))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1>>.Default, arg1);
				Component.TreeAddSelf();
			}
			return Component;
		}
		public static T AddNewComponent<N, T, T1, T2>(this N self, out T Component, T1 arg1, T2 arg2)
			where N : class, INode
			where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component, false))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2>>.Default, arg1, arg2);
				Component.TreeAddSelf();
			}
			return Component;
		}

		public static T AddNewComponent<N, T, T1, T2, T3>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3)
			where N : class, INode
			where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component, false))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3>>.Default, arg1, arg2, arg3);
				Component.TreeAddSelf();
			}
			return Component;
		}

		public static T AddNewComponent<N, T, T1, T2, T3, T4>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			where N : class, INode
			where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component, false))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4>>.Default, arg1, arg2, arg3, arg4);
				Component.TreeAddSelf();
			}
			return Component;
		}
		public static T AddNewComponent<N, T, T1, T2, T3, T4, T5>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			where N : class, INode
			where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
		{
			if (self.AddBranch<ComponentBranch>().TryAddNode(out Component, false))
			{
				Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4, T5>>.Default, arg1, arg2, arg3, arg4, arg5);
				Component.TreeAddSelf();
			}
			return Component;
		}

		#endregion

		#endregion

	}

}
