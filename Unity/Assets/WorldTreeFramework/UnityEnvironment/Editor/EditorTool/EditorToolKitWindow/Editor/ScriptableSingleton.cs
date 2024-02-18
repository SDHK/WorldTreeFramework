
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace EditorTool
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {

#if UNITY_EDITOR
        public string FilePath { get => GetFilePath(); }

        private static T mInstance;
        public static T Inst
        {
            get
            {
                //mInstance ??= LoadOrCreate();

                return mInstance;
            }
        }

        public static T LoadOrCreate()
        {
            string filePath = GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
				T instance = AssetDatabase.LoadAssetAtPath<T>(filePath);
                if (!instance)
                {
                    instance = CreateInstance<T>();

                    AssetDatabase.CreateAsset(instance, filePath);

                    AssetDatabase.Refresh();
                }

                return instance;
            }
            else throw new ArgumentNullException($"{nameof(ScriptableSingleton<T>)}: 请指定单例存档路径");
        }

        protected static string GetFilePath()
        {
            return typeof(T).GetCustomAttributes(true)
                .Cast<FilePathAttribute>()
                .FirstOrDefault(p => p != null)
                ?.FilePath;
        }
#endif
    }
}