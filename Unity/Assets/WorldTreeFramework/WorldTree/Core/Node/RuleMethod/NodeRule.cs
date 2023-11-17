/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 11:35

* 描述： 节点法则

*/

using System;
using static Codice.CM.Common.CmCallContext;

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
			self.Core.RuleManager.TryGetRuleList(TypeInfo<N>.HashCode64, out IRuleList<R> rulelist);
			return rulelist;
		}



		///// <summary>
		///// 移除全部组件和子节点
		///// </summary>
		//public static void RemoveAll(this INode self)
		//{
		//	self.RemoveAllChildren();
		//	self.RemoveAllComponent();
		//}


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
				if (node.Type == TypeInfo<T>.HashCode64)
				{
					parent = node as T;
					break;
				}
				node = node.Parent;
			}
			return parent != null;
		}



		//添加
		//获取事件通知
		//添加父节点
		//添加域节点
		//添加到父分支
		//初始化事件通知
		//添加到引用池
		//激活变更
		//激活事件通知
		//广播给全部监听器
		//是否为监听器注册
		//节点添加事件通知

		//倒序遍历

		//即将移除事件通知 X?
		//从父节点分支移除
		//_判断移除引用关系 X
		//激活变更
		//禁用事件通知 X
		//是否为监听器注册
		//移除事件通知
		//广播给全部监听器通知 X
		//引用池移除 ?
		//清除域节点
		//清除父节点
		//回到对象池，回收事件通知 X


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
						str += node.ToStringDrawTree(t1);
					}
				}
			}
			return str;
		}
	}
}
