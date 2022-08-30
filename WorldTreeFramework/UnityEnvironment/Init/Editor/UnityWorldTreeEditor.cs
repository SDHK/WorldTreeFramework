using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WorldTree
{

    [CustomEditor(typeof(UnityWorldTree))] 
    public class UnityWorldTreeEditor : Editor
    {
        private UnityWorldTree script;

        public override void OnInspectorGUI()
        { 
            base.OnInspectorGUI();
            if (!Application.isPlaying) return;

            script = target as UnityWorldTree;

            GUILayout.Label(script.root.ToStringDrawTree());
        }


    }
}
