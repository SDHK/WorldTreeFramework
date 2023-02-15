using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorldTree
{
    /// <summary>
    /// 电机
    /// </summary>
    public class Motor : Entity
    {
        /// <summary>
        /// 获取
        /// </summary>
        public Func<float> GetValue;
        /// <summary>
        /// 设置
        /// </summary>
        public Action<float> SetValue;
        /// <summary>
        /// 条件
        /// </summary>
        public Func<Motor, bool> Condition;
        /// <summary>
        /// 完成
        /// </summary>
        public Action OnComplete;

        /// <summary>
        /// 设置
        /// </summary>
        public void Set(Func<float> get, Action<float> set, Func<Motor, bool> condition)
        {
            GetValue = get;
            SetValue = set;
            Condition = condition;
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Update()
        {
            if (Condition(this))
            {
                OnComplete();
            }
        }
    }



    public static class MoveTool
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
