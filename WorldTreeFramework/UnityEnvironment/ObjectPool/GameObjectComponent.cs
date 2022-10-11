
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/11 10:06

* 描述： 游戏物体组件
* 
* 将GameObject包装为组件
* 可随着父节点一起回收

*/

using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 游戏物体组件
    /// </summary>
    public class GameObjectComponent : Entity
    {
        public GameObjectPool pool;

        public GameObject gameObject;
        public Transform transform;
    }

    class GameObjectComponentSendSystem : SendSystem<GameObjectComponent, GameObject>
    {
        public override void Event(GameObjectComponent self, GameObject prefab)
        {
            if (self.gameObject != null)
            {
                self.pool?.Recycle(self.gameObject);
            }

            self.pool = self.GamePoolManager().GetPool(prefab);
            self.gameObject = self.pool.Get();
            self.transform = self.gameObject.transform;
        }
    }

    class GameObjectComponentRemoveSystem : RemoveSystem<GameObjectComponent>
    {
        public override void OnRemove(GameObjectComponent self)
        {
            self.pool?.Recycle(self.gameObject);
            self.gameObject = null;
            self.transform = null;
            self.pool = null;

        }
    }
}
