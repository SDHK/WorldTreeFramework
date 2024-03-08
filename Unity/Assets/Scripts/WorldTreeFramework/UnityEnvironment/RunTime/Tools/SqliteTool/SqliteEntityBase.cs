
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 10:56

* 描述： 

*/

namespace Logic.SqliteData
{
    /// <summary>
    /// 数据库实体基类
    /// </summary>
    public abstract class SqliteEntityBase<T>
    {
        /// <summary>
        /// 编号
        /// </summary>
        public T Id;
    }
}
