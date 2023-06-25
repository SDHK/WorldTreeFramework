/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/22 9:40

* 描述： 异步扩展

*/

using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace WorldTree
{
    public static class TimerAsyncExtension
    {
        /// <summary>
        /// 异步延迟帧
        /// </summary>
        public static async TreeTask<bool> AsyncYield(this INode self, int count = 0)
        {
            //拿到令牌
            TreeTaskToken token = await self.TreeTaskTokenCatch();

            self.AddChild(out TreeTask asyncTask).AddComponent(out CounterCall counter, count);

            if (token != null)
            {
                //组件的令牌事件
                token.tokenEvent.Add(counter, default(ITreeTaskTokenEventRule));
                await asyncTask;
                return token.State == TaskState.Cancel;
            }
            else
            {
                await asyncTask;
                return false;
            }
        }


        /// <summary>
        /// 异步延迟秒
        /// </summary>
        public static async TreeTask<bool> AsyncDelay(this INode self, float time)
        {

            //拿到令牌
            TreeTaskToken token = await self.TreeTaskTokenCatch();

            self.AddChild(out TreeTask asyncTask).AddComponent(out TimerCall counter, time);
            //组件的任务完成回调注册
            counter.callback.Add(asyncTask, default(ITreeTaskSetResuItRule));

            if (token != null)
            {
                //组件的令牌事件
                token.tokenEvent.Add(counter, default(ITreeTaskTokenEventRule));
                await asyncTask;
                return token.State == TaskState.Cancel;
            }
            else
            {
                await asyncTask;
                return false;
            }
        }
    }
}
