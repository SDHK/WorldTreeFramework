namespace WorldTree
{
    public partial class SystemBroadcast
    {
        public void Send()
        {
            if (systems != null && IsActive)
            {
                int length = entityQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (entityQueue.TryDequeue(out Entity entity))
                    {
                        systems.Send(entity);
                        if(!entity.IsRecycle) entityQueue.Enqueue(entity);
                    }
                }
            }
        }
        public void Send<T1>(T1 arg1)
        {
            if (systems != null && IsActive)
            {

                int length = entityQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (entityQueue.TryDequeue(out Entity entity))
                    {
                        systems.Send(entity, arg1);
                        if (!entity.IsRecycle) entityQueue.Enqueue(entity);
                    }
                }
            }
        }
        public void Send<T1, T2>(T1 arg1, T2 arg2)
        {
            if (systems != null && IsActive)
            {
                int length = entityQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (entityQueue.TryDequeue(out Entity entity))
                    {
                        systems.Send(entity, arg1, arg2);

                        if (!entity.IsRecycle) entityQueue.Enqueue(entity);
                    }
                }
            }
        }
        public void Send<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            if (systems != null && IsActive)
            {
                int length = entityQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (entityQueue.TryDequeue(out Entity entity))
                    {
                        systems.Send(entity, arg1, arg2, arg3);
                        if (!entity.IsRecycle) entityQueue.Enqueue(entity);
                    }
                }
            }
        }
        public void Send<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (systems != null && IsActive)
            {
                int length = entityQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (entityQueue.TryDequeue(out Entity entity))
                    {
                        systems.Send(entity, arg1, arg2, arg3, arg4);
                        if (!entity.IsRecycle) entityQueue.Enqueue(entity);
                    }
                }
            }
        }
        public void Send<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (systems != null && IsActive)
            {
                int length = entityQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (entityQueue.TryDequeue(out Entity entity))
                    {
                        systems.Send(entity, arg1, arg2, arg3, arg4, arg5);
                        if (!entity.IsRecycle) entityQueue.Enqueue(entity);
                    }
                }
            }
        }
    }

}
