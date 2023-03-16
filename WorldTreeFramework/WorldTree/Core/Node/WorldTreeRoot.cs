
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/14 20:30

* 描述： 世界树根
* 挂载核心启动后的管理器组件

*/

namespace WorldTree
{
    /// <summary>
    /// 世界树根
    /// </summary>
    public class WorldTreeRoot : Node, ComponentOf<WorldTreeCore>
    {
        public WorldTreeRoot()
        {
            Branch = this;
        }
    }
}
