
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/19 10:24

* 描述： UI 窗口栈管理器
* 
* 缺少非栈窗口的管理方法

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{

    //class InitialDomain1 : AddRule<InitialDomain>
    //{
    //    public override void OnEvent(InitialDomain self)
    //    {
    //        self.Root.AddComponent<WindowManager>();
    //    }
    //}

    /// <summary>
    /// UI 栈窗口管理器
    /// </summary>
    public class WindowManager : Node
    {
        /// <summary>
        /// 全部窗口
        /// </summary>
        public UnitDictionary<Type, Node> windows = new UnitDictionary<Type, Node>();
        /// <summary>
        /// 栈窗口
        /// </summary>
        public Stack<Node> windowStack = new Stack<Node>();

        /// <summary>
        /// 根节点
        /// </summary>
        public GameObjectEntity gameObject;

        /// <summary>
        /// 打开窗口入栈
        /// </summary>
        public async TreeTask<T> Show<T>()
            where T : Node
        {
            if (windows.TryGetValue(typeof(T), out Node entity))
            {
                if (windowStack.TryPeek(out Node outEntity))
                {
                    outEntity?.SendRule<IWindowLostFocusSystem>();
                }
                while (windowStack.Count != 0 ? windowStack.Peek().id != entity.id : false)
                {
                    outEntity = windowStack.Pop();
                    windows.Remove(outEntity.Type);
                    outEntity.ParentTo<GameObjectEntity>()?.Dispose();
                }
                entity?.SendRule<IWindowFocusSystem>();

                await this.TreeTaskCompleted();
            }
            else
            {
                entity = await this.AddGameObjectEntity<T>(gameObject);

                windows.Add(entity.Type, entity);

                if (windowStack.TryPeek(out Node topEntity))
                {
                    topEntity?.SendRule<IWindowLostFocusSystem>();
                }
                windowStack.Push(entity);

                entity?.SendRule<IWindowFocusSystem>();
            }
            return entity as T;
        }


        /// <summary>
        /// 关闭栈窗口
        /// </summary>
        public void Dispose<T>()
           where T : Node
        {
            if (windows.TryGetValue(typeof(T), out Node targetEntity))
            {
                if (windowStack.TryPeek(out Node topEntity))
                {
                    topEntity?.SendRule<IWindowLostFocusSystem>();
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
                    topEntity?.SendRule<IWindowFocusSystem>();
                }
            }
        }

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void DisposeTop()
        {
            if (windowStack.TryPop(out Node outEntity))
            {
                windows.Remove(outEntity.Type);
                outEntity?.SendRule<IWindowLostFocusSystem>();
                if (windowStack.TryPeek(out Node topEntity))
                {
                    topEntity?.SendRule<IWindowFocusSystem>();
                }
                outEntity.ParentTo<GameObjectEntity>()?.Dispose();
            }
        }
        /// <summary>
        /// 关闭全部
        /// </summary>
        public void DisposeAll()
        {
            if (windowStack.TryPeek(out Node topEntity))
            {
                topEntity?.SendRule<IWindowFocusSystem>();
            }
            while (windowStack.TryPop(out topEntity))
            {
                topEntity.ParentTo<GameObjectEntity>()?.Dispose();
            }
        }


        /// <summary>
        /// 关闭动画栈窗口
        /// </summary>
        public void Close<T>()
           where T : Node
        {

        }

        /// <summary>
        /// 关闭动画栈顶窗口
        /// </summary>
        public void CloseTop()
        {
        }
    }


    class WindowManagerAddSystem : AddRule<WindowManager>
    {
        public override void OnEvent(WindowManager self)
        {
            World.Log("WindowManager启动!!!");
            self.gameObject = self.AddComponent<GameObjectEntity>().Instantiate<WindowManager>();
        }
    }

    class WindowManagerUpdateSystem : UpdateRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, float deltaTime)
        {
            if (self.windowStack.Count != 0)
            {
                if (self.windowStack.TryPeek(out Node entity))
                {
                    entity?.SendRule<IWindowFocusUpdateSystem, float>(deltaTime);
                }
            }
        }
    }

    class WindowManagerEntityAddSystem : ListenerAddRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, Node entity)
        {

        }
    }
    class WindowManagerEntityRemoveSystem : ListenerRemoveRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, Node entity)
        {

        }
    }


}