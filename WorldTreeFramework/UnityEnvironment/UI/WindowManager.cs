
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/19 10:24

* 描述： UI管理器

*/

using System;
using System.Collections;
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
        public UnitDictionary<Type, Entity> allWindows = new UnitDictionary<Type, Entity>();

        public UnitDictionary<Type, Entity> windows = new UnitDictionary<Type, Entity>();
        public UnitList<Entity> windowList = new UnitList<Entity>();

        //栈顶
        public Entity topPage;
        //栈底
        public Entity rootPage;

        public GameObjectEntity gameObject;


        private void PushWindow(Entity entity)
        {
            if (!windows.ContainsKey(entity.GetType()))
            {
                windows.Add(entity.GetType(), entity);
                windowList.Add(entity);

                topPage?.SendSystem<IWindowLostFocusSystem>();
                topPage = entity;//栈顶切换
                topPage?.SendSystem<IWindowFocusSystem>();
            }
        }

        private Entity PopWindow()
        {
            Entity entity = null;
            if (windowList.Count != 0)
            {
                if (topPage != null)
                {
                    topPage?.SendSystem<IWindowLostFocusSystem>();
                    windowList.Remove(topPage);
                    windows.Remove(topPage.GetType());
                    entity = topPage;
                    topPage = windowList[windowList.Count - 1];//栈顶切换
                    topPage?.SendSystem<IWindowFocusSystem>();
                }
            }
            return entity;
        }


        /// <summary>
        /// 打开窗口
        /// </summary>
        public T Show<T>()
            where T : Entity
        {
            if (windows.TryGetValue(typeof(T), out Entity entity))
            {
                while (entity.id != windowList[windowList.Count - 1].id)
                {
                    PopWindow().Dispose();
                }
            }
            else
            {
                entity = AddComponent<T>();
                PushWindow(entity);
            }
            return entity as T;
        }

        ///// <summary>
        ///// 关闭窗口
        ///// </summary>
        //public void Close<T>()
        //   where T : Entity
        //{
        //    if (windows.TryGetValue(typeof(T), out Entity entity))
        //    {
        //        while (entity.id != windowList[windowList.Count - 1].id)
        //        {
        //            PopWindow();
        //        }
        //    }
        //}

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void CloseTop()
        {
            PopWindow();
        }
        /// <summary>
        /// 关闭全部
        /// </summary>
        public void CloseAll()
        {
            while (windowList.Count != 0)
            {
                PopWindow();
            }
        }
    }


    class WindowManagerAddSystem : AddSystem<WindowManager>
    {
        public override async void OnAdd(WindowManager self)
        {
            World.Log("WindowManager启动!!!");
            self.gameObject = self.AddComponent<GameObjectEntity>().Instantiate<WindowManager>();

            //预制体测试
            await self.AddGameObjectEntity<MainWindow>(self.gameObject);
        }
    }

    class WindowManagerUpdateSystem : UpdateSystem<WindowManager>
    {
        public override void Update(WindowManager self, float deltaTime)
        {
            //self.topPage?.SendSystem<IWindowFocusUpdateSystem, float>(deltaTime);
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