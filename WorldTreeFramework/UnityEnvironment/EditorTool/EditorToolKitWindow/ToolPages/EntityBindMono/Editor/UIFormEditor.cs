//====================================================
//Author:Makka Pakka
//Time  :2022-07-12 14:36:03
//Desc  :
//====================================================
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class UIFormEditor : EditorWindow
    {
        private string mScriptContent;
        private string mFilePath;
        private Vector2 mScroll = Vector2.zero;

        /// <summary>
        /// 显示代码展示窗口
        /// </summary>
        public static void ShowWindow(string content, string filePath, Dictionary<string, string> methodDict = null)
        {
            var window = GetWindowWithRect<UIFormEditor>(new Rect(100, 50, 800, 700), true, "代码生成界面");
            window.mScriptContent = content;
            window.mFilePath = filePath;

            //处理代码新增
            if (File.Exists(filePath))
            {
                if (methodDict != null)
                {
                    //获取原始代码
                    string originScript = File.ReadAllText(filePath);
                    foreach (var item in methodDict)
                    {
                        //如果老代码中没有这个代码就进行插入操作
                        if (!originScript.Contains(item.Key))
                        {
                            int index = window.GetInsertIndex(originScript, "回调事件");
                            originScript = window.mScriptContent = originScript.Insert(index, item.Value+"\t\t");
                        }
                    }
                }
            }
            window.Show();
        }

        public void OnGUI()
        {
            //绘制scroview
            mScroll = EditorGUILayout.BeginScrollView(mScroll, GUILayout.Height(600), GUILayout.Width(800));
            mScriptContent = EditorGUILayout.TextArea(mScriptContent);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            //绘制脚本生成路径
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("脚本生成路径：" + mFilePath);
            EditorGUILayout.EndHorizontal();

            //绘制按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
            {
                //按钮事件
                BtnClick();
            }
            EditorGUILayout.EndHorizontal();
        }

        public void BtnClick()
        {
            File.WriteAllText(mFilePath, mScriptContent);

            if (EditorUtility.DisplayDialog("自动化生成工具", "生成脚本成功", "确定"))
            {
                Close();
            }
        }

        /// <summary>
        /// 获取插入代码的下标
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int GetInsertIndex(string content, string tag)
        {
            //找到 回调事件 下面的第一个 public 所在的位置 进行插入
            Regex regex = new Regex(tag);
            Match match = regex.Match(content);

            Regex regex1 = new Regex("public");
            MatchCollection matchCollection = regex1.Matches(content);

            for (int i = 0; i < matchCollection.Count; i++)
            {
                if (matchCollection[i].Index > match.Index)
                {
                    return matchCollection[i].Index;
                }
            }

            return -1;
        }
    }
}