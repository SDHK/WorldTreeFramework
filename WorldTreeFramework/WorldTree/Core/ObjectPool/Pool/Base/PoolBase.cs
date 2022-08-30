/****************************************

 * 作者：闪电黑客
 * 日期: 2021/12/13 13:35:32 （第5次重写）
 
 * 描述:  
 * 对原先对象池工具的重写，
 * 增加计时销毁设定，并抽象大部分方法
 * 所有对象池的最基类

*/
/****************************************

 * 作者： 闪电黑客
 * 日期： 2022/5/17 10:34 （第6次大修改）

 * 描述:  
 * 改为继承Unit统一了销毁功能
 * 重命名类由 ObjectPoolBase 改为 PoolBase
 * 分离计时功能
*/
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/15 9:32

* 描述： 
* 为了扩展性再抽出一层IPool接口作为基础

*/

/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/5 10:32

* 描述： 对象池抽象基类 （第7次大修改）
* 
* 从oop改为ecs 并入Entity树 

*/


using System;

namespace WorldTree
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    public interface IPool:IUnit
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// 当前保留对象数量
        /// </summary>
        public int Count { get; }
        
        /// <summary>
        /// 获取对象
        /// </summary>
        public object GetObject();

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle(object obj);

        /// <summary>
        /// 释放全部对象
        /// </summary>
        public void DisposeAll();

        /// <summary>
        /// 释放一个对象
        /// </summary>
        public void DisposeOne();

        /// <summary>
        /// 预加载
        /// </summary>
        public void Preload();

    }



    /// <summary>
    /// 对象池抽象基类
    /// </summary>
    public abstract class PoolBase : Entity, IPool
    {

        public Type ObjectType { get; set; }

        public abstract int Count { get; }

        /// <summary>
        /// 预加载数量
        /// </summary>
        public int minLimit = 0;

        /// <summary>
        /// 对象回收数量限制
        /// </summary>
        public int maxLimit = -1;


        public abstract object GetObject();

        public abstract void Recycle(object obj);

        public abstract void DisposeAll();

        public abstract void DisposeOne();

        public abstract void Preload();


    }

}