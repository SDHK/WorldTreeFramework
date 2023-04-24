using System;

namespace WorldTree
{

    /// <summary>
    /// 值渐变
    /// </summary>
    public class TreeTween<T1,T2> : Node,IAwake,ComponentOf<TreeValueBase>
        where T1 :  IEquatable<T1>
        where T2 :  IEquatable<T2>
    {
        /// <summary>
        /// 值
        /// </summary>
        public TreeValueBase<T1> changeValue;

        /// <summary>
        /// 开始
        /// </summary>
        public TreeValueBase<T2> startValue;

        /// <summary>
        /// 结束
        /// </summary>
        public TreeValueBase<T2> endValue;

     


        //执行方式

        //曲线？

        //下一个Tween?
    }


    public static class TreeValueBaseRule
    {
        public static void Lerp<T>(this TreeValueBase<T> self, T target, T timeScale)
        where T : struct, IEquatable<T>
        {
            //self.AddComponent(out TreeTween<T> treeTween);
            
            //treeTween.startValue.Value = self.Value;
            //treeTween.endValue.Value = target;
            //treeTween.changeValue = self;

        }

    }

    public static class TreeTweenRule
    {
        //public static void Tween<T>(this TreeTween<T> self, float target, float timeScale)
        //where T : struct, IEquatable<T>
        //{
        //}

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
