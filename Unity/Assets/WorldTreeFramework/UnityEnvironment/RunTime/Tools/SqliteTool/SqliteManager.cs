
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 10:56

* 描述： 

*/

using Logic.SqliteData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{


    /// <summary>
    /// 数据管理器
    /// </summary>
    public class SqliteManager : SingletonBase<SqliteManager>
    {
        #region Fields

        /// <summary>
        /// 缓存开关
        /// </summary>
        public bool isCache = true;

        private Dictionary<Type, SqliteModelBase> models = new Dictionary<Type, SqliteModelBase>();

        #endregion


        public override void OnInstance()
        {
            isCache = Application.isPlaying;
            Debug.Log("数据库缓存: " + isCache);
        }


        /// <summary>
        /// 释放数据模型
        /// </summary>
        public void Dispose<T>() where T : SqliteModelBase
        {
            models.Remove(typeof(T));
        }

        /// <summary>
        /// 获取数据模型
        /// </summary>
        public T GetModel<T>()
         where T : SqliteModelBase, new()
        {
            Type typekey = typeof(T);

            if (!models.TryGetValue(typekey, out SqliteModelBase model))
            {
                model = new T();
                SqliteTool.Select(model.EntityType.Name, model.GetData, "*");
                models.Add(typekey, model);
            }
            else if (!isCache)//数据已存在，不缓存，直接覆盖原有的
            {
                model = new T();
                SqliteTool.Select(model.EntityType.Name, model.GetData, "*");
                models[typekey] = model;
            }


            return model as T;
        }


        public override void Dispose()
        {
            foreach (var item in models)
            {
                item.Value.Dispose();
            }
            models.Clear();
        }
    }
}
