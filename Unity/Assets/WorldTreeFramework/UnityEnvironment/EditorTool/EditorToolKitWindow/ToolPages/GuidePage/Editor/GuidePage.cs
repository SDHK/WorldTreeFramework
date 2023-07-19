
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/11 16:09

* 描述： 指南页面

*/

using System;
using System.IO;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
//using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorTool
{
    //[CreateAssetMenu]
    [FilePath("Assets/WorldTreeFramework/UnityEnvironment/EditorTool/EditorToolKitWindow/ToolPages/GuidePage/Assets/GuidePage.asset")]
    public class GuidePage : ScriptableSingleton<GuidePage>
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


        [LabelText("是否编译后重启运行")]
        public bool isReloadedRun = false;

        [DidReloadScripts]
        private static async void OnScriptsReloaded()
        {
            if (EditorApplication.isPlaying)
            {
                if (GuidePage.Inst.isReloadedRun)
                {
                    Debug.Log("运行时编译：重启运行!!!");

                    EditorApplication.isPlaying = false;//Unity停止运行

                    while (Time.frameCount > 1)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.1f));//未停止时等待

                        if (Time.frameCount <= 1)
                        {
                            EditorApplication.isPlaying = true;//Unity运行
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("运行时编译：停止运行!!!");

                    EditorApplication.isPlaying = false;
                }
            }

        }



        [Button("Addressable打包")]
        public void Button()
        {
            //AddressableAssetSettings.BuildPlayerContent();
        }


        [InitializeOnLoadMethod]

        private static void InitializeOnLoadAttribute()
        {
            //Debug.Log("程序集重新编译前2");
        }

    }


}
