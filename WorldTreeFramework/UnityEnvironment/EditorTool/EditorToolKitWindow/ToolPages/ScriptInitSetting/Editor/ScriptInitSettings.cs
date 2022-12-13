#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace EditorTool
{
    /// <summary>
    /// 脚本初始化设置
    /// </summary>
    [FilePath("Assets/SDHK/WorldTreeFramework/UnityEnvironment/EditorTool/EditorToolKitWindow/ToolPages/ScriptInitSetting/Assets/ScriptInitSetting.asset")]
    public class ScriptInitSetting : ScriptableSingleton<ScriptInitSetting>
    {
        #region Properties

        [HorizontalGroup("Open", 0.03f)]
        [LabelText("启用工具")]
        [ToggleLeft]
        [OnValueChanged("OnOpen")]
        public bool IsOpen;

        [PropertySpace(10)]
        [HorizontalGroup("Author", 0.03f)]
        [HideLabel]
        [EnableIf("IsOpen")]
        [OnValueChanged("OnOpen")]
        public bool IsInfo;

        [PropertySpace(10)]
        [HorizontalGroup("Author", 0.3f)]
        [LabelText("作者"), LabelWidth(60)]
        [EnableIf("@IsOpen && IsInfo")]
        [OnValueChanged("OnOpen")]
        public string Author;

        [PropertySpace(10)]
        [HorizontalGroup("Author", 0.1f)]
        [Button("清空", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsInfo")]
        public void ClearAuthor()
        {
            Author = null;
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Editor", 0.03f)]
        [HideLabel]
        [EnableIf("IsOpen")]
        [OnValueChanged("OnOpen")]
        public bool IsEditor;

        [PropertySpace(10)]
        [HorizontalGroup("Editor", 0.03f)]
        [LabelText("编辑器代码")]
        [DisplayAsString]
        [EnableIf("IsOpen")]
        [OnValueChanged("OnOpen")]
        public string EditorLabel;

        [PropertySpace(10)]
        [HorizontalGroup("Namespace", 0.03f)]
        [HideLabel]
        [EnableIf("IsOpen")]
        [OnValueChanged("OnOpen")]
        public bool IsNamespace;

        [PropertySpace(10)]
        [HorizontalGroup("Namespace", 0.3f)]
        [LabelText("命名空间"), LabelWidth(60)]
        [EnableIf("@IsOpen && IsNamespace")]
        [OnValueChanged("OnOpen")]
        public string Namespace;

        [PropertySpace(10)]
        [HorizontalGroup("Namespace", 0.1f)]
        [Button("清空", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsNamespace")]
        public void ClearNamespace()
        {
            Namespace = null;
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Namespace", 0.1f)]
        [Button("WorldTree", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsNamespace")]
        public void Frame()
        {
            Namespace = "WorldTree";
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Namespace", 0.1f)]
        [Button("EditorTool", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsNamespace")]
        public void ToolKit()
        {
            Namespace = "EditorTool";
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Namespace", 0.1f)]
        [Button("Logic", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsNamespace")]
        public void Game()
        {
            Namespace = "Logic";
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Parent", 0.03f)]
        [HideLabel]
        [EnableIf("IsOpen")]
        [OnValueChanged("OnOpen")]
        public bool IsParent;

        [PropertySpace(10)]
        [HorizontalGroup("Parent", 0.3f)]
        [LabelText("基类"), LabelWidth(60)]
        [EnableIf("@IsOpen && IsParent")]
        [OnValueChanged("OnOpen")]
        public string Parent;

        [PropertySpace(10)]
        [HorizontalGroup("Parent", 0.1f)]
        [Button("清空", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsParent")]
        public void CleaParent()
        {
            Parent = null;
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Parent", 0.1f)]
        [Button("Mono", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsParent")]
        public void Mono()
        {
            Parent = "MonoBehaviour";
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Parent", 0.1f)]
        [Button("Assets", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsParent")]
        public void Assets()
        {
            Parent = "ScriptableObject";
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Parent", 0.1f)]
        [Button("Editor", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsParent")]
        public void Editor()
        {
            Parent = "Editor";
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Region", 0.03f)]
        [HideLabel]
        [EnableIf("IsOpen")]
        [OnValueChanged("OnOpen")]
        public bool IsRegion;

        [PropertySpace(10)]
        [HorizontalGroup("Region", 0.305f)]
        [LabelText("代码块"), LabelWidth(60)]
        [EnableIf("@IsOpen && IsRegion")]
        [OnValueChanged("OnOpen")]
        public string[] Region;

        [PropertySpace(10)]
        [HorizontalGroup("Region", 0.1f)]
        [Button("清空", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsRegion")]
        public void ClearRegion()
        {
            Region = null;
            OnOpen();
        }

        [PropertySpace(10)]
        [HorizontalGroup("Region", 0.1f)]
        [Button("Default", ButtonSizes.Small)]
        [EnableIf("@IsOpen && IsRegion")]
        [OnValueChanged("OnOpen")]
        public void Default()
        {
            Region = new string[] { "Fields", "Properties", "Func" };
            OnOpen();
        }

        [GUIColor(1, 1, 1)]
        [EnableIf("IsOpen")]
        [Title("预览")]
        [DisplayAsString(false)]
        [HideLabel]
        public string Preview;

        private void OnOpen()
        {
            StringBuilder sbr = new StringBuilder();
            if (IsOpen)
            {
                if (IsEditor)
                {
                    sbr.AppendLine("<color=#9B9B9B>#if</color> UNITY_EDITOR");
                }

                if (IsInfo)
                {
                    sbr.AppendLine("<color=#57A64A>/****************************************</color>");
                    sbr.AppendLine("");
                    sbr.AppendLine($"<color=#57A64A>* 作者: {Author}</color>");
                    sbr.AppendLine($"<color=#57A64A>* 日期: {DateTime.Now:yyyy/MM/dd HH:mm:ss}</color>");
                    sbr.AppendLine("");
                    sbr.AppendLine("<color=#57A64A>* 描述: </color>");
                    sbr.AppendLine("");
                    sbr.AppendLine("<color=#57A64A>*/</color>");
                }

                if (IsParent && !string.IsNullOrWhiteSpace(Parent))
                {
                    if (Parent.Equals("MonoBehaviour") || Parent.Equals("ScriptableObject"))
                        sbr.AppendLine("<color=#569CEA>using</color> UnityEngine;");
                    if (Parent.Equals("Editor"))
                        sbr.AppendLine("<color=#569CEA>using</color> UnityEditor;");
                    sbr.AppendLine();
                }

                if (IsNamespace && !string.IsNullOrWhiteSpace(Namespace))
                {
                    sbr.AppendLine("<color=#569CEA>namespace</color> " + Namespace);
                    sbr.AppendLine("{");

                    if (IsParent && !string.IsNullOrWhiteSpace(Parent))
                    {
                        if (Parent.Equals("ScriptableObject"))
                        {
                            sbr.AppendLine($"    <color=#4EC9B0>[CreateAssetMenu(fileName = \"{name}\", menuName = \"{name}\")]</color>");
                        }
                        sbr.AppendLine($"    <color=#569CEA>public class</color> <color=#4EC9B0>{name}</color> : <color=#4EC9B0>{Parent}</color>");
                        //sbr.AppendFormat("    public class {0} : {1}\r\n", name, Parent);
                    }
                    else
                    {
                        sbr.AppendLine($"    <color=#569CEA>public class</color> <color=#4EC9B0>{name}</color>");
                        //sbr.AppendLine("    public class " + name);
                    }

                    sbr.AppendLine("    {");

                    if (IsRegion && Region != null && Region.Length > 0)
                    {
                        for (int i = 0; i < Region.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(Region[i]))
                            {
                                if (i != 0) sbr.AppendLine();
                                sbr.AppendLine("        <color=#9B9B9B>#region</color> " + Region[i]);
                                sbr.AppendLine();
                                sbr.AppendLine("        <color=#9B9B9B>#endregion</color>");
                            }
                        }
                    }

                    sbr.AppendLine("    }");

                    sbr.Append("}");
                }
                else
                {
                    if (IsParent && !string.IsNullOrWhiteSpace(Parent))
                    {
                        sbr.AppendLine($"<color=#569CEA>public class</color> <color=#4EC9B0>{name}</color> : <color=#4EC9B0>{Parent}</color>");
                        //sbr.AppendFormat("public class {0} : {1}\r\n", name, Parent);
                    }
                    else
                    {
                        sbr.AppendLine($"<color=#569CEA>public class</color> <color=#4EC9B0>{name}</color>");
                        //sbr.AppendLine("public class " + name);
                    }

                    sbr.AppendLine("{");

                    if (IsRegion && Region != null && Region.Length > 0)
                    {
                        for (int i = 0; i < Region.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(Region[i]))
                            {
                                if (i != 0) sbr.AppendLine();
                                sbr.AppendLine("    <color=#9B9B9B>#region</color> " + Region[i]);
                                sbr.AppendLine();
                                sbr.AppendLine("    <color=#9B9B9B>#endregion</color>");
                            }
                        }
                    }

                    sbr.Append("}");
                }
                if (IsEditor)
                {
                    sbr.Append("\r\n<color=#9B9B9B>#endif</color>");
                }

                Preview = sbr.ToString();
                AssetDatabase.Refresh();
            }
            else
            {
                sbr.Append("<color=#FF0000><size=250>未启用</size></color>");
                Preview = sbr.ToString();
            }
            sbr.Clear();
        }

        #endregion

        #region ScriptInit

        public class ScriptInit : UnityEditor.AssetModificationProcessor
        {
            private static void OnWillCreateAsset(string path)
            {
                if (!Inst.IsOpen) return;
                path = path.Replace(".meta", "");
                if (path.EndsWith(".cs"))
                {
                    string content = "";
                    content += File.ReadAllText(path);
                    string name = GetClassName(content);
                    if (string.IsNullOrEmpty(name))
                    {
                        return;
                    }

                    var result = GetScriptContent(name);

                    File.WriteAllText(path, result, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }
            }

            private static string GetScriptContent(string name)
            {
                StringBuilder sbr = new StringBuilder();

                if (Inst.IsEditor)
                {
                    sbr.AppendLine("#if UNITY_EDITOR");
                }

                if (Inst.IsInfo)
                {
                    sbr.AppendLine("/****************************************");
                    sbr.AppendLine("");
                    sbr.AppendLine($"* 作者: {Inst.Author}");
                    sbr.AppendLine($"* 日期: {DateTime.Now:yyyy/MM/dd HH:mm:ss}");
                    sbr.AppendLine("");
                    sbr.AppendLine("* 描述: ");
                    sbr.AppendLine("");
                    sbr.AppendLine("*/");
                }

                //if (!Inst.IsNamespace || string.IsNullOrWhiteSpace(Inst.Namespace) ||
                //    !Inst.Namespace.Contains("RSFramework")|| !Inst.Namespace.Contains("RS"))
                //{
                //    sbr.AppendLine("using RSFramework;");
                //}

                if (Inst.IsParent && !string.IsNullOrWhiteSpace(Inst.Parent))
                {
                    if (Inst.Parent.Equals("MonoBehaviour") || Inst.Parent.Equals("ScriptableObject"))
                        sbr.AppendLine("using UnityEngine;");
                    if (Inst.Parent.Equals("Editor"))
                        sbr.AppendLine("using UnityEditor;");
                    sbr.AppendLine();
                }


                if (Inst.IsNamespace && !string.IsNullOrWhiteSpace(Inst.Namespace))
                {
                    sbr.AppendLine("namespace " + Inst.Namespace);
                    sbr.AppendLine("{");
                    if (Inst.IsParent && !string.IsNullOrWhiteSpace(Inst.Parent))
                    {
                        //if (Inst.Parent.Equals("ScriptableObject"))
                        //{
                        //    sbr.AppendLine($"    [CreateAssetMenu(fileName = \"{name}\",menuName = \"{name}\")]");
                        //}
                        sbr.AppendFormat("    public class {0} : {1}\r\n", name, Inst.Parent);
                    }
                    else
                    {
                        sbr.AppendLine("    public class " + name);
                    }

                    sbr.AppendLine("    {");

                    if (Inst.IsRegion)
                    {
                        for (int i = 0; i < Inst.Region.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(Inst.Region[i]))
                            {
                                if (i != 0) sbr.AppendLine();
                                sbr.AppendLine("        #region " + Inst.Region[i]);
                                sbr.AppendLine();
                                sbr.AppendLine("        #endregion");
                            }
                        }
                    }

                    sbr.AppendLine("    }");

                    sbr.Append("}");
                }
                else
                {
                    if (Inst.IsParent && !string.IsNullOrWhiteSpace(Inst.Parent))
                    {
                        sbr.AppendFormat("public class {0} : {1}\r\n", name, Inst.Parent);
                    }
                    else
                    {
                        sbr.AppendLine("public class " + name);
                    }

                    sbr.AppendLine("{");

                    if (Inst.IsRegion)
                    {
                        for (int i = 0; i < Inst.Region.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(Inst.Region[i]))
                            {
                                if (i != 0) sbr.AppendLine();
                                sbr.AppendLine("    #region " + Inst.Region[i]);
                                sbr.AppendLine();
                                sbr.AppendLine("    #endregion");
                            }
                        }
                    }

                    sbr.Append("}");
                }

                if (Inst.IsEditor)
                {
                    sbr.Append("\r\n#endif");
                }

                return sbr.ToString();
            }

            private static string GetClassName(string content)
            {
                #region 字符串

                //string[] data = content.Split(' ');
                //int index = 0;

                //for (int i = 0; i < data.Length; i++)
                //{
                //    if (data[i].Contains("class"))
                //    {
                //        index = i + 1;
                //        break;
                //    }
                //}

                //if (data[index].Contains(":"))
                //{
                //    return data[index].Split(':')[0];
                //}
                //else
                //{
                //    return data[index];
                //}

                #endregion

                #region 正则表达式

                string patterm = "public class ([A-Za-z0-9_]+)\\s*:\\s*MonoBehaviour";
                var match = Regex.Match(content, patterm);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }

                return "";

                #endregion
            }
        }

        #endregion
    }
}
#endif