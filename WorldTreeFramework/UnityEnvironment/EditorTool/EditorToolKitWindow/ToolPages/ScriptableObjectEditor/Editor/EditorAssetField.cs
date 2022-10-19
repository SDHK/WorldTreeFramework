
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/17 10:25

* 描述： 资源的字段编辑

*/

using Sirenix.OdinInspector;
using System;

namespace WorldTree
{

    /// <summary>
    /// 编辑资源字段
    /// </summary>
    [Serializable]
    public class EditorAssetField
    {
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldAllType FieldType;

        [ShowIf("FieldType", Value = EditorFieldAllType.Dictionary)]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType KeyType;

        [ShowIf("@FieldType==EditorFieldAllType.List||FieldType==EditorFieldAllType.Array||FieldType==EditorFieldAllType.Dictionary")]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType ValueType;

        [HideLabel, HorizontalGroup("类型")]
        [ShowIf("FieldType", Value = EditorFieldAllType.其它)]
        public string TypeName;

        [HideLabel, HorizontalGroup("名称")]
        public string FieldName;
        [HideLabel, HorizontalGroup("注释")]
        public string Comment;

    }
}
