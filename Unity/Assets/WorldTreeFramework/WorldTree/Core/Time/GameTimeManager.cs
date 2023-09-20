/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/28 19:56

* 描述： 游戏时间管理器，通过累加增量时间计时，可暂停

*/

namespace WorldTree
{
    /// <summary>
    /// 游戏时间管理器
    /// </summary>
    public class GameTimeManager : Node, ComponentOf<WorldTreeCore>
        , AsRule<IAwakeRule>
    {

        /// <summary>
        /// 帧时间
        /// </summary>
        public float FrameTime;

    }
}
