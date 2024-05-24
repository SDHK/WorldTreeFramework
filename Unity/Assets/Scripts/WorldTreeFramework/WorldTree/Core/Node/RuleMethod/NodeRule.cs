/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 11:35

* 描述： 节点法则

*/

namespace WorldTree
{
	/// <summary>
	/// 节点内部扩展
	/// </summary>
	public static partial class NodeInternalExtend
	{
		#region 节点处理扩展

		#region 释放

		///// <summary>
		///// 根据键值释放分支的节点
		///// </summary>
		//public static void RemoveNode<B, K>(INode self, K key)
		//	where B : class, IBranch<K>
		//=> self.GetBranch<B>()?.GetNode(key)?.Dispose();

		///// <summary>
		///// 根据id释放分支的节点
		///// </summary>
		//public static void RemoveNodeById<B>(INode self, long id) where B : class, IBranch => self.GetBranch<B>()?.GetNodeById(id)?.Dispose();

		#endregion

		#region 裁剪

		///// <summary>
		///// 树结构尝试裁剪节点
		///// </summary>
		//public static bool TryCutNodeById<B>(INode self, long id, out INode node) where B : class, IBranch => (node = self.GetBranch<B>()?.GetNodeById(id)?.CutSelf()) != null;

		///// <summary>
		///// 树结构尝试裁剪节点
		///// </summary>
		//public static bool TryCutNode<B, K>(INode self, K key, out INode node) where B : class, IBranch<K> => (node = self.GetBranch<B>()?.GetNode(key)?.CutSelf()) != null;

		///// <summary>
		///// 树结构裁剪节点
		///// </summary>
		//public static INode CutNodeById<B>(INode self, long id) where B : class, IBranch => self.GetBranch<B>()?.GetNodeById(id)?.CutSelf();

		///// <summary>
		///// 树结构裁剪节点
		///// </summary>
		//public static INode CutNode<B, K>(INode self, K key) where B : class, IBranch<K> => self.GetBranch<B>()?.GetNode(key)?.CutSelf();

		#endregion

		#region 获取

		///// <summary>
		///// 节点Id包含判断
		///// </summary>
		//public static bool ContainsId<B>(INode self, long id) where B : class, IBranch => self.GetBranch<B>()?.ContainsId(id) ?? false;

		///// <summary>
		///// 节点键值包含判断
		///// </summary>
		//public static bool Contains<B, K>(INode self, K key) where B : class, IBranch<K> => self.GetBranch<B>()?.Contains(key) ?? false;

		///// <summary>
		///// 树结构尝试获取节点
		///// </summary>
		//public static bool TryGetNodeById<B>(INode self, long id, out INode node) where B : class, IBranch => (node = self.GetBranch<B>()?.GetNodeById(id)) != null;

		///// <summary>
		///// 树结构尝试获取节点
		///// </summary>
		//public static bool TryGetNode<B, K>(INode self, K key, out INode node) where B : class, IBranch<K> => (node = self.GetBranch<B>()?.GetNode(key)) != null;

		///// <summary>
		///// 树结构获取节点
		///// </summary>
		//public static INode GetNodeById<B>(INode self, long Id) where B : class, IBranch => self.GetBranch<B>()?.GetNodeById(Id);

		///// <summary>
		///// 树结构获取节点
		///// </summary>
		//public static INode GetNode<B, K>(INode self, K key) where B : class, IBranch<K> => self.GetBranch<B>()?.GetNode(key);

		#endregion

		#endregion
	}

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
		public static INode GetOrNewNode(this INode self, long type, bool isPool = true) => (isPool ? self.PoolGetNode(type) : self.NewNodeLifecycle(type));

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