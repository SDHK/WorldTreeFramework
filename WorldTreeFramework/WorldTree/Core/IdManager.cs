﻿/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 一个编号分发的管理器
*/

using System.Collections.Generic;

namespace WorldTree
{

    public static class IdManagerExtension
    {
        public static IdManager IdManager(this Entity self)
        {
            return self.Root.IdManager;
        }
    }

    /// <summary>
    /// id管理器
    /// </summary>
    public class IdManager : Entity
    {
        /// <summary>
        /// id池
        /// </summary>
        public Queue<long> idPool = new Queue<long>();


        /// <summary>
        /// 当前递增的id值
        /// </summary>
        public long currentId = 0;

        /// <summary>
        /// 获取id后递增
        /// </summary>
        public long GetId()
        {
            if (!idPool.TryDequeue(out var value))
            {
                value = currentId++;
            }
            return value;
        }

        /// <summary>
        /// 回收id
        /// </summary>
        public void Recycle(long id)
        {
            if (!idPool.Contains(id))
            {
                idPool.Enqueue(id);
            }
        }


        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            idPool.Clear();
            IsRecycle = true;
            IsDisposed = true;
        }
    }
}
