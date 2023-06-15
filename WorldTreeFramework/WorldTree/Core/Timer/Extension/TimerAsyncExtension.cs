/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/22 9:40

* 描述： 异步扩展

*/

namespace WorldTree
{
    public static class TimerAsyncExtension
    {
        ///// <summary>
        ///// 异步延迟帧
        ///// </summary>
        //public static TreeTask AsyncYield(this INode self, int count = 0)
        //{
        //    self.AddChild(out TreeTask asyncTask).AddComponent(out CounterCall counter);
        //    counter.countOut = count;
        //    counter.callback ??= counter.AddChild(out counter.callback);
        //    counter.callback.Add(asyncTask, default(ITreeTaskSetResuItRule));

        //    return asyncTask;
        //}

        /// <summary>
        /// 异步延迟帧
        /// </summary>
        public static async TreeTask<bool> AsyncYield(this INode self, int count = 0)
        {
            //拿到令牌
            TreeTaskToken token = await self.TreeTaskTokenCatch();

            self.AddChild(out TreeTask asyncTask).AddComponent(out CounterCall counter, count);

            //组件的令牌事件
            token.tokenEvent.Add(counter, default(ITreeTaskTokenEventRule));

            //组件的任务完成回调注册
            counter.callback.Add(asyncTask, default(ITreeTaskSetResuItRule));

            await asyncTask;

            return token.State == TaskState.Running;
        }



        ///// <summary>
        ///// 异步延迟秒
        ///// </summary>
        //public static TreeTask AsyncDelay(this INode self, float time)
        //{
        //    self.AddChild(out TreeTask asyncTask).AddComponent(out TimerCall timer, time);
        //    timer.callback ??= timer.AddChild(out timer.callback);
        //    timer.callback.Add(asyncTask, default(ITreeTaskSetResuItRule));
        //    return asyncTask;
        //}


        /// <summary>
        /// 异步延迟秒
        /// </summary>
        public static async TreeTask<bool> AsyncDelay(this INode self, float time)
        {
            //拿到令牌
            TreeTaskToken token = await self.TreeTaskTokenCatch();

            self.AddChild(out TreeTask asyncTask).AddComponent(out TimerCall counter, time);

            //组件的令牌事件
            token.tokenEvent.Add(counter, default(ITreeTaskTokenEventRule));

            //组件的任务完成回调注册
            counter.callback.Add(asyncTask, default(ITreeTaskSetResuItRule));

            await asyncTask;

            return token.State == TaskState.Running;
        }
    }
}
