
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/19 10:24

* 描述： UI管理器

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{

    //class InitialDomain1 : AddSystem<InitialDomain>
    //{
    //    public override void OnAdd(InitialDomain self)
    //    {
    //        self.Root.AddComponent<WindowManager>();
    //    }
    //}


    public class WindowManager : Entity
    {
        public UnitDictionary<Type, Entity> windows = new UnitDictionary<Type, Entity>();
        public Stack<Entity> windowStack = new Stack<Entity>();

        public GameObjectEntity gameObject;

        /// <summary>
        /// 打开窗口
        /// </summary>
        public async AsyncTask<T> Show<T>()
            where T : Entity
        {
            if (windows.TryGetValue(typeof(T), out Entity entity))
            {
                if (windowStack.TryPeek(out Entity outEntity))
                {
                    outEntity?.SendSystem<IWindowLostFocusSystem>();
                }
                while (windowStack.Count != 0 ? windowStack.Peek().id != entity.id : false)
                {
                    outEntity = windowStack.Pop();
                    windows.Remove(outEntity.Type);
                    outEntity.ParentTo<GameObjectEntity>()?.Dispose();
                }
                entity?.SendSystem<IWindowFocusSystem>();

                await this.AsyncYield();
            }
            else
            {
                entity = await this.AddGameObjectEntity<T>(gameObject);

                windows.Add(entity.Type, entity);

                if (windowStack.TryPeek(out Entity topEntity))
                {
                    topEntity?.SendSystem<IWindowLostFocusSystem>();
                }
                windowStack.Push(entity);

                entity?.SendSystem<IWindowFocusSystem>();
            }
            return entity as T;
        }


        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void Dispose<T>()
           where T : Entity
        {
            if (windows.TryGetValue(typeof(T), out Entity targetEntity))
            {
                if (windowStack.TryPeek(out Entity topEntity))
                {
                    topEntity?.SendSystem<IWindowLostFocusSystem>();
                }
                while (windowStack.TryPop(out topEntity))
                {
                    if (topEntity.id == targetEntity.id)
                    {
                        windows.Remove(topEntity.Type);
                        topEntity.ParentTo<GameObjectEntity>()?.Dispose();
                        break;
                    }
                }
                if (windowStack.TryPeek(out topEntity))
                {
                    topEntity?.SendSystem<IWindowFocusSystem>();
                }
            }
        }

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void DisposeTop()
        {
            if (windowStack.TryPop(out Entity outEntity))
            {
                windows.Remove(outEntity.Type);
                outEntity?.SendSystem<IWindowLostFocusSystem>();
                if (windowStack.TryPeek(out Entity topEntity))
                {
                    topEntity?.SendSystem<IWindowFocusSystem>();
                }
                outEntity.ParentTo<GameObjectEntity>()?.Dispose();
            }
        }
        /// <summary>
        /// 关闭全部
        /// </summary>
        public void DisposeAll()
        {
            if (windowStack.TryPeek(out Entity topEntity))
            {
                topEntity?.SendSystem<IWindowFocusSystem>();
            }
            while (windowStack.TryPop(out topEntity))
            {
                topEntity.ParentTo<GameObjectEntity>()?.Dispose();
            }
        }


        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void Close<T>()
           where T : Entity
        {

        }

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void CloseTop()
        {
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
            if (self.windowStack.Count != 0)
            {
                if (self.windowStack.TryPeek(out Entity entity))
                {
                    entity?.SendSystem<IWindowFocusUpdateSystem, float>(deltaTime);
                }
            }
        }
    }

    class WindowManagerEntityAddSystem : ListenerAddSystem<WindowManager>
    {
        public override void OnAdd(WindowManager self, Entity entity)
        {

        }
    }
    class WindowManagerEntityRemoveSystem : ListenerRemoveSystem<WindowManager>
    {
        public override void OnRemove(WindowManager self, Entity entity)
        {

        }
    }


}