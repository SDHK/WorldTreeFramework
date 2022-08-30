using WorldTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{


    public class PathAssetsFileEditorWindow : EditorWindow
    {
        public static PathAssetsFileEditorWindow windows;

        public EntityManager root;
        public UpdateManager update;


        [MenuItem("SDHK/PathAssets")]
        static void ShowWindow()
        {
            if (windows == null)
            {
                windows = EditorWindow.GetWindow<PathAssetsFileEditorWindow>(false, "路径资源编辑器");
            }
            windows.Show();//显示窗口
        }

        public PathAssetsFileEditorWindow()
        {
            World.Log = Debug.Log;
            World.LogWarning = Debug.LogWarning;
            World.LogError = Debug.LogError;

            root = new EntityManager();

            update = root.AddComponent<UpdateManager>();
            update.deltaTime = 0.2f;
            root.AddComponent<InitialDomain>();

            World.Log(root.ToStringDrawTree());

        }

        private void OnEnable()
        {

        }

        public void OnGUI()
        {


        }
        private void OnInspectorUpdate()
        {
            update.Update();
        }


        private void OnDestroy()
        {
            root.Dispose();
            update = null;
            World.Log(root.ToStringDrawTree());

        }

    }

}
