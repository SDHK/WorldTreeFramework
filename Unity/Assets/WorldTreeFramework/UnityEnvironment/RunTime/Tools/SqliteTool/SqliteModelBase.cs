
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 10:56

* 描述： 

*/

using FrameWork;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.SqliteData
{
    /// <summary>
    /// 数据表管理基类
    /// </summary>
    public abstract class SqliteModelBase : IDisposable
    {
        public abstract Type EntityType { get; }
        public abstract void Dispose();
        public abstract void GetData(SqliteDataReader sqliteData);
    }


    /// <summary>
    /// 数据表管理泛型基类
    /// </summary>
    public abstract class SqliteModelBase<ID, T> : SqliteModelBase
        where T : SqliteEntityBase<ID>
    {
        public override Type EntityType => typeof(T);
        protected List<T> mList;
        protected Dictionary<ID, T> mDict;
        public SqliteModelBase()
        {
            mList = new List<T>();
            mDict = new Dictionary<ID, T>();
        }

        /// <summary>
        /// 获取集合
        /// </summary>
        public List<T> Get() => mList;

        /// <summary>
        /// 根据编号获取实体
        /// </summary>
        public T Get(ID id)
        {
            if (mDict.ContainsKey(id))
            {
                return mDict[id];
            }
            return null;
        }

        /// <summary>
        /// 根据编号获取实体列表
        /// </summary>
        public List<T> Get(params ID[] ids)
        {
            List<T> values = new List<T>();

            foreach (var item in mDict)
            {
                if (ids.Contains(item.Key))
                {
                    values.Add(item.Value);
                }
            }
            return values;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            mList.Clear();
            mDict.Clear();
        }

        public override void Dispose()
        {
            Clear();
        }
    }

}
