/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 一个编号分发的管理器
* 
* 后续需要改为根据时间生成
*/

using System.Collections.Generic;

namespace WorldTree
{

    /// <summary>
    /// id管理器
    /// </summary>
    public class IdManager : CoreNode, ComponentOf<WorldTreeCore>
    {
        public IdManager()
        {
            Type = GetType();
        }

        /// <summary>
        /// id池
        /// </summary>
        public Queue<long> idPool = new Queue<long>();

        /// <summary>
        /// 当前递增的id值
        /// </summary>
        public long currentId = 0;

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            this.Destroy();
        }
    }

    public static class IdManagerRule
    {
        public static void Destroy(this IdManager self)
        {
            self.idPool.Clear();
            self.idPool = default;
            self.IsRecycle = true;
            self.IsDisposed = true;
        }

        /// <summary>
        /// 获取id后递增
        /// </summary>
        public static long GetId(this IdManager self)
        {
            if (!self.idPool.TryDequeue(out var value))
            {
                value = self.currentId++;
            }
            return value;
        }

        /// <summary>
        /// 回收id
        /// </summary>
        public static void RecycleId(this IdManager self, long id)
        {
            if (!self.idPool.Contains(id))
            {
                self.idPool.Enqueue(id);
            }
        }

    }
}
