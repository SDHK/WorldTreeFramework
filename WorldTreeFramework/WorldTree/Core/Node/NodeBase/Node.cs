/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

using System;

namespace WorldTree
{
    public interface INode : IUnitPoolItem
    { 
    
    
    }

    /// <summary>
    /// 世界树节点基类
    /// </summary>
    public abstract partial class Node : INode
    {
        public IPool thisPool { get; set; }

        public bool IsRecycle { get; set; }

        public bool IsDisposed { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        /// <remarks>在框架内唯一</remarks>
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
        /// 回收节点
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsRecycle)//是否已经回收
            {
                this.RemoveInParent();//从父节点中移除
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

    }

}
