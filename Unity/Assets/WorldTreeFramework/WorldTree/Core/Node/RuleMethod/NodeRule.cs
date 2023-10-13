/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 11:35

* 描述： 节点法则

*/

using System;

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
            self.Core.RuleManager.TryGetRuleList(TypeInfo<N>.HashCode64,out IRuleList<R> rulelist);
            return rulelist;
        }


        /// <summary>
        /// 从父节点中删除
        /// </summary>
        public static void RemoveInParent(this INode self)
        {
            if (self.Parent != null)
            {
                if (self.isComponent)
                {
                    self.Parent.m_Components.Remove(self.Type);
                    if (self.Parent.m_Components.Count == 0)
                    {
                        self.Parent.m_Components.Dispose();
                        self.Parent.m_Components = null;
                    }
                }
                else
                {
                    self.Parent.m_Children.Remove(self.Id);
                    if (self.Parent.m_Children.Count == 0)
                    {
                        self.Parent.m_Children.Dispose();
                        self.Parent.m_Children = null;
                    }
                }
            }
        }

        /// <summary>
        /// 回收自己
        /// </summary>
        public static void DisposeSelf(this INode self)
        {
            if (!self.IsRecycle)//是否已经回收
            {
                WorldTreeCore core = self.Core;
                INode current;
                UnitStack<INode> stack = self.PoolGet<UnitStack<INode>>();
                UnitStack<INode> allStack = self.PoolGet<UnitStack<INode>>();
                stack.Push(self);
                while (stack.Count != 0)
                {
                    current = stack.Pop();

                    //前序通知
                    core.BeforeRemoveRuleGroup?.Send(current);

                    allStack.Push(current);
                    if (current.m_Children != null)
                    {
                        foreach (var item in current.m_Children)
                        {
                            stack.Push(item.Value);
                        }
                    }
                    if (current.m_Components != null)
                    {
                        foreach (var item in current.m_Components)
                        {
                            stack.Push(item.Value);
                        }
                    }
                }
                stack.Dispose();
                while (allStack.Count != 0)
                {
                    //后序遍历
                    current = allStack.Pop();
                    core.RemoveNode(current);//全局通知移除
                    current.OnDispose();//回收到池
                }
                allStack.Dispose();
            }
        }


        /// <summary>
        /// 移除全部组件和子节点
        /// </summary>
        public static void RemoveAll(this INode self)
        {
            self.RemoveAllChildren();
            self.RemoveAllComponent();
        }


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
		/// 在节点添加时的处理
		/// </summary>
		public static void OnNodeAdd(this INode self)
		{
			self.Core.ReferencedPoolManager.TryAdd(self);//添加到引用池
			self.SetActive(true);//激活变更
			self.Core.EnableRuleGroup?.Send(self);//激活事件通知
			if (self is not ICoreNode)//广播给全部监听器
			{
				self.GetListenerActuator<IListenerAddRule>()?.Send(self);
			}
			if (self is INodeListener nodeListener && self is not ICoreNode)
			{
				//检测添加静态监听
				self.Core.ReferencedPoolManager.TryAddStaticListener(nodeListener);
			}
			self.Core.AddRuleGroup?.Send(self);//节点添加事件通知
		}

		/// <summary>
		/// 在节点移除时的处理
		/// </summary>
		public static void OnNodeRemove(this INode self)
		{
			if (self.IsRecycle) return; //是否已经回收

			WorldTreeCore core = self.Core;
			INode current;
			UnitStack<INode> stack = self.PoolGet<UnitStack<INode>>();
			UnitStack<INode> allStack = self.PoolGet<UnitStack<INode>>();
			stack.Push(self);
			//前序遍历通知
			while (stack.Count != 0)
			{
				current = stack.Pop();
				core.BeforeRemoveRuleGroup?.Send(current);
				allStack.Push(current);
				if (current.m_Branchs != null)
				{
					foreach (var branchs in current.m_Branchs)
					{
						foreach (INode node in branchs.Value)
						{
							stack.Push(node);
						}
					}
				}
			}
			stack.Dispose();

			//后序遍历回收
			while (allStack.Count != 0)
			{
				current = allStack.Pop();
				current.RemoveInParentBranch();//从父节点分支移除
				current.SendAllReferencedNodeRemove();//_判断移除引用关系 X
				current.SetActive(false);//激活变更
				core.DisableRuleGroup?.Send(current); //禁用事件通知 X
				if (current is INodeListener nodeListener && current is not ICoreNode)
				{
					//检测移除静态监听
					core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
					//检测移除动态监听
					core.ReferencedPoolManager.RemoveDynamicListener(nodeListener);
				}
				core.RemoveRuleGroup?.Send(current);//移除事件通知
				if (current is not ICoreNode)//广播给全部监听器通知 X
				{
					current.GetListenerActuator<IListenerRemoveRule>()?.Send(current);
				}
				core.ReferencedPoolManager.Remove(current);//引用池移除 ?
				current.DisposeDomain(); //清除域节点
				current.Parent = null;//清除父节点
				current.OnDispose();//回收到池
			}
			allStack.Dispose();
		}


		/// <summary>
		/// 返回用字符串绘制的树
		/// </summary>
		public static string ToStringDrawTree(this INode self, string t = "\t")
        {
            string t1 = "\t" + t;
            string str = "";

            str += t1 + $"[{self.Id:0}] " + self.ToString() + "\n";

            if (self.m_Components != null)
            {
                if (self.m_Components.Count > 0)
                {
                    str += t1 + "   Components:\n";
                    foreach (var item in self.ComponentsDictionary().Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }

            if (self.m_Children != null)
            {
                if (self.m_Children.Count > 0)
                {
                    str += t1 + "   Children:\n";
                    foreach (var item in self.ChildrenDictionary().Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }
            return str;
        }
    }
}
