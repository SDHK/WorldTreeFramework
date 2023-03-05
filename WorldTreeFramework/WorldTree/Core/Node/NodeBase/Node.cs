/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

using System;

namespace WorldTree
{

    /// <summary>
    /// 世界树节点基类
    /// </summary>
    public abstract partial class Node : IUnitPoolItem
    {
        public IPool thisPool { get; set; }

        public bool IsRecycle { get; set; }

        public bool IsDisposed { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        public long id;

        /// <summary>
        /// 节点类型
        /// </summary>
        public Type Type;

        /// <summary>
        /// 根节点
        /// </summary>
        public WorldTreeRoot Root;

        /// <summary>
        /// 父节点
        /// </summary>
        public Node Parent;


        public Node()
        {
            Type = GetType();
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        /// <summary>
        /// 类型转换为
        /// </summary>
        public T To<T>()
        where T : Node
        {
            return this as T;
        }

        /// <summary>
        /// 父节点转换为
        /// </summary>
        public T ParentTo<T>()
        where T : Node
        {
            return Parent as T;
        }
        /// <summary>
        /// 尝试转换父节点
        /// </summary>
        public bool TryParentTo<T>(out T node)
        where T : Node
        {
            node = Parent as T;
            return node != null;
        }



        /// <summary>
        /// 移除全部组件和子节点
        /// </summary>
        public void RemoveAll()
        {
            RemoveAllChildren();
            RemoveAllComponent();
        }

        /// <summary>
        /// 向上查找父物体
        /// </summary>
        public T FindParent<T>()
        where T : Node
        {
            TryFindParent(out T parent);
            return parent;
        }

        /// <summary>
        /// 尝试向上查找父物体
        /// </summary>
        public bool TryFindParent<T>(out T parent)
        where T : Node
        {
            parent = null;
            Node node = Parent;
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
        /// 从父节点中删除
        /// </summary>
        public void RemoveInParent()
        {
            if (Parent != null)
            {
                if (isComponent)
                {
                    Parent.components.Remove(GetType());
                    if (Parent.components.Count == 0)
                    {
                        Parent.components.Dispose();
                        Parent.components = null;
                    }
                }
                else
                {
                    Parent.children.Remove(this.id);
                    if (Parent.children.Count == 0)
                    {
                        Parent.children.Dispose();
                        Parent.children = null;
                    }
                }
            }
        }


        /// <summary>
        /// 回收节点
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsRecycle)//是否已经回收
            {
                RemoveInParent();//从父节点中移除
                Root.Remove(this);//全局通知移除
                this.DisposeDomain();//清除域节点
                Parent = null;//清除父节点

                OnDispose();
            }
        }

        /// <summary>
        /// 释放后：回收到对象池
        /// </summary>
        public virtual void OnDispose()
        {
            thisPool?.Recycle(this);
        }

        //===============================================

        /// <summary>
        /// 返回用字符串绘制的树
        /// </summary>
        public string ToStringDrawTree(string t = "\t")
        {
            string t1 = "\t" + t;
            string str = "";

            str += t1 + $"[{this.id:0}] " + this.ToString() + "\n";

            if (this.components != null)
            {
                if (this.components.Count > 0)
                {
                    str += t1 + "   Components:\n";
                    foreach (var item in this.Components.Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }

            if (this.children != null)
            {
                if (this.children.Count > 0)
                {
                    str += t1 + "   Children:\n";
                    foreach (var item in this.Children.Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }
            return str;
        }
    }

}
