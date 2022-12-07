
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/19 10:24

* 描述： UI管理器

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{

    class InitialDomain1 : AddSystem<InitialDomain>
    {
        public override void OnAdd(InitialDomain self)
        {
            self.Root.AddComponent<WindowManager>();
        }
    }


    //动画？
    //焦点进入，焦点离开，焦点Update
    //UI Update

    //Update系统，思考时间参数的必要

    //思考Manager的全局广播和监听功能
    public class WindowManager : Entity
    {
        //public UnitDictionary<Type, Entity> windows = new UnitDictionary<Type, Entity>();

        public UnitDictionary<Type, Entity> windows = new UnitDictionary<Type, Entity>();
        public Stack<Entity> windowStack = new Stack<Entity>();

        public GameObjectEntity gameObject;

        private void PushWindow(Entity entity)
        {

            if (!windows.ContainsKey(entity.Type))
            {
                windows.Add(entity.Type, entity);

                if (windowStack.Count != 0) windowStack.Peek()?.SendSystem<IWindowLostFocusSystem>();

                windowStack.Push(entity);

                entity?.SendSystem<IWindowFocusSystem>();
            }
        }

        private Entity PopWindow()
        {
            Entity entity = null;
            if (windowStack.Count != 0)
            {
                entity = windowStack.Pop();
                windows.Remove(entity.Type);
                entity?.SendSystem<IWindowLostFocusSystem>();
                if (windowStack.Count != 0) windowStack.Peek()?.SendSystem<IWindowFocusSystem>();
            }
            return entity;
        }


        /// <summary>
        /// 打开窗口
        /// </summary>
        public async AsyncTask<T> Show<T>()
            where T : Entity
        {
            if (windows.TryGetValue(typeof(T), out Entity entity))
            {
                while (windowStack.Count != 0 ? windowStack.Peek().id != entity.id : false)
                {
                    PopWindow().ParentTo<GameObjectEntity>()?.Dispose();
                }
                await this.AsyncYield();
            }
            else
            {
                entity = await this.AddGameObjectEntity<T>(gameObject);
                PushWindow(entity);
            }
            return entity as T;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void Dispose<T>()
           where T : Entity
        {
            if (windows.TryGetValue(typeof(T), out Entity entity))
            {
                while (windowStack.Count != 0 ? windowStack.Peek().id != entity.id : false)
                {
                    PopWindow().ParentTo<GameObjectEntity>()?.Dispose();
                }
                PopWindow().ParentTo<GameObjectEntity>()?.Dispose();
            }
        }

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void CloseTop()
        {
            PopWindow().ParentTo<GameObjectEntity>()?.Dispose();
        }
        /// <summary>
        /// 关闭全部
        /// </summary>
        public void CloseAll()
        {
            while (windowStack.Count != 0)
            {
                PopWindow().ParentTo<GameObjectEntity>()?.Dispose();
            }
        }
    }


    class WindowManagerAddSystem : AddSystem<WindowManager>
    {
        public override void OnAdd(WindowManager self)
        {
            World.Log("WindowManager启动!!!");
            self.gameObject = self.AddComponent<GameObjectEntity>().Instantiate<WindowManager>();
        }
    }

    class WindowManagerUpdateSystem : UpdateSystem<WindowManager>
    {
        public override void Update(WindowManager self, float deltaTime)
        {

        }
    }

    class WindowManagerEntityAddSystem : EntityAddSystem<WindowManager>
    {
        public override void OnEntityAdd(WindowManager self, Entity entity)
        {

        }
    }
    class WindowManagerEntityRemoveSystem : EntityRemoveSystem<WindowManager>
    {
        public override void OnEntityRemove(WindowManager self, Entity entity)
        {

        }
    }


}