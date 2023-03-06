/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 11:35

* 描述： 节点法则

*/

namespace WorldTree
{
    public static class NodeStaticRule
    {
        /// <summary>
        /// 类型转换为
        /// </summary>
        public static T To<T>(this Node self)
        where T : Node
        {
            return self as T;
        }

        /// <summary>
        /// 父节点转换为
        /// </summary>
        public static T ParentTo<T>(this Node self)
        where T : Node
        {
            return self.Parent as T;
        }
        /// <summary>
        /// 尝试转换父节点
        /// </summary>
        public static bool TryParentTo<T>(this Node self, out T node)
        where T : Node
        {
            node = self.Parent as T;
            return node != null;
        }

        /// <summary>
        /// 向上查找父物体
        /// </summary>
        public static T FindParent<T>(this Node self)
        where T : Node
        {
            self.TryFindParent(out T parent);
            return parent;
        }

        /// <summary>
        /// 尝试向上查找父物体
        /// </summary>
        public static bool TryFindParent<T>(this Node self, out T parent)
        where T : Node
        {
            parent = null;
            Node node = self.Parent;
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
        public static void RemoveAll(this Node self)
        {
            self.RemoveAllChildren();
            self.RemoveAllComponent();
        }

        /// <summary>
        /// 从父节点中删除
        /// </summary>
        public static void RemoveInParent(this Node self)
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
                    self.Parent.m_Children.Remove(self.id);
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
        public static string ToStringDrawTree(this Node self, string t = "\t")
        {
            string t1 = "\t" + t;
            string str = "";

            str += t1 + $"[{self.id:0}] " + self.ToString() + "\n";

            if (self.m_Components != null)
            {
                if (self.m_Components.Count > 0)
                {
                    str += t1 + "   Components:\n";
                    foreach (var item in self.Components.Values)
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
                    foreach (var item in self.Children.Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }
            return str;
        }
    }
}
