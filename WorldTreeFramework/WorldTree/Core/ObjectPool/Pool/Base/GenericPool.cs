
/******************************

 * 作者： 闪电黑客
 * 日期: 2021/12/13 13:37:00

 * 描述:  
 
    泛型通用对象池 ,继承 PoolBase
    
    需要手动注册New和Destroy的方法

    1.预加载数：objectPreload = 0 ，在 池新建时 和 Get时 , 将池内对象数量保持到设定数值。
    
    2.限制数：objectLimit = -1 ，为-1时则不限制对象数量。
        数量超过限制时，对象将不再被回收保留，而是被回收后立即销毁。

    设计思路是可以用于任意类型的对象管理，
    也可以被继承定义为指定对象管理的对象池。

*/
/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/23 10:30

* 描述： 名称改为通用对象池，
* 去除了倒计时功能。

*/



using System;
using System.Collections.Generic;

namespace WorldTree
{

    /// <summary>
    /// 泛型通用对象池
    /// </summary>
    public class GenericPool<T> : PoolBase
        where T : class
    {
        /// <summary>
        /// 对象池
        /// </summary>
        private Queue<T> objetPool = new Queue<T>();

        /// <summary>
        /// 当前保留对象数量
        /// </summary>
        public override int Count => objetPool.Count;

        /// <summary>
        /// 实例化对象的方法
        /// </summary>
        public Func<PoolBase, T> NewObject;
        /// <summary>
        /// 销毁对象的方法
        /// </summary>
        public Action<T> DestroyObject;


        /// <summary>
        /// 对象新建时
        /// </summary>
        public Action<T> objectOnNew;

        /// <summary>
        /// 对象获取时
        /// </summary>
        public Action<T> objectOnGet;

        /// <summary>
        /// 对象回收时
        /// </summary>
        public Action<T> objectOnRecycle;

        /// <summary>
        /// 对象销毁时
        /// </summary>
        public Action<T> objectOnDestroy;


        //用于继承的子类可实现无参数构造函数
        protected GenericPool() { ObjectType = typeof(T); }

        /// <summary>
        /// 对象池构造 （实例化对象的委托，销毁对象的委托）
        /// </summary>
        public GenericPool(Func<PoolBase, T> objectNew, Action<T> objectDestroy = null)
        {
            ObjectType = typeof(T);
            this.NewObject = objectNew;
            this.DestroyObject = objectDestroy;
        }

        public override string ToString()
        {
            return "[GenericPool<" + ObjectType + ">]";
        }

        /// <summary>
        /// 从队列获取一个对象，假如队列无对象则新建对象
        /// </summary>
        public T DequeueOrNewObject()
        {
            lock (objetPool)
            {
                T obj = null;

                while (objetPool.Count > 0 && obj== null)
                {
                    obj = objetPool.Dequeue();
                }

                if (obj == null)
                {
                    if (NewObject != null)
                    {
                        obj = NewObject(this);
                    }
                    objectOnNew?.Invoke(obj);
                }

                return obj;
            }
        }


        /// <summary>
        /// 获取对象
        /// </summary>
        public virtual T Get()
        {
            T obj = DequeueOrNewObject();
            objectOnGet?.Invoke(obj);
            Preload();
            return obj;
        }


        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle(T obj)
        {
            lock (objetPool)
            {
                if (obj != null)
                {

                    if (maxLimit == -1 || objetPool.Count < maxLimit)
                    {
                        if (!objetPool.Contains(obj))
                        {
                            objectOnRecycle?.Invoke(obj);
                            objetPool.Enqueue(obj);
                        }
                    }
                    else
                    {
                        objectOnRecycle?.Invoke(obj);
                        objectOnDestroy?.Invoke(obj);
                        DestroyObject?.Invoke(obj);
                    }
                }
            }
        }
        public override object GetObject()
        {
            return Get();
        }

        public override void Recycle(object obj)
        {
            Recycle((T)obj);
        }
        public override void DisposeOne()
        {
            lock (objetPool)
            {
                if (objetPool.Count > 0)
                {
                    var obj = objetPool.Dequeue();
                    objectOnDestroy?.Invoke(obj);
                    DestroyObject?.Invoke(obj);
                }
            }
        }
        public override void DisposeAll()
        {
            lock (objetPool)
            {
                while (objetPool.Count > 0)
                {
                    var obj = objetPool.Dequeue();
                    objectOnDestroy?.Invoke(obj);
                    DestroyObject?.Invoke(obj);
                }
            }
        }

        public override void Preload()
        {
            lock (objetPool)
            {
                while (objetPool.Count < minLimit)
                {
                    T obj = NewObject(this);
                    objectOnNew?.Invoke(obj);
                    objetPool.Enqueue(obj);
                }
            }
        }



        public override void OnDispose()
        {
            DisposeAll();
            NewObject = null;
            DestroyObject = null;
            objectOnNew = null;
            objectOnGet = null;
            objectOnRecycle = null;
            objectOnDestroy = null;
        }


    }
}