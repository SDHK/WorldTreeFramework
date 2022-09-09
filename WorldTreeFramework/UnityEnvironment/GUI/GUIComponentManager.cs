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


    class GUIComponentManagerEntityAddSystem : EntityAddSystem<GUIComponentManager>
    {
        public override void OnEntityAdd(GUIComponentManager self, Entity entity)
        {

        }
    }


    class GUIComponentManagerEntityRemoveSystem : EntityRemoveSystem<GUIComponentManager>
    {
        public override void OnEntityRemove(GUIComponentManager self, Entity entity)
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
