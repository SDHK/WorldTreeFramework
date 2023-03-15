
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
            TreeTask asyncTask = self.AddChildren<TreeTask>();
            var counter = asyncTask.AddComponent(out CounterCall _);

            counter.countOut = count;
            counter.callback = asyncTask.SetResult;
            return asyncTask;
        }


        /// <summary>
        /// 异步延迟秒
        /// </summary>
        public static TreeTask AsyncDelay(this INode self, float time)
        {
            TreeTask asyncTask = self.AddChildren<TreeTask>();
            var timer = asyncTask.AddComponent(out TimerCall _);
            timer.timeOutTime = time;
            timer.callback = asyncTask.SetResult;
            return asyncTask;
        }
    }
}
