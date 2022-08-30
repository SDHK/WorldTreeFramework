/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 一个编号分发的管理器

*/

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
        /// 当前递增的id值
        /// </summary>
        public long Id = 0;

        /// <summary>
        /// 获取id后递增
        /// </summary>
        public long GetId()
        {
            return Id++;
        }

    }
}
