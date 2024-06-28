
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 14:45

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree.Sample
{


    /// <summary>
    /// 数据库类接口
    /// </summary>
    public interface ISqliteData
    {
        /// <summary>
        /// 从数据库接收数据的方法
        /// </summary>
        void SetData(SqliteDataReader sqliteData);

        /// <summary>
        /// 获取可以填入数据库的数据
        /// </summary>
        byte[][] GetData();
    }

	/// <summary>
	/// 数据库工具类
	/// </summary>
	public partial class SqliteHelper : SingletonBase<SqliteHelper>
    {
		/// <summary>
		/// 操作符
		/// </summary>
		public static readonly char[] Operations = { '=' };
        /// <summary>
        /// 缓存开关
        /// </summary>
        public bool IsCache = true;

        /// <summary>
        /// sql缓存的触发次数
        /// 不能为0
        /// </summary>
        public int SQL_NUM = 5;
        /// <summary>
        /// sql缓存触发消耗时间
        /// 不能为0
        /// </summary>
        public float SQL_TIMER = 0.05f;

		/// <summary>
		/// 查询语句
		/// </summary>
		private string sql = "";
		/// <summary>
		/// 查询结果缓存
		/// </summary>
		private static Dictionary<string, List<object>> sqlResultCacheDict = new Dictionary<string, List<object>>();
		/// <summary>
		/// 查询次数表
		/// </summary>
		private static Dictionary<string, int> sqlExecCounterDict = new Dictionary<string, int>();//次数表

		/// <summary>
		/// 查询条件
		/// </summary>
		public static List<string> ValueNameList = new List<string>();
		/// <summary>
		/// 查询条件值
		/// </summary>
		public static List<byte[]> ValueList = new List<byte[]>();


        public override void OnInstance()
        {
            IsCache = Application.isPlaying;
            Debug.Log("数据库缓存: " + IsCache);
        }


        /// <summary>
        /// 附加条件 与
        /// </summary>
        public SqliteHelper And(string str, byte[] data)
        {
            Instance.sql += " and " + str + "=@" + str;
            ValueNameList.Add(str);
            ValueList.Add(data);
            return Instance;
        }

        /// <summary>
        /// 附加条件 或
        /// </summary>
        public SqliteHelper Or(string str, byte[] data)
        {
            Instance.sql += " or " + str + "=@" + str;
            ValueNameList.Add(str);
            ValueList.Add(data);
            return Instance;
        }


        /// <summary>
        /// 查询相等条件：例 ("Id",1.GetBytes())
        /// </summary>
        public static SqliteHelper Where(string str, byte[] data)
        {
            Instance.sql = " WHERE " + str + "=@" + str;
            ValueNameList.Add(str);
            ValueList.Add(data);
            return Instance;
        }

        /// <summary>
        /// 查询相等条件：例 ("Id",1.GetBytes())
        /// </summary>
        public static SqliteHelper Where(string str, object data)
        {
            return Where(str,SqliteTool.SwitchTypeGetBytes(data.GetType().Name, data));
        }

        /// <summary>
        /// 附加条件 与
        /// </summary>
        public SqliteHelper And(string str, object data)
        {
            return And(str,SqliteTool.SwitchTypeGetBytes(data.GetType().Name, data));
        }

        /// <summary>
        /// 附加条件 或
        /// </summary>
        public SqliteHelper Or(string str, object data)
        {
            return Or(str, SqliteTool.SwitchTypeGetBytes(data.GetType().Name, data));
        }





        /// <summary>
        /// 条件筛选
        /// </summary>
        public static void Select(string tableName, string condition, string[] valueNames, byte[][] values, Action<SqliteDataReader> callBack, params string[] cols)
        {
            SqliteCommand command = SqliteTool.SqliteConnection.CreateCommand();
            command.CommandText = $"SELECT {SqliteTool.ParameterNames(cols)} FROM {tableName} {((condition == "") ? " " : " WHERE " + condition)}";

            SqliteTool.SetParameters(command, valueNames, values);

            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                callBack(reader);
            }
            reader.Close();
            command.Dispose();
        }

        /// <summary>
        /// 开始查询
        /// </summary>
        public List<T> FromAll<T>()
            where T : ISqliteData, new()
        {
            Type type = typeof(T);

            List<object> dataList = null;

            string commandText = $"SELECT * FROM {type.Name} {sql}";
            sql = "";//清除查询语句            

            if (IsCache && (SQL_NUM > 0 || SQL_TIMER > 0))
            {
                if (!sqlResultCacheDict.TryGetValue(commandText, out dataList))
                {

                    var st = Time.realtimeSinceStartup;
                    {
                        dataList = new List<object>();

                        SqliteCommand command = SqliteTool.SqliteConnection.CreateCommand();
                        command.CommandText = commandText;
                        SqliteTool.SetParameters(command, ValueNameList.ToArray(), ValueList.ToArray());
                        SqliteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            T data = new T();
                            data.SetData(reader);
                            dataList.Add(data);
                        }
                        reader.Close();
                        command.Dispose();

                        ValueNameList.Clear();
                        ValueList.Clear();
                    }
                    var intelval = Time.realtimeSinceStartup - st;

                    var counter = GetSqlExecCount(commandText);
                    if (counter >= SQL_NUM || intelval >= SQL_TIMER)
                    {
                        sqlResultCacheDict[commandText] = dataList;

                    }
                    else
                    {
                        sqlExecCounterDict[commandText] = counter + 1;
                    }
                }
            }
            else
            {
                dataList = new List<object>();

                SqliteCommand command = SqliteTool.SqliteConnection.CreateCommand();
                command.CommandText = commandText;
                SqliteTool.SetParameters(command, ValueNameList.ToArray(), ValueList.ToArray());
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    T data = new T();
                    data.SetData(reader);
                    dataList.Add(data);
                }
                reader.Close();
                command.Dispose();

                ValueNameList.Clear();
                ValueList.Clear();
            }


            var retList = new List<T>(dataList.Count);
            foreach (var item in dataList)
            {
                if (item is T tObj)
                {
                    retList.Add(tObj);
                }
            }

            return retList;
        }

        /// <summary>
        /// 开始查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="condition">条件</param>
        public static List<T> FromAll<T>(string str = "", byte[] data = null)
            where T : ISqliteData, new()
        {
            if (str != "")
            {
                Instance.sql = " WHERE " + str + "=@" + str;
                ValueNameList.Add(str);
                ValueList.Add(data);
            }

            return Instance.FromAll<T>();
        }

        /// <summary>
        /// 开始查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="condition">条件</param>
        public static List<T> FromAll<T>(string str, object data)
            where T : ISqliteData, new()
        {
            return FromAll<T>(str, SqliteTool.SwitchTypeGetBytes(data.GetType().Name, data));
        }




        /// <summary>
        /// 清空缓存
        /// </summary>
        public static void ClearCache()
        {
            sqlResultCacheDict?.Clear();
            sqlExecCounterDict?.Clear();
        }

		/// <summary>
		/// 获取sql执行次数
		/// </summary>
		private int GetSqlExecCount(string cmd)
        {
            int counter = 0;
            var ret = sqlExecCounterDict.TryGetValue(cmd, out counter);
            if (!ret)
            {
                sqlExecCounterDict[cmd] = 0;
            }

            return counter;
        }

        public override void Dispose()
        {
        }
    }
}
