//====================================================
//Author:Makka Pakka
//Time  :2022-12-02 18:27:26
//Desc  :
//====================================================
using System;

namespace EditorTool
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FilePathAttribute : Attribute
    {
        internal string FilePath;

        /// <summary>
        /// 单例存放路径
        /// </summary>
        /// <param name="path">相对 Project 路径</param>
        public FilePathAttribute(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Invalid relative path (it is empty)");
            if (path[0] == '/') path = path[1..];
            FilePath = path;
        }
    }
}