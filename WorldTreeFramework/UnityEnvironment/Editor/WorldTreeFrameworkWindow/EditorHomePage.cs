
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 14:29

* 描述： 主页面

*/


//WorldTreeFrameworkHome Page

namespace WorldTree
{
    public class EditorHomePage : Entity
    {

    }


    class EditorHomePageAddSystem : AddSystem<EditorHomePage>
    {
        public override void OnAdd(EditorHomePage self)
        {
            World.Log("!!!!!!!!!!add");
        }
    }
    class EditorHomePageOnGUISystem : OnGUISystem<EditorHomePage>
    {
        public override void OnGUI(EditorHomePage self, float deltaTime)
        {
            World.Log("!!!!!!!!!!OnGUI");
        }
    }

    class EditorHomePageUpdateSystem : UpdateSystem<EditorHomePage>
    {
        public override void Update(EditorHomePage self, float deltaTime)
        {
            World.Log("!!!!!!!!!!Update");
        }
    }
}
