
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
        public static AsyncTask AsyncYield(this Entity self, int count = 0)
        {
            var counter = self.AddChildren<CounterCall>();
            AsyncTask asyncTask = counter.AddChildren<AsyncTask>();
            counter.countOut = count;
            counter.callback = asyncTask.SetResult;
            return asyncTask;
        }


        /// <summary>
        /// 异步延迟秒
        /// </summary>
        public static AsyncTask AsyncDelay(this Entity self, float time)
        {
            var timer = self.AddChildren<TimerCall>();
            AsyncTask asyncTask = timer.AddChildren<AsyncTask>();
            timer.timeOutTime = time;
            timer.callback = asyncTask.SetResult;
            return asyncTask;
        }
    }
}
