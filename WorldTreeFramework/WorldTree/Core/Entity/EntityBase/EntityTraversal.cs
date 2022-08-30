using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{

    public abstract partial class Entity
    {
        /// <summary>
        /// 递归遍历
        /// </summary>
        public static void TraversalRecursive(Entity node, Action<Entity> action)
        {
            action(node);
            foreach (var item in node.components)
            {
                TraversalRecursive(item.Value, action);
            }
            foreach (var item in node.children)
            {
                TraversalRecursive(item.Value, action);
            }
        }

        /// <summary>
        /// 前序遍历
        /// </summary>
        public static void Traversal(Entity node, Action<Entity> action)
        {
            if (node == null) return;

            UnitStack<Entity> allStack = node.Root.ObjectPoolManager.Get<UnitStack<Entity>>();
            UnitStack<Entity> localStack = node.Root.ObjectPoolManager.Get<UnitStack<Entity>>();
            allStack.Push(node);
            while (allStack.Count != 0)
            {
                Entity current = allStack.Pop();
                action(current);

                if (current.children != null)
                {
                    foreach (var item in current.children)
                    {
                        localStack.Push(item.Value);
                    }
                    while (localStack.Count != 0)
                    {
                        allStack.Push(localStack.Pop());
                    }
                    node.Root.ObjectPoolManager.Recycle(localStack);
                }

                if (current.components != null)
                {
                    foreach (var item in current.components)
                    {
                        localStack.Push(item.Value);
                    }
                    while (localStack.Count != 0)
                    {
                        allStack.Push(localStack.Pop());
                    }
                }
            }
            localStack.Recycle();
            allStack.Recycle();
        }

        /// <summary>
        /// 层序遍历
        /// </summary>
        public static void TraversalLevel(Entity node, Action<Entity> action)
        {
            if (node == null) return;

            UnitQueue<Entity> queue = node.Root.ObjectPoolManager.Get<UnitQueue<Entity>>();
            queue.Enqueue(node);

            while (queue.Any())
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
            queue.Recycle();
        }



    }
}
