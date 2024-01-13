/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 11:35

* 描述： 节点法则

*/

namespace WorldTree
{
	public static partial class NodeRule
	{
		/// <summary>
		/// 获取指定父类型法则列表
		/// </summary>
		public static IRuleList<R> GetBaseRule<N, B, R>(this N self)
			where R : IRule
			where N : class, B, INode
			where B : class, INode, AsRule<R>
		{
			self.Core.RuleManager.TryGetRuleList(TypeInfo<N>.TypeCode, out IRuleList<R> rulelist);
			return rulelist;
		}


		/// <summary>
		/// 获取新建节点
		/// </summary>
		public static N GetOrNewNode<N>(this INode self, bool isPool = true) where N : class, INode => (isPool ? self.PoolGetNode(TypeInfo<N>.TypeCode) : self.NewNodeLifecycle(TypeInfo<N>.TypeCode)) as N;
		/// <summary>
		/// 获取新建节点
		/// </summary>
		public static INode GetOrNewNode(this INode self,long type, bool isPool = true)  => (isPool ? self.PoolGetNode(type) : self.NewNodeLifecycle(type));

		/// <summary>
		/// 类型转换为
		/// </summary>
		public static T To<T>(this INode self)
		where T : class, INode
		{
			return self as T;
		}

		/// <summary>
		/// 父节点转换为
		/// </summary>
		public static T ParentTo<T>(this INode self)
		where T : class, INode
		{
			return self.Parent as T;
		}
		/// <summary>
		/// 尝试转换父节点
		/// </summary>
		public static bool TryParentTo<T>(this INode self, out T node)
		where T : class, INode
		{
			node = self.Parent as T;
			return node != null;
		}

		/// <summary>
		/// 向上查找父物体
		/// </summary>
		public static T FindParent<T>(this INode self)
		where T : class, INode
		{
			self.TryFindParent(out T parent);
			return parent;
		}

		/// <summary>
		/// 尝试向上查找父物体
		/// </summary>
		public static bool TryFindParent<T>(this INode self, out T parent)
		where T : class, INode
		{
			parent = null;
			INode node = self.Parent;
			while (node != null)
			{
				if (node.Type == TypeInfo<T>.TypeCode)
				{
					parent = node as T;
					break;
				}
				node = node.Parent;
			}
			return parent != null;
		}

		/// <summary>
		/// 返回用字符串绘制的树
		/// </summary>
		public static string ToStringDrawTree(this INode self, string t = "\t")
		{
			string t1 = "\t" + t;
			string str = "";

			str += t1 + $"[{self.Id:0}] " + self.ToString() + "\n";

			if (self.m_Branchs != null)
			{
				foreach (var branchs in self.m_Branchs)
				{
					str += t1 + $"   {branchs.Value.GetType().Name}:\n";

					foreach (INode node in branchs.Value)
					{
						if (branchs.Value.Type == node.BranchType)
						{
							str += node.ToStringDrawTree(t1);
						}
					}
				}
			}
			return str;
		}
	}
}
