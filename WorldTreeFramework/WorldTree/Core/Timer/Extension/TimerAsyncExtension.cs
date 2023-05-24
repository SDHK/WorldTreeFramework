
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/22 9:40

* 描述： 异步扩展

*/


namespace WorldTree
{
    public static class TimerAsyncExtension
    {
        /// <summary>
        /// 异步延迟帧
        /// </summary>
        public static TreeTask AsyncYield(this INode self, int count = 0)
        {
            self.AddChild(out TreeTask asyncTask).AddComponent(out CounterCall counter);
            counter.countOut = count;
            counter.callback.Add(asyncTask, out TreeTask.SetResultRuleNode _);
            return asyncTask;
        }


        /// <summary>
        /// 异步延迟秒
        /// </summary>
        public static TreeTask AsyncDelay(this INode self, float time)
        {
            self.AddChild(out TreeTask asyncTask).AddComponent(out TimerCall timer);
            timer.timeOutTime = time;
            timer.callback.Add(asyncTask, out TreeTask.SetResultRuleNode _);
            return asyncTask;
        }
    }
}
