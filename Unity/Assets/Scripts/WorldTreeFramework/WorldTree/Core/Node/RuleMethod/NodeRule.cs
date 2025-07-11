/****************************************

* 作者：闪电黑客
* 日期：2025/6/5 18:16

* 描述：

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
			self.Core.RuleManager.TryGetRuleList(self.TypeToCode<B>(), out IRuleList<R> rulelist);
			return rulelist;
		}

		/// <summary>
		/// 类型转换为
		/// </summary>
		public static T As<T>(this INode self) where T : class, INode => self as T;

		/// <summary>
		/// 设置父节点，指定子节点分支裁剪和嫁接
		/// </summary>
		public static void SetParent<N, P>(this N self, P parent)
			where N : class, INode, ChildOf<P>
			where P : class, INode, AsChildBranch
		{
			self.CutSelf()?.TryGraftSelfToTree<ChildBranch, long>(self.Id, parent);
		}

		/// <summary>
		/// 获取父节点
		/// </summary>
		public static P GetParent<N, P>(this N self, out P parent)
			where P : class, INode
			where N : NodeOf<P>
			=> parent = self.Parent as P;

		/// <summary>
		/// 尝试获取转换父节点,无约束
		/// </summary>
		public static bool TryGetParent<T>(this INode self, out T node)
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
			long typeCode = self.TypeToCode<T>();
			while (node != null)
			{
				if (node.Type == typeCode)
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
		public static string ToStringDrawTree(INode self, string t = "\t")
		{
			string t1 = "\t" + t;
			string str = "";

			str += $"{t1}[{self.Id:0}] {self} \n";

			if (self.BranchDict != null)
			{
				foreach (var branchs in self.BranchDict)
				{
					if (branchs.Value == null)
					{
						str += $"{t1}   Null: \n";
						continue;
					}
					str += $"{t1}   {branchs.Value.GetType().Name}: \n";
					foreach (INode node in branchs.Value)
					{
						if (branchs.Value.Type == node.BranchType)
						{
							str += ToStringDrawTree(node, t1);
						}
					}
				}
			}
			return str;
		}
	}
}