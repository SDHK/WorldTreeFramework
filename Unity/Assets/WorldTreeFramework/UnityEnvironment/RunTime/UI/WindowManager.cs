
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

    //class InitialDomain1 : AddNodeRule<InitialDomain>
    //{
    //    public override void OnEvent(InitialDomain self)
    //    {
    //        self.Core.AddComponent<WindowManager>();
    //    }
    //}

    /// <summary>
    /// UI 栈窗口管理器
    /// </summary>
    public class WindowManager : NodeListener
        , AsRule<IAwakeRule>
    {
        /// <summary>
        /// 全部窗口
        /// </summary>
        public UnitDictionary<Type, INode> windows = new UnitDictionary<Type, INode>();
        /// <summary>
        /// 栈窗口
        /// </summary>
        public Stack<INode> windowStack = new Stack<INode>();

        /// <summary>
        /// 根节点
        /// </summary>
        public GameObjectNode gameObject;

        /// <summary>
        /// 打开窗口入栈
        /// </summary>
        public async TreeTask<T> Show<T>()
            where T : class, INode, ComponentOf<INode>, AsRule<IAwakeRule>

        {
            if (windows.TryGetValue(typeof(T), out INode node))
            {
                if (windowStack.TryPeek(out INode outNode))
                {
                    outNode?.TrySendRule(default(IWindowLostFocusRule));
                }
                while (windowStack.Count != 0 ? windowStack.Peek().Id != node.Id : false)
                {
                    outNode = windowStack.Pop();
                    windows.Remove(outNode.Type);
                    outNode.ParentTo<GameObjectNode>()?.Dispose();
                }
                node?.TrySendRule(default(IWindowFocusRule));

                await this.TreeTaskCompleted();
            }
            else
            {
                node = await this.AddGameObjectNode<T>(gameObject);

                windows.Add(node.Type, node);

                if (windowStack.TryPeek(out INode topNode))
                {
                    topNode?.TrySendRule(default(IWindowLostFocusRule));
                }
                windowStack.Push(node);

                node?.TrySendRule(default(IWindowFocusRule));
            }
            return node as T;
        }


        /// <summary>
        /// 关闭栈窗口
        /// </summary>
        public void Dispose<T>()
           where T : class, INode
        {
            if (windows.TryGetValue(typeof(T), out INode targetNode))
            {
                if (windowStack.TryPeek(out INode topNode))
                {
                    topNode?.TrySendRule(default(IWindowLostFocusRule));
                }
                while (windowStack.TryPop(out topNode))
                {
                    if (topNode.Id == targetNode.Id)
                    {
                        windows.Remove(topNode.Type);
                        topNode.ParentTo<GameObjectNode>()?.Dispose();
                        break;
                    }
                }
                if (windowStack.TryPeek(out topNode))
                {
                    topNode?.TrySendRule(default(IWindowFocusRule));
                }
            }
        }

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void DisposeTop()
        {
            if (windowStack.TryPop(out INode outNode))
            {
                windows.Remove(outNode.Type);
                outNode?.TrySendRule(default(IWindowLostFocusRule));
                if (windowStack.TryPeek(out INode topNode))
                {
                    topNode?.TrySendRule(default(IWindowFocusRule));
                }
                outNode.ParentTo<GameObjectNode>()?.Dispose();
            }
        }
        /// <summary>
        /// 关闭全部
        /// </summary>
        public void DisposeAll()
        {
            if (windowStack.TryPeek(out INode topNode))
            {
                topNode?.TrySendRule(default(IWindowFocusRule));
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
           where T : class, INode
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
            self.gameObject = self.AddComponent(out GameObjectNode _).Instantiate<WindowManager>();
        }
    }

    class WindowManagerUpdateRule : UpdateRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, float deltaTime)
        {
            if (self.windowStack.Count != 0)
            {
                if (self.windowStack.TryPeek(out INode node))
                {
                    node?.TrySendRule(default(IWindowFocusUpdateRule), deltaTime);
                }
            }
        }
    }

    class WindowManagerNodeAddRule : ListenerAddRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, INode node)
        {

        }
    }
    class WindowManagerNodeRemoveRule : ListenerRemoveRule<WindowManager>
    {
        public override void OnEvent(WindowManager self, INode node)
        {

        }
    }


}