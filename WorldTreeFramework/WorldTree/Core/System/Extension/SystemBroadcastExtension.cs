/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/

namespace WorldTree
{
    public partial class SystemBroadcast
    {
        public void Send()
        {
            if (systems != null && IsActive)
            {
                (update1, update2) = (update2, update1);

                int length = updateQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    long id = updateQueue.Dequeue();
                    if (update1.TryGetValue(id, out Entity entity))
                    {
                        update1.Remove(entity.id);
                        systems.Send(entity);
                        if (!entity.IsRecycle)
                        {
                            updateQueue.Enqueue(id);
                            update2.Add(entity.id, entity);
                        }
                    }
                }
            }
        }
        public void Send<T1>(T1 arg1)
        {
            if (systems != null && IsActive)
            {
                (update1, update2) = (update2, update1);

                int length = updateQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    long id = updateQueue.Dequeue();
                    if (update1.TryGetValue(id, out Entity entity))
                    {
                        update1.Remove(entity.id);
                        systems.Send(entity, arg1);
                        if (!entity.IsRecycle)
                        {
                            updateQueue.Enqueue(id);
                            update2.Add(entity.id, entity);
                        }
                    }
                }
            }
        }
        public void Send<T1, T2>(T1 arg1, T2 arg2)
        {
            if (systems != null && IsActive)
            {
                (update1, update2) = (update2, update1);
                int length = updateQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    long id = updateQueue.Dequeue();
                    if (update1.TryGetValue(id, out Entity entity))
                    {
                        update1.Remove(entity.id);
                        systems.Send(entity, arg1, arg2);
                        if (!entity.IsRecycle)
                        {
                            updateQueue.Enqueue(id);
                            update2.Add(entity.id, entity);
                        }
                    }
                }
            }
        }
        public void Send<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            if (systems != null && IsActive)
            {
                (update1, update2) = (update2, update1);
                int length = updateQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    long id = updateQueue.Dequeue();
                    if (update1.TryGetValue(id, out Entity entity))
                    {
                        update1.Remove(entity.id);
                        systems.Send(entity, arg1, arg2, arg3);
                        if (!entity.IsRecycle)
                        {
                            updateQueue.Enqueue(id);
                            update2.Add(entity.id, entity);
                        }
                    }
                }
            }
        }
        public void Send<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (systems != null && IsActive)
            {
                (update1, update2) = (update2, update1);
                int length = updateQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    long id = updateQueue.Dequeue();
                    if (update1.TryGetValue(id, out Entity entity))
                    {
                        update1.Remove(entity.id);
                        systems.Send(entity, arg1, arg2, arg3, arg4);

                        if (!entity.IsRecycle)
                        {
                            updateQueue.Enqueue(id);
                            update2.Add(entity.id, entity);
                        }
                    }
                }
            }
        }
        public void Send<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (systems != null && IsActive)
            {
                (update1, update2) = (update2, update1);
                int length = updateQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    long id = updateQueue.Dequeue();
                    if (update1.TryGetValue(id, out Entity entity))
                    {
                        update1.Remove(entity.id);
                        systems.Send(entity, arg1, arg2, arg3, arg4, arg5);
                        if (!entity.IsRecycle)
                        {
                            updateQueue.Enqueue(id);
                            update2.Add(entity.id, entity);
                        }
                    }
                }
            }
        }
    }
}
