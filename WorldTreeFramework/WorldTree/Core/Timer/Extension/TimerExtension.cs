
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/22 9:40

* 描述： 异步扩展

*/

namespace WorldTree
{
    public static class TimerExtension
    {
        /// <summary>
        /// 异步延迟帧
        /// </summary>
        public static AsyncTask AsyncYield(this Entity self, int count = 0)
        {
            AsyncTask asyncTask = self.AddChildren<AsyncTask>();
            var counter = asyncTask.AddComponent<CounterCall>();
            counter.countOut = count;
            counter.callback = asyncTask.SetResult;
            return asyncTask;
        }

        /// <summary>
        /// 异步延迟秒
        /// </summary>
        public static AsyncTask AsyncDelay(this Entity self, float time)
        {
            AsyncTask asyncTask = self.AddChildren<AsyncTask>();
            var timer = asyncTask.AddComponent<TimerCall>();
            timer.timeOutTime = time;
            timer.callback = asyncTask.SetResult;
            return asyncTask;
        }
    }
}
