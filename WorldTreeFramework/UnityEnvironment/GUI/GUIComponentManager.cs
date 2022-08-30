/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 21:41

* 描述： 

*/
using UnityEngine;

namespace WorldTree
{
    class InitialDomainAddSystem1 : AddSystem<InitialDomain>
    {
        public override void OnAdd(InitialDomain self)
        {
            self.Root.AddChildren<GUIComponentManager>();
        }
    }

    public class GUIComponentManager : Entity
    {
        public bool isSkin = false;
        public int Size;
        public UnitDictionary<long, Entity> Guis = new UnitDictionary<long, Entity>();

    }

    public class GUIComponentManagerNewSystem : NewSystem<GUIComponentManager>
    {
        public override void OnNew(GUIComponentManager self)
        {


        }
    }


    class GUIComponentManagerEntitySystem : EntitySystem<GUIComponentManager>
    {
        public override void OnAddEntity(GUIComponentManager self, Entity entity)
        {

        }

        public override void OnRemoveEntity(GUIComponentManager self, Entity entity)
        {

        }
    }

    class GUIComponentManagerOnGUISystem : OnGUISystem<GUIComponentManager>
    {
        public override void OnGUI(GUIComponentManager self, float deltaTime)
        {

        }
    }



}
