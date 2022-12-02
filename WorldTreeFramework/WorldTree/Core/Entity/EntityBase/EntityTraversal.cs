using System;

namespace WorldTree
{

    public static class EntityTraversalExtension
    {
        /// <summary>
        /// 递归遍历（少用）
        /// </summary>
        private static Entity TraversalRecursive(this Entity self, Action<Entity> action)
        {
            action(self);
            foreach (var item in self.components)
            {
                item.Value.TraversalRecursive(action);
            }
            foreach (var item in self.children)
            {
                item.Value.TraversalRecursive(action);
            }
            return self;
        }

        /// <summary>
        /// 前序遍历
        /// </summary>
        public static Entity TraversalPreorder(this Entity self, Action<Entity> action)
        {
            Entity current;
            UnitStack<Entity> stack = self.PoolGet<UnitStack<Entity>>();
            UnitStack<Entity> localStack = self.PoolGet<UnitStack<Entity>>();
            stack.Push(self);
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
            return self;
        }

        /// <summary>
        /// 层序遍历
        /// </summary>
        public static Entity TraversalLevel(this Entity self, Action<Entity> action)
        {
            UnitQueue<Entity> queue = self.PoolGet<UnitQueue<Entity>>();
            queue.Enqueue(self);

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
            return self;
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        public static Entity TraversalPostorder(this Entity self, Action<Entity> action)
        {
            Entity current;
            UnitStack<Entity> stack = self.PoolGet<UnitStack<Entity>>();
            UnitStack<Entity> allStack = self.PoolGet<UnitStack<Entity>>();
            stack.Push(self);
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
            return self;
        }


        /// <summary>
        /// 前序遍历广播
        /// </summary>
        public static SystemBroadcast GetTraversalPreorderSystemBroadcast<T>(this Entity self)
           where T : ISystem
        {
            SystemBroadcast systemBroadcast = self.AddChildren<SystemBroadcast, Type>(typeof(T));
            self.TraversalPostorder(systemBroadcast.AddEntity);
            return systemBroadcast;
        }

        /// <summary>
        /// 层序遍历广播
        /// </summary>
        public static SystemBroadcast GetTraversalLevelSystemBroadcast<T>(this Entity self)
          where T : ISystem
        {
            SystemBroadcast systemBroadcast = self.AddChildren<SystemBroadcast, Type>(typeof(T));
            self.TraversalLevel(systemBroadcast.AddEntity);
            return systemBroadcast;
        }
        /// <summary>
        /// 后序遍历广播
        /// </summary>
        public static SystemBroadcast GetTraversalPostorderSystemBroadcast<T>(this Entity self)
         where T : ISystem
        {
            SystemBroadcast systemBroadcast = self.AddChildren<SystemBroadcast, Type>(typeof(T));
            self.TraversalPostorder(systemBroadcast.AddEntity);
            return systemBroadcast;
        }




        /// <summary>
        /// 前序遍历执行
        /// </summary>
        public static SystemActuator GetTraversalPreorderSystemActuator<T>(this Entity self)
           where T : ISystem
        {
            SystemActuator systemActuator = self.AddChildren<SystemActuator, Type>(typeof(T));
            self.TraversalPostorder(systemActuator.AddEntity);
            return systemActuator;
        }

        /// <summary>
        /// 层序遍历执行
        /// </summary>
        public static SystemActuator GetTraversalLevelSystemActuator<T>(this Entity self)
          where T : ISystem
        {
            SystemActuator systemActuator = self.AddChildren<SystemActuator, Type>(typeof(T));
            self.TraversalLevel(systemActuator.AddEntity);
            return systemActuator;
        }

        /// <summary>
        /// 后序遍历执行
        /// </summary>
        public static SystemActuator GetTraversalPostorderSystemActuator<T>(this Entity self)
         where T : ISystem
        {
            SystemActuator systemActuator = self.AddChildren<SystemActuator, Type>(typeof(T));
            self.TraversalPostorder(systemActuator.AddEntity);
            return systemActuator;
        }
    }
}
