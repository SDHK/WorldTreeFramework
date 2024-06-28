
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/14 11:18

* 描述： 和 EditorPrefs 一样的用法，得先连上数据库才能使用。
* 用来储存一些单独的数值

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
	/// SqlitePrefs
	/// </summary>
	public static class SqlitePrefs
    {
        static string[] names = new string[] { "key", "value" };
        static string[] IntTypes = new string[] { "string", "int" };
        static string[] FloatTypes = new string[] { "string", "float" };
        static string[] BoolTypes = new string[] { "string", "bool" };
        static string[] StringTypes = new string[] { "string", "string" };

		/// <summary>
		/// 设置值
		/// </summary>
		private static void SetValue(string tableName, string[] types, string name, byte[] data)
        {
            if (SqliteTool.IsOpen)
            {
                if (!SqliteTool.Contains(tableName))
                {
                    SqliteTool.CreateTable(tableName, names, types, SqliteTool.PRIMARY_KEY_NOT_NULL);
                }

                SqliteCommand command = SqliteTool.SqliteConnection.CreateCommand();
                command.CommandText = $"REPLACE INTO " + tableName + $"({SqliteTool.ParameterNames(names)}) VALUES ({SqliteTool.ParameterReferences(names)} )";
                SqliteTool.SetParameters(command, names, new byte[][] { name.GetBytes(),data });
                command.ExecuteNonQuery();
                command.Dispose();
            }
        }
		/// <summary>
		/// 获取值
		/// </summary>
		private static void GetValue(string tableName, string key, Action<SqliteDataReader> callBack)
        {
            if (SqliteTool.IsOpen)
                if (SqliteTool.Contains(tableName)) {

                    SqliteCommand command = SqliteTool.SqliteConnection.CreateCommand();
                    command.CommandText = $"SELECT value FROM {tableName}  WHERE key = @key";

                    SqliteTool. AddParameter(command, "key", key.GetBytes());

                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        callBack(reader);
                    }
                    reader.Close();
                    command.Dispose();

                }
        }
		/// <summary>
		/// 设置int
		/// </summary>
		public static void SetInt(string key, int value)
        {
            SetValue("SqlitePrefsInt", IntTypes, key, value.GetBytes());
        }
		/// <summary>
		/// 获取int
		/// </summary>
		public static int GetInt(string key)
        {
            int data = 0;
            GetValue("SqlitePrefsInt", key, (reader) => data = reader.GetBytesData(0).GetInt());
            return data;
        }
		/// <summary>
		/// 设置float
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetFloat(string key, float value)
        {
            SetValue("SqlitePrefsFloat", FloatTypes, key, value.GetBytes());
        }
		/// <summary>
		/// 获取float
		/// </summary>
		public static float GetFloat(string key)
        {
            float data = 0;
            GetValue("SqlitePrefsFloat", key, (reader) => data = reader.GetBytesData(0).GetFloat());
            return data;
        }
		/// <summary>
		/// 设置bool
		/// </summary>
		public static void SetBool(string key, bool value)
        {
            SetValue("SqlitePrefsBool", BoolTypes, key, value.GetBytes());
        }
		/// <summary>
		/// 获取bool
		/// </summary>
		public static bool GetBool(string key)
        {
            bool data = false;
            GetValue("SqlitePrefsBool", key, (reader) => data = reader.GetBytesData(0).GetBool());
            return data;
        }
		/// <summary>
		/// 设置string
		/// </summary>
		public static void SetString(string key, string value)
        {
            SetValue("SqlitePrefsString", StringTypes, key, value.GetBytes());
        }
		/// <summary>
		/// 获取string
		/// </summary>
		public static string GetString(string key)
        {
            string data = "";
            GetValue("SqlitePrefsString", key, (reader) => data = reader.GetBytesData(0).GetString());
            return data;
        }
    }
}
