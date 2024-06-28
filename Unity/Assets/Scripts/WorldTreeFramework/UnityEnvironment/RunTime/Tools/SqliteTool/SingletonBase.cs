/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/7 9:23
* 描    述:

****************************************/


using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree.Sample
{
	/// <summary>
	/// 单例基类
	/// </summary>
	public abstract class SingletonBase<T>:IDisposable
    where T : SingletonBase<T>, new()
    {
		/// <summary>
		/// 实例
		/// </summary>
		private static T instance;
		/// <summary>
		/// 对象锁
		/// </summary>
		private static readonly object lockObj = new object();//对象锁

        /// <summary>
        /// 单例实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                            Debug.Log("[单例启动] : " + typeof(T).Name);
                            instance.OnInstance();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static T InstanceGet()
        {
            return Instance;
        }

		/// <summary>
		/// 实例化时调用
		/// </summary>
		public virtual void OnInstance() { }

        public abstract void Dispose();
    }
}