using Sirenix.OdinInspector;
using System;

namespace WorldTree
{
    [Serializable]
    public class EditorAssetField
    {
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldAllType FieldType;

        [ShowIf("FieldType", Value = EditorFieldAllType.Dictionary)]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType KeyType;

        [ShowIf("FieldType", Value = EditorFieldAllType.Dictionary)]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType ValueType;

        [ShowIf("FieldType", Value = EditorFieldAllType.List)]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType ItemType;

        [HideLabel, HorizontalGroup("类型")]
        [ShowIf("FieldType", Value = EditorFieldAllType.其它)]
        public string TypeName;

        [HideLabel, HorizontalGroup("名称")]
        public string FieldName;
        [HideLabel, HorizontalGroup("注释")]
        public string Comment;

    }
}
