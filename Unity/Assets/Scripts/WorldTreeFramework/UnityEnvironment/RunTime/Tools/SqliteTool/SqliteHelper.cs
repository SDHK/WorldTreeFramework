
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


    public partial class SqliteHelper : SingletonBase<SqliteHelper>
    {

        public static readonly char[] operation = { '=' };
        /// <summary>
        /// 缓存开关
        /// </summary>
        public bool isCache = true;

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

        private string sql = "";
        private static Dictionary<string, List<object>> sqlResultCacheMap = new Dictionary<string, List<object>>();
        private static Dictionary<string, int> sqlExecCounterMap = new Dictionary<string, int>();//次数表

        public static List<string> valueNames = new List<string>();
        public static List<byte[]> values = new List<byte[]>();


        public override void OnInstance()
        {
            isCache = Application.isPlaying;
            Debug.Log("数据库缓存: " + isCache);
        }


        /// <summary>
        /// 附加条件 与
        /// </summary>
        public SqliteHelper And(string str, byte[] data)
        {
            Instance.sql += " and " + str + "=@" + str;
            valueNames.Add(str);
            values.Add(data);
            return Instance;
        }

        /// <summary>
        /// 附加条件 或
        /// </summary>
        public SqliteHelper Or(string str, byte[] data)
        {
            Instance.sql += " or " + str + "=@" + str;
            valueNames.Add(str);
            values.Add(data);
            return Instance;
        }


        /// <summary>
        /// 查询相等条件：例 ("Id",1.GetBytes())
        /// </summary>
        public static SqliteHelper Where(string str, byte[] data)
        {
            Instance.sql = " WHERE " + str + "=@" + str;
            valueNames.Add(str);
            values.Add(data);
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
            SqliteCommand command = SqliteTool.connection.CreateCommand();
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

            List<object> datas = null;

            string commandText = $"SELECT * FROM {type.Name} {sql}";
            sql = "";//清除查询语句            

            if (isCache && (SQL_NUM > 0 || SQL_TIMER > 0))
            {
                if (!sqlResultCacheMap.TryGetValue(commandText, out datas))
                {

                    var st = Time.realtimeSinceStartup;
                    {
                        datas = new List<object>();

                        SqliteCommand command = SqliteTool.connection.CreateCommand();
                        command.CommandText = commandText;
                        SqliteTool.SetParameters(command, valueNames.ToArray(), values.ToArray());
                        SqliteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            T data = new T();
                            data.SetData(reader);
                            datas.Add(data);
                        }
                        reader.Close();
                        command.Dispose();

                        valueNames.Clear();
                        values.Clear();
                    }
                    var intelval = Time.realtimeSinceStartup - st;

                    var counter = GetSqlExecCount(commandText);
                    if (counter >= SQL_NUM || intelval >= SQL_TIMER)
                    {
                        sqlResultCacheMap[commandText] = datas;

                    }
                    else
                    {
                        sqlExecCounterMap[commandText] = counter + 1;
                    }
                }
            }
            else
            {
                datas = new List<object>();

                SqliteCommand command = SqliteTool.connection.CreateCommand();
                command.CommandText = commandText;
                SqliteTool.SetParameters(command, valueNames.ToArray(), values.ToArray());
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    T data = new T();
                    data.SetData(reader);
                    datas.Add(data);
                }
                reader.Close();
                command.Dispose();

                valueNames.Clear();
                values.Clear();
            }


            var retList = new List<T>(datas.Count);
            foreach (var item in datas)
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
                valueNames.Add(str);
                values.Add(data);
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
            sqlResultCacheMap?.Clear();
            sqlExecCounterMap?.Clear();
        }


        private int GetSqlExecCount(string cmd)
        {
            int counter = 0;
            var ret = sqlExecCounterMap.TryGetValue(cmd, out counter);
            if (!ret)
            {
                sqlExecCounterMap[cmd] = 0;
            }

            return counter;
        }

        public override void Dispose()
        {
        }
    }
}
