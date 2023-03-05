
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 节点树遍历

*/

using System;

namespace WorldTree
{

    public abstract partial class Node
    {
        /// <summary>
        /// 前序遍历
        /// </summary>
        public Node TraversalPreorder(Action<Node> action)
        {
            Node current;
            UnitStack<Node> stack = this.PoolGet<UnitStack<Node>>();
            UnitStack<Node> localStack = this.PoolGet<UnitStack<Node>>();
            stack.Push(this);
            while (stack.Count != 0)
            {
                current = stack.Pop();
                action(current);
                if (current.children != null)
                {
                    foreach (var item in current.children)
                    {
                        localStack.Push(item.Value);
                    }
                    while (localStack.Count != 0)
                    {
                        stack.Push(localStack.Pop());
                    }
                }
                if (current.components != null)
                {
                    foreach (var item in current.components)
                    {
                        localStack.Push(item.Value);
                    }
                    while (localStack.Count != 0)
                    {
                        stack.Push(localStack.Pop());
                    }
                }
            }
            localStack.Dispose();
            stack.Dispose();
            return this;
        }

        /// <summary>
        /// 层序遍历
        /// </summary>
        public Node TraversalLevel(Action<Node> action)
        {
            UnitQueue<Node> queue = this.PoolGet<UnitQueue<Node>>();
            queue.Enqueue(this);

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();

                action(current);

                if (current.components != null)
                {
                    foreach (var item in current.components)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
                if (current.children != null)
                {
                    foreach (var item in current.children)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
            }
            queue.Dispose();
            return this;
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        public Node TraversalPostorder(Action<Node> action)
        {
            Node current;
            UnitStack<Node> stack = this.PoolGet<UnitStack<Node>>();
            UnitStack<Node> allStack = this.PoolGet<UnitStack<Node>>();
            stack.Push(this);
            while (stack.Count != 0)
            {
                current = stack.Pop();
                allStack.Push(current);
                if (current.children != null)
                {
                    foreach (var item in current.children)
                    {
                        stack.Push(item.Value);
                    }
                }
                if (current.components != null)
                {
                    foreach (var item in current.components)
                    {
                        stack.Push(item.Value);
                    }
                }
            }
            stack.Dispose();
            while (allStack.Count != 0)
            {
                action(allStack.Pop());
            }
            allStack.Dispose();
            return this;
        }


        ///// <summary>
        ///// 前序遍历广播
        ///// </summary>
        //public static SystemBroadcast GetTraversalPreorderSystemBroadcast<LR>(this Node self)
        //   where LR : ISystem
        //{
        //    SystemBroadcast systemBroadcast = self.AddChildren<SystemBroadcast, Type>(typeof(LR));
        //    self.TraversalPostorder(systemBroadcast.AddEntity);
        //    return systemBroadcast;
        //}

        ///// <summary>
        ///// 层序遍历广播
        ///// </summary>
        //public static SystemBroadcast GetTraversalLevelSystemBroadcast<LR>(this Node self)
        //  where LR : ISystem
        //{
        //    SystemBroadcast systemBroadcast = self.AddChildren<SystemBroadcast, Type>(typeof(LR));
        //    self.TraversalLevel(systemBroadcast.AddEntity);
        //    return systemBroadcast;
        //}
        ///// <summary>
        ///// 后序遍历广播
        ///// </summary>
        //public static SystemBroadcast GetTraversalPostorderSystemBroadcast<LR>(this Node self)
        // where LR : ISystem
        //{
        //    SystemBroadcast systemBroadcast = self.AddChildren<SystemBroadcast, Type>(typeof(LR));
        //    self.TraversalPostorder(systemBroadcast.AddEntity);
        //    return systemBroadcast;
        //}




        ///// <summary>
        ///// 前序遍历执行
        ///// </summary>
        //public static SystemBroadcast GetTraversalPreorderSystemActuator<LR>(this Node self)
        //   where LR : ISystem
        //{
        //    SystemBroadcast systemActuator = self.AddChildren<SystemBroadcast, Type>(typeof(LR));
        //    self.TraversalPostorder(systemActuator.AddEntity);
        //    return systemActuator;
        //}

        ///// <summary>
        ///// 层序遍历执行
        ///// </summary>
        //public static SystemBroadcast GetTraversalLevelSystemActuator<LR>(this Node self)
        //  where LR : ISystem
        //{
        //    SystemBroadcast systemActuator = self.AddChildren<SystemBroadcast, Type>(typeof(LR));
        //    self.TraversalLevel(systemActuator.AddEntity);
        //    return systemActuator;
        //}

        ///// <summary>
        ///// 后序遍历执行
        ///// </summary>
        //public static SystemBroadcast GetTraversalPostorderSystemActuator<LR>(this Node self)
        // where LR : ISystem
        //{
        //    SystemBroadcast systemActuator = self.AddChildren<SystemBroadcast, Type>(typeof(LR));
        //    self.TraversalPostorder(systemActuator.AddEntity);
        //    return systemActuator;
        //}
    }
}
