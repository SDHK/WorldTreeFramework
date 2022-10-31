
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/19 10:24

* 描述： UI管理器

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
    //动画？
    //焦点进入，焦点离开，焦点Update
    //UI Update

    //Update系统，思考时间参数的必要

    //思考Manager的全局广播和监听功能
    public class UIManager : Entity
    {
        public UnitDictionary<Type, Entity> allWindows = new UnitDictionary<Type, Entity>();

        public UnitDictionary<Type, Entity> windows = new UnitDictionary<Type, Entity>();
        public UnitList<Entity> windowList = new UnitList<Entity>();
        
        //栈顶
        public Entity topPage;
        //栈底
        public Entity rootPage;

        public GameObjectComponent gameObject;


        /// <summary>
        /// 打开窗口
        /// </summary>
        public void Show<T>()
            where T : Entity
        {


        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void Close<T>()
           where T : Entity
        {


        }
      

        /// <summary>
        /// 关闭栈顶
        /// </summary>
        public void CloseTop()
        {


        }
        /// <summary>
        /// 关闭全部
        /// </summary>
        public void CloseAll()
        {


        }
    }

    class UIManagerAddSystem : AddSystem<UIManager>
    {
        public override void OnAdd(UIManager self)
        {
            self.gameObject = self.AddComponent<GameObjectComponent, GameObject>(null);
        }
    }

    //???GamaObject组件完善树结构
    class UIManagerEntityAddSystem : EntityAddSystem<UIManager>
    {
        public override void OnEntityAdd(UIManager self, Entity entity)
        {

        }
    }
    class UIManagerEntityRemoveSystem : EntityRemoveSystem<UIManager>
    {
        public override void OnEntityRemove(UIManager self, Entity entity)
        {

        }
    }


}