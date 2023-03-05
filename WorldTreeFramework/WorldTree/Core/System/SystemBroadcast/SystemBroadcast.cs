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
    public partial class SystemBroadcast : Node
    {
        /// <summary>
        /// 系统组
        /// </summary>
        public RuleGroup systems;

        public DynamicEntityQueue entityQueue;

        public override string ToString()
        {
            return $"SystemBroadcast : {systems?.RuleType}";
        }

        /// <summary>
        /// 当前数量
        /// </summary>
        public int Count => entityQueue.Count;

        /// <summary>
        /// 添加实体
        /// </summary>
        public void Enqueue(Node entity)
        {
            entityQueue.Enqueue(entity);
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        public void Remove(Node entity)
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
        public Node Dequeue()
        {
            return entityQueue.Dequeue();
        }

        /// <summary>
        /// 尝试出列
        /// </summary>
        public bool TryDequeue(out Node entity)
        {
            return entityQueue.TryDequeue(out entity);
        }


    }

    class SystemBroadcastAddSystem : AddRule<SystemBroadcast>
    {
        public override void OnEvent(SystemBroadcast self)
        {
            if (self.thisPool != null)
            {
                self.AddComponent(out self.entityQueue);
            }

        }
    }

    class SystemBroadcastRemoveSystem : RemoveRule<SystemBroadcast>
    {
        public override void OnEvent(SystemBroadcast self)
        {

        }
    }
}
