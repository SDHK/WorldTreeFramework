
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
        public GameObjectNode gameObject;

        /// <summary>
        /// 打开窗口入栈
        /// </summary>
        public async TreeTask<T> Show<T>()
            where T : Node
        {
            if (windows.TryGetValue(typeof(T), out Node node))
            {
                if (windowStack.TryPeek(out Node outNode))
                {
                    outNode?.SendRule<IWindowLostFocusRule>();
                }
                while (windowStack.Count != 0 ? windowStack.Peek().id != node.id : false)
                {
                    outNode = windowStack.Pop();
                    windows.Remove(outNode.Type);
                    outNode.ParentTo<GameObjectNode>()?.Dispose();
                }
                node?.SendRule<IWindowFocusRule>();

                await this.TreeTaskCompleted();
            }
            else
            {
                node = await this.AddGameObjectNode<T>(gameObject);

                windows.Add(node.Type, node);

                if (windowStack.TryPeek(out Node topNode))
                {
                    topNode?.SendRule<IWindowLostFocusRule>();
                }
                windowStack.Push(node);

                node?.SendRule<IWindowFocusRule>();
            }
            return node as T;
        }


        /// <summary>
        /// 关闭栈窗口
        /// </summary>
        public void Dispose<T>()
           where T : Node
        {
            if (windows.TryGetValue(typeof(T), out Node targetNode))
            {
                if (windowStack.TryPeek(out Node topNode))
                {
                    topNode?.SendRule<IWindowLostFocusRule>();
                }
                while (windowStack.TryPop(out topNode))
                {
                    if (topNode.id == targetNode.id)
                    {
                        windows.Remove(topNode.Type);
                        topNode.ParentTo<GameObjectNode>()?.Dispose();
                        break;
                    }
                }
                if (windowStack.TryPeek(out topNode))
                {
                    topNode?.SendRule<IWindowFocusRule>();
                }
            }
        }

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void DisposeTop()
        {
            if (windowStack.TryPop(out Node outNode))
            {
                windows.Remove(outNode.Type);
                outNode?.SendRule<IWindowLostFocusRule>();
                if (windowStack.TryPeek(out Node topNode))
                {
                    topNode?.SendRule<IWindowFocusRule>();
                }
                outNode.ParentTo<GameObjectNode>()?.Dispose();
            }
        }
        /// <summary>
        /// 关闭全部
        /// </summary>
        public void DisposeAll()
        {
            if (windowStack.TryPeek(out Node topNode))
            {
                topNode?.SendRule<IWindowFocusRule>();
            }
            while (windowStack.TryPop(out topNode))
            {
                topNode.ParentTo<GameObjectNode>()?.Dispose();
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


    class WindowManagerAddRule : AddRule<WindowManager>
    {
        public override void OnEvent(WindowManager self)
        {
            World.Log("WindowManager启动!!!");
            self.gameObject = self.AddComponent<GameObjectNode>().Instantiate<WindowManager>();
        }
    }

    class WindowManagerUpdateRule : UpdateRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, float deltaTime)
        {
            if (self.windowStack.Count != 0)
            {
                if (self.windowStack.TryPeek(out Node node))
                {
                    node?.SendRule<IWindowFocusUpdateRule, float>(deltaTime);
                }
            }
        }
    }

    class WindowManagerNodeAddRule : ListenerAddRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, Node node)
        {

        }
    }
    class WindowManagerNodeRemoveRule : ListenerRemoveRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, Node node)
        {

        }
    }


}