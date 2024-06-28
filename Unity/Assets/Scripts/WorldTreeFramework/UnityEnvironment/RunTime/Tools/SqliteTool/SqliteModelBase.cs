
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 10:56

* 描述： 

*/

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
		/// <summary>
		/// 数据表实体类型
		/// </summary>
		public abstract Type EntityType { get; }
		/// <summary>
		/// 释放
		/// </summary>
		public abstract void Dispose();
		/// <summary>
		/// 从数据库接收数据的方法
		/// </summary>
		public abstract void GetData(SqliteDataReader sqliteData);
    }


    /// <summary>
    /// 数据表管理泛型基类
    /// </summary>
    public abstract class SqliteModelBase<ID, T> : SqliteModelBase
        where T : SqliteEntityBase<ID>
    {
        public override Type EntityType => typeof(T);
		/// <summary>
		/// 数据表实体列表
		/// </summary>
		protected List<T> mList;
		/// <summary>
		/// 数据表实体字典
		/// </summary>
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
            List<T> valueList = new List<T>();

            foreach (var item in mDict)
            {
                if (ids.Contains(item.Key))
                {
                    valueList.Add(item.Value);
                }
            }
            return valueList;
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
