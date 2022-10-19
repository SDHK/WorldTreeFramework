
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

    //思考Manager的全局广播和监听功能
    public class UIManager : Entity
    {

        public UnitDictionary<Type, Entity> pages = new UnitDictionary<Type, Entity>();

        public Entity topPage;
        public Entity rootPage;

        public GameObjectComponent gameObject;


        public void Show<T>()
            where T : Entity
        {


        }

        public void Close<T>()
           where T : Entity
        {


        }

        public void Close()
        {


        }
    }

    class UIManagerAddSystem : AddSystem<UIManager>
    {
        public override void OnAdd(UIManager self)
        {
            self.gameObject = self.AddComponent<GameObjectComponent,GameObject>(null);
        }
    }


}