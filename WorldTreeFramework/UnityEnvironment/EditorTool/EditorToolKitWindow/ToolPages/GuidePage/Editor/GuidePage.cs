
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/11 16:09

* 描述： 指南页面

*/

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
    public class OdinMenuEditorItem : Entity
    {
        public OdinMenuTree tree;
    }

    class ItemAddSystem : AddSystem<OdinMenuEditorItem>
    {
        public override void OnAdd(OdinMenuEditorItem self)
        {
            self.tree.AddAssetAtPath("","");
        }
    }

    //[CreateAssetMenu]
    public class GuidePage : ScriptableObject
    {

        [BoxGroup("关于")]
        //[HorizontalGroup("关于/Split", 80)]
        //[VerticalGroup("关于/Split/Left")]
        //[HideLabel, PreviewField(80, ObjectFieldAlignment.Center)]
        //public Texture Icon;

        [HorizontalGroup("关于/Split", LabelWidth = 70)]
        [VerticalGroup("关于/Split/Right")]
        [DisplayAsString]
        [LabelText("框架名称")]
        public string Name = "WorldTreeFramework";

        [PropertySpace(10)]
        [VerticalGroup("关于/Split/Right")]
        [DisplayAsString]
        [LabelText("作者")]
        public string Author = "闪电黑客";

        [VerticalGroup("关于/Split/Todo")]
        [Title("工作计划", bold: false)]
        [HideLabel]
        [MultiLineProperty]
        public string Todo = "";


        [BoxGroup("简介")]
        [TabGroup("简介/Split", "模块")]
        [Title("ScriptTableObject资源编辑器", bold: false)]
        [HideLabel]
        [DisplayAsString(false)]
        public string ScriptTableObjectText = "ScriptTableObject节点套娃";
    }
}
