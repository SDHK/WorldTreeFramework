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
        public static async TreeTask AsyncYield(this INode self, int count = 0)
        {
            self.AddChild(out TreeTask asyncTask).AddComponent(out CounterCall counter, count);

            //令牌是否为空,不为空则将组件挂入令牌
            (await self.TreeTaskTokenCatch())?.tokenEvent.Add(counter, default(TreeTaskTokenEvent));

            //组件的任务完成回调注册
            counter.callback.Add(asyncTask, default(TreeTaskSetResuIt));

            //等待异步执行
            await asyncTask;
        }


        /// <summary>
        /// 异步延迟秒
        /// </summary>
        public static async TreeTask AsyncDelay(this INode self, float time)
        {
            self.AddChild(out TreeTask asyncTask).AddComponent(out TimerCall counter, time);

            //令牌是否为空,不为空则将组件挂入令牌
            (await self.TreeTaskTokenCatch())?.tokenEvent.Add(counter, default(TreeTaskTokenEvent));
            //组件的任务完成回调注册
            counter.callback.Add(asyncTask, default(TreeTaskSetResuIt));

            //等待异步执行
            await asyncTask;
        }
    }
}
