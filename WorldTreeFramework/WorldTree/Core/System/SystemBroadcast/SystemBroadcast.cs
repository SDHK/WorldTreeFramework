/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 系统广播器
* 
* 用于广播拥有指定系统的实体

*/

namespace WorldTree
{
    /// <summary>
    /// 系统广播
    /// </summary>
    public partial class SystemBroadcast : Entity
    {
        /// <summary>
        /// 系统组
        /// </summary>
        public SystemGroup systems;

        public DynamicEntityQueue entityQueue;

        public override string ToString()
        {
            return $"SystemBroadcast : {systems?.systemType}";
        }

        /// <summary>
        /// 当前数量
        /// </summary>
        public int Count => entityQueue.Count;

        /// <summary>
        /// 添加实体
        /// </summary>
        public void Enqueue(Entity entity)
        {
            entityQueue.Enqueue(entity);
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        public void Remove(Entity entity)
        {
            entityQueue.Remove(entity);
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            entityQueue.Clear();
        }

        /// <summary>
        /// 出列
        /// </summary>
        public Entity Dequeue()
        {
            return entityQueue.Dequeue();
        }

        /// <summary>
        /// 尝试出列
        /// </summary>
        public bool TryDequeue(out Entity entity)
        {
            return entityQueue.TryDequeue(out entity);
        }


    }

    class SystemBroadcastAddSystem : AddSystem<SystemBroadcast>
    {
        public override void OnEvent(SystemBroadcast self)
        {
            if (self.thisPool != null)
            {
                self.AddComponent(out self.entityQueue);
            }

        }
    }

    class SystemBroadcastRemoveSystem : RemoveSystem<SystemBroadcast>
    {
        public override void OnEvent(SystemBroadcast self)
        {

        }
    }
}
