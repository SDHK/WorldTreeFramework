using System;

namespace WorldTree
{

    /// <summary>
    /// 值渐变
    /// </summary>
    public class TreeTween : Node,IAwake,ComponentOf<TreeValueBase>
    {
        /// <summary>
        /// 开始
        /// </summary>
        public TreeValueBase<float> start;

        /// <summary>
        /// 结束
        /// </summary>
        public TreeValueBase<float> end;

        /// <summary>
        /// 值
        /// </summary>
        public TreeValueBase<float> value;

    }


    public static class TreeValueBaseRule
    {
        public static void Lerp(this TreeValueBase<float> self, float target, float timeScale)
        {
            self.AddComponent(out TreeTween treeTween);
            
            treeTween.start.Value = self.Value;
            treeTween.end.Value = target;
            treeTween.value = self;

        }

    }

    public static class TreeTweenRule
    {
        public static void Tween(this TreeTween self, float target, float timeScale)
        {

        }

    }



    public static class TweenTool
    {
        /// <summary>
        /// 移动到目标
        /// </summary>
        /// <param name="current">当前</param>
        /// <param name="target">目标</param>
        /// <param name="timeScale">距离比例</param>
        public static float Lerp(float current, float target, float timeScale)
        {
            return current + (target - current) * Math.Clamp(timeScale, 0, 1);
        }
        /// <summary>
        /// 移动到目标
        /// </summary>
        /// <param name="current">当前</param>
        /// <param name="target">目标</param>
        /// <param name="maxDelta">每次移动间隔</param>
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
            {
                return target;
            }
            return current + Math.Sign(target - current) * maxDelta;
        }
        /// <summary>
        /// 定向移动
        /// </summary>
        /// <param name="current">当前</param>
        /// <param name="distance">移动方向</param>
        /// <param name="speed">速度</param>
        public static float Move(float current, float distance, float speed)
        {
            return current += distance * speed;
        }




    }




}
