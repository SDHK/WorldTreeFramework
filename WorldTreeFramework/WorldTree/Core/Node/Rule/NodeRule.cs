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
        /// 获取父类型法则列表
        /// </summary>
        public static IRuleList<R> BaseRule<N, R, B>(this AsBaseRule<N, B> self, R defaultRule)
            where R : IRule
            where N : class, B, INode
            where B : class, INode
        {
            return self.Core.RuleManager.GetRuleList<R, B>();
        }



        /// <summary>
        /// 回收自己
        /// </summary>
        public static void DisposeSelf(this INode self)
        {
            if (!self.IsRecycle)//是否已经回收
            {
                self.RemoveInParent();//从父节点中移除
                self.Core.RemoveNode(self);//全局通知移除
                self.DisposeDomain();//清除域节点
                self.Parent = null;//清除父节点

                self.OnDispose();
            }
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
                if (node.Type == typeof(T))
                {
                    parent = node as T;
                    break;
                }
                node = node.Parent;
            }
            return parent != null;
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
        /// 从父节点中删除
        /// </summary>
        public static void RemoveInParent(this INode self)
        {
            if (self.Parent != null)
            {
                if (self.isComponent)
                {
                    self.Parent.m_Components.Remove(self.GetType());
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
