/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 实体对象池
*
* 管理类型： Entity
*   
*   调用 ECS 生命周期系统， 生成， 获取， 回收， 销毁，
*   
*   同时对 实体 赋予 根节点 和 Id 分发。
*   
*/
using System;
using System.Collections.Generic;

namespace WorldTree
{

    class EntityPoolAddSystem : AddSystem<EntityPool>
    {
        public override void OnAdd(EntityPool self)
        {
            //生命周期系统
            self.newSystem = self.GetSystems<INewSystem>(self.ObjectType);
            self.getSystem = self.GetSystems<IGetSystem>(self.ObjectType);
            self.recycleSystem = self.GetSystems<IRecycleSystem>(self.ObjectType);
            self.destroySystem = self.GetSystems<IDestroySystem>(self.ObjectType);
        }
    }


    /// <summary>
    /// 实体对象池
    /// </summary>
    public class EntityPool : GenericPool<Entity>
    {
        public List<ISystem> newSystem;
        public List<ISystem> getSystem;
        public List<ISystem> recycleSystem;
        public List<ISystem> destroySystem;

        public Dictionary<long, Entity> Entitys;

        public EntityPool(Type type) : base()
        {

            ObjectType = type;

            Entitys = new Dictionary<long, Entity>();

            NewObject = ObjectNew;
            DestroyObject = ObjectDestroy;

            objectOnNew = ObjectOnNew;
            objectOnGet = ObjectOnGet;
            objectOnRecycle = ObjectOnRecycle;
            objectOnDestroy = ObjectOnDestroy;

        }

        /// <summary>
        /// 获取对象并转为指定类型
        /// </summary>
        public T Get<T>()
            where T : class
        {
            return Get() as T;
        }

        public override string ToString()
        {
            return $"[EntityPool<{ObjectType}>] : {Count} ";
        }

        private Entity ObjectNew(IPool pool)
        {
            Entity obj = Activator.CreateInstance(ObjectType, true) as Entity;
            obj.thisPool = this;
            obj.id = Root.IdManager.GetId();
            obj.Root = Root;
            return obj;
        }
        public override void Recycle(object obj) => Recycle(obj as Entity);
        public void Recycle(Entity obj)
        {
            lock (objetPool)
            {
                if (obj != null)
                {
                    if (maxLimit == -1 || objetPool.Count < maxLimit)
                    {
                        if (obj.IsRecycle) return;

                        objectOnRecycle.Invoke(obj);
                        objetPool.Enqueue(obj);
                    }
                    else
                    {
                        objectOnRecycle.Invoke(obj);
                        objectOnDestroy.Invoke(obj);
                        DestroyObject.Invoke(obj);
                    }
                }
            }
        }
        private void ObjectDestroy(Entity obj)
        {
            obj.Dispose();
        }

        private void ObjectOnNew(Entity obj)
        {
            newSystem?.Send(obj);
        }

        private void ObjectOnGet(Entity obj)
        {
            obj.IsRecycle = false;
            Entitys.TryAdd(obj.id, obj);
            getSystem?.Send(obj);
        }

        private void ObjectOnRecycle(Entity obj)
        {
            obj.IsRecycle = true;
            recycleSystem?.Send(obj);
            Entitys.Remove(obj.id);
        }

        private void ObjectOnDestroy(Entity obj)
        {
            obj.IsDisposed = true;
            destroySystem?.Send(obj);
        }

    }


    //=====================

    ////实体字典基类
    //public class EntityDictionary : Entity { public IDictionary value; } //箱基类
    ////实体字典泛型类
    //public class EntityDictionary<K, V> : EntityDictionary  //泛型箱
    //{
    //    public EntityDictionary()
    //    {
    //        Type = typeof(EntityDictionary); //将这个泛型类的 匹配标签 改为基类
    //        value = new Dictionary<K, V>(); //初始化赋值
    //    }
    //    public Dictionary<K, V> Value => value as Dictionary<K, V>; //强转获取

    //}
    ////回收系统匹配到基类
    //class EntityDictionaryRemoveystem : RemoveSystem<EntityDictionary>
    //{
    //    public override void OnRemove(EntityDictionary self)
    //    {
    //        self.value.Clear();//统一清理
    //    }
    //}










}
