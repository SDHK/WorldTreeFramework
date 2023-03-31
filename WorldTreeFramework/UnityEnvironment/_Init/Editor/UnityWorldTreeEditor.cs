using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{

    [CustomEditor(typeof(UnityWorldTree))] 
    public class UnityWorldTreeEditor : Editor
    {
        private static UnityWorldTree script;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!Application.isPlaying) return;

            script = target as UnityWorldTree;

            GUILayout.Label(script.Core.ToStringDrawTree());
        }


    }
}
