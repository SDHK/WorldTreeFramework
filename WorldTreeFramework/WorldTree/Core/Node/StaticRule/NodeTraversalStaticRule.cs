
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 节点树遍历

*/

using System;

namespace WorldTree
{

    public static class NodeTraversalStaticRule
    {
        /// <summary>
        /// 前序遍历
        /// </summary>
        public static INode TraversalPreorder(this INode self, Action<INode> action)
        {
            INode current;
            UnitStack<INode> stack = self.PoolGet<UnitStack<INode>>();
            UnitStack<INode> localStack = self.PoolGet<UnitStack<INode>>();
            stack.Push(self);
            while (stack.Count != 0)
            {
                current = stack.Pop();
                action(current);
                if (current.m_Children != null)
                {
                    foreach (var item in current.m_Children)
                    {
                        localStack.Push(item.Value);
                    }
                    while (localStack.Count != 0)
                    {
                        stack.Push(localStack.Pop());
                    }
                }
                if (current.m_Components != null)
                {
                    foreach (var item in current.m_Components)
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
            return self;
        }

        /// <summary>
        /// 层序遍历
        /// </summary>
        public static INode TraversalLevel(this INode self, Action<INode> action)
        {
            UnitQueue<INode> queue = self.PoolGet<UnitQueue<INode>>();
            queue.Enqueue(self);

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();

                action(current);

                if (current.m_Components != null)
                {
                    foreach (var item in current.m_Components)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
                if (current.m_Children != null)
                {
                    foreach (var item in current.m_Children)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
            }
            queue.Dispose();
            return self;
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        public static INode TraversalPostorder(this INode self, Action<INode> action)
        {
            INode current;
            UnitStack<INode> stack = self.PoolGet<UnitStack<INode>>();
            UnitStack<INode> allStack = self.PoolGet<UnitStack<INode>>();
            stack.Push(self);
            while (stack.Count != 0)
            {
                current = stack.Pop();
                allStack.Push(current);
                if (current.m_Children != null)
                {
                    foreach (var item in current.m_Children)
                    {
                        stack.Push(item.Value);
                    }
                }
                if (current.m_Components != null)
                {
                    foreach (var item in current.m_Components)
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
            return self;
        }


        ///// <summary>
        ///// 前序遍历广播
        ///// </summary>
        //public static RuleActuator GetTraversalPreorderSystemBroadcast<LR>(this INode self)
        //   where LR : ISystem
        //{
        //    RuleActuator nodeQueue = self.AddChild<RuleActuator, Type>(typeof(LR));
        //    self.TraversalPostorder(nodeQueue.AddEntity);
        //    return nodeQueue;
        //}

        ///// <summary>
        ///// 层序遍历广播
        ///// </summary>
        //public static RuleActuator GetTraversalLevelSystemBroadcast<LR>(this INode self)
        //  where LR : ISystem
        //{
        //    RuleActuator nodeQueue = self.AddChild<RuleActuator, Type>(typeof(LR));
        //    self.TraversalLevel(nodeQueue.AddEntity);
        //    return nodeQueue;
        //}
        ///// <summary>
        ///// 后序遍历广播
        ///// </summary>
        //public static RuleActuator GetTraversalPostorderSystemBroadcast<LR>(this INode self)
        // where LR : ISystem
        //{
        //    RuleActuator nodeQueue = self.AddChild<RuleActuator, Type>(typeof(LR));
        //    self.TraversalPostorder(nodeQueue.AddEntity);
        //    return nodeQueue;
        //}




        ///// <summary>
        ///// 前序遍历执行
        ///// </summary>
        //public static RuleActuator GetTraversalPreorderSystemActuator<LR>(this INode self)
        //   where LR : ISystem
        //{
        //    RuleActuator systemActuator = self.AddChild<RuleActuator, Type>(typeof(LR));
        //    self.TraversalPostorder(systemActuator.AddEntity);
        //    return systemActuator;
        //}

        ///// <summary>
        ///// 层序遍历执行
        ///// </summary>
        //public static RuleActuator GetTraversalLevelSystemActuator<LR>(this INode self)
        //  where LR : ISystem
        //{
        //    RuleActuator systemActuator = self.AddChild<RuleActuator, Type>(typeof(LR));
        //    self.TraversalLevel(systemActuator.AddEntity);
        //    return systemActuator;
        //}

        ///// <summary>
        ///// 后序遍历执行
        ///// </summary>
        //public static RuleActuator GetTraversalPostorderSystemActuator<LR>(this INode self)
        // where LR : ISystem
        //{
        //    RuleActuator systemActuator = self.AddChild<RuleActuator, Type>(typeof(LR));
        //    self.TraversalPostorder(systemActuator.AddEntity);
        //    return systemActuator;
        //}
    }
}
