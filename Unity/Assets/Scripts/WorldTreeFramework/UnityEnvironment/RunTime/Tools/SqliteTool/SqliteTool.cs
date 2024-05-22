
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/25 14:44

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace WorldTree.Sample
{
	/// <summary>
	/// SQlite不加密特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
    public class SqliteUnEncrypted : Attribute { }

    public static partial class SqliteTool
    {
        public static readonly string PRIMARY_KEY_NOT_NULL = "PRIMARY KEY NOT NULL";
        public static readonly string UNIQUE = "UNIQUE";


        /// <summary>
        /// 是否加密
        /// </summary>
        public static bool isEncryption = false;

        /// <summary>
        /// 加密
        /// </summary>
        public static Func<byte[], byte[]> Encryption = Xor;

        /// <summary>
        /// 解密
        /// </summary>
        public static Func<byte[], byte[]> Decryption = Xor;


        /// <summary>
        /// 异或因子
        /// </summary>


        public static SqliteConnection connection;
        /// <summary>
        /// 判断是否连接到数据库
        /// </summary>
        public static bool isOpen => (connection == null) ? false : connection.State == ConnectionState.Open;


        /// <summary>
        /// 异或因子
        /// </summary>
        private static readonly byte[] xorScale = new byte[] { 45, 66, 38, 55, 23, 254, 9, 165, 90, 19, 41, 45, 201, 58, 55, 37, 254, 185, 165, 169, 19, 171 };

        /// <summary>
        /// 对数组进行异或
        /// </summary>
        public static byte[] Xor(byte[] buffer)
        {
            if (buffer == null) return null;
            int iScaleLen = xorScale.Length;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ xorScale[i % iScaleLen]);
            }
            return buffer;
        }




        /// <summary>
        /// 创建数据库
        /// </summary>
        public static void Create(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));//如果文件夹不存在就创建它
            SqliteConnection.CreateFile(path);
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        public static SqliteConnection Connection(string path)
        {
            try
            {
                connection = new SqliteConnection("Data Source = " + path);
                connection.Open();
                Debug.Log("数据库连接成功");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                connection = null;
            }
            return connection;
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public static void Close()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }


        /// <summary>
        /// 执行非查询
        /// </summary>
        public static void Execute(string sql)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /// <summary>
        /// 执行读取
        /// </summary>
        public static SqliteDataReader ExecuteReader(string sql)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            var reader = command.ExecuteReader();
            command.Dispose();
            return reader;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public static object ExecuteScalar(string sql)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            var reader = command.ExecuteScalar();
            command.Dispose();
            return reader;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cols">字段名</param>
        /// <param name="colTypes">类型</param>
        public static void CreateTable(string tableName, string[] cols, string[] colTypes, params string[] setKey)
        {
            string sql = "CREATE TABLE IF NOT EXISTS " + tableName + "(";
            for (int i = 0; i < cols.Length; i++)
            {
                sql += cols[i] + " " + colTypes[i];

                if (i < setKey.Length)
                {
                    if (setKey[i] != null)
                    {
                        sql += " " + setKey[i];
                    }
                }
                if (i < cols.Length - 1)
                    sql += ",";
            }
            sql += ")";
            Execute(sql);
        }

        /// <summary>
        /// 删除表
        /// </summary>
        public static void Delete(string tableName)
        {
            string sql = "DROP TABLE IF EXISTS " + tableName;
            Execute(sql);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        public static void Delete(string tableName, string condition, string[] valueNames, byte[][] values)
        {
            string sql = "DELETE FROM " + tableName + " WHERE " + condition;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            SetParameters(command, valueNames, values);
            command.ExecuteNonQuery();
            command.Dispose();
        }


        /// <summary>
        /// 判断表存在
        /// </summary>
        public static bool Contains(string tableName)
        {
            string sql = $"SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'";
            return Convert.ToInt32(ExecuteScalar(sql)) > 0;
        }


        /// <summary>
        /// 判断表包含
        /// </summary>
        public static bool Contains(string tableName, string col, byte[] value)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT count(*) FROM {tableName} WHERE {col} = @{col}";
            AddParameter(command, col, value);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }


        /// <summary>
        /// 条件筛选
        /// </summary>
        public static void Select(string tableName, string condition, string[] valueNames, byte[][] values, Action<SqliteDataReader> callBack, params string[] cols)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT {ParameterNames(cols)} FROM {tableName} {((condition == "") ? " " : " WHERE " + condition)}";

            SetParameters(command, valueNames, values);

            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                callBack(reader);
            }
            reader.Close();
            command.Dispose();
        }



        /// <summary>
        /// 条件筛选
        /// </summary>
        public static void Select(string tableName, Action<SqliteDataReader> callBack, params string[] cols)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT {ParameterNames(cols)} FROM {tableName} ";

            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                callBack(reader);
            }
            reader.Close();
            command.Dispose();
        }

        /// <summary>
        /// 插入数据 
        /// </summary>
        public static void Insert(string tableName, string[] cols, byte[][] values)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO " + tableName + $"({ParameterNames(cols)}) VALUES ({ParameterReferences(cols)} )";
            try
            {
                
                SetParameters(command, cols, values);
                //Debug.Log(command.CommandText);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch(Exception ex)
            {
                Debug.LogError($"执行失败,{command.CommandText},{ex.Message},{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 取代
        /// </summary>
        public static void Replace(string tableName, string[] cols, byte[][] values)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"REPLACE INTO " + tableName + $"({ParameterNames(cols)}) VALUES ({ParameterReferences(cols)} )";
            SetParameters(command, cols, values);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static void Update(string tableName, string condition, string[] cols, byte[][] values)
        {
            SqliteCommand command = connection.CreateCommand();

            string sql = "";
            for (int i = 0; i < cols.Length; i++)
            {
                sql += $"{cols[i]} = ' @{cols[i]}'";
                if (i < cols.Length - 1)
                    sql += ",";
            }

            command.CommandText = $"UPDATE  {tableName}  SET {sql} {((condition == "") ? " " : " WHERE " + condition)}";

            SetParameters(command, cols, values);

            command.ExecuteNonQuery();
            command.Dispose();
        }


        /// <summary>
        /// 设置多个参数
        /// </summary>
        public static SqliteCommand SetParameters(SqliteCommand command, string[] cols, byte[][] values)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                AddParameter(command, cols[i], values[i]);
            }
            return command;
        }

        /// <summary>
        /// 添加一个参数
        /// </summary>
        public static void AddParameter(SqliteCommand command, string col, byte[] value)
        {
            command.Parameters.Add(col, DbType.Binary).Value = (isEncryption) ? Encryption?.Invoke(value) ?? value : value;
        }

        /// <summary>
        /// 获取byte[]
        /// </summary>
        public static byte[] GetBytesData(this SqliteDataReader reader, string name)
        {
            //一块的数据
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(name, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                byte[] result = stream.ToArray();
                return (isEncryption) ? Decryption?.Invoke(result) ?? result : result;
            }
        }

        /// <summary>
        /// 获取byte[]
        /// </summary>
        public static byte[] GetBytesData(this SqliteDataReader reader, int index)
        {
            //一块的数据
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                if (reader.IsDBNull(index))
                {
                    return null;
                }
                else
                {

                    while ((bytesRead = reader.GetBytes(index, fieldOffset, buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, (int)bytesRead);
                        fieldOffset += bytesRead;
                    }

                    byte[] result = stream.ToArray();

                    return (isEncryption) ? Decryption?.Invoke(result) ?? result : result;
                }
            }
        }


        /// <summary>
        /// 参数名字拼接
        /// </summary>
        public static string ParameterNames(string[] names)
        {
            string sql = "";
            for (int i = 0; i < names.Length; i++)
            {
                sql += $"{names[i]}";
                if (i < names.Length - 1)
                    sql += ",";
            }
            return sql;
        }

        /// <summary>
        /// 参数名字引用拼接
        /// </summary>
        public static string ParameterReferences(string[] strs)
        {
            string sql = "";
            for (int i = 0; i < strs.Length; i++)
            {
                sql += $"@{strs[i]}";

                if (i < strs.Length - 1)
                    sql += ",";
            }
            return sql;
        }



        #region 反射

        /// <summary>
        /// 获取类型字段名
        /// </summary>
        public static string[] GetFieldName<T>()
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields();
            string[] fieldNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldNames[i] = fields[i].Name;
            }
            return fieldNames;
        }

        /// <summary>
        /// 获取类型字段类型
        /// </summary>
        public static string[] GetFieldType<T>()
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields();
            string[] fieldTypes = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldTypes[i] = fields[i].FieldType.Name;
            }
            return fieldTypes;
        }



        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="setKey">1设置主键，2设置不可重复，</param>
        public static void CreateTable<T>(params string[] setKey)
        {
            string tableName = typeof(T).Name;
            string[] cols = new string[typeof(T).GetFields().Length];
            string[] colTypes = new string[typeof(T).GetFields().Length];
            for (int i = 0; i < typeof(T).GetFields().Length; i++)
            {
                cols[i] = typeof(T).GetFields()[i].Name;
                colTypes[i] = typeof(T).GetFields()[i].FieldType.Name;
            }

            CreateTable(tableName, cols, colTypes, setKey);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static void Delete<T>()
        {
            Delete(typeof(T).Name);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static void Delete<T>(string condition, string[] valueNames, byte[][] values)
        {
            Type type = typeof(T);
            var bit = isEncryption;
            if (type.GetCustomAttributes(typeof(SqliteUnEncrypted), false).Length != 0) isEncryption = false;
            Delete(typeof(T).Name, condition, valueNames, values);
            isEncryption = bit;
        }

        /// <summary>
        /// 插入
        /// </summary>
        public static void Insert<T>(T obj)
        {
            Type type = typeof(T);
            var bit = isEncryption;
            if (type.GetCustomAttributes(typeof(SqliteUnEncrypted), false).Length != 0) isEncryption = false;
            string tableName = type.Name;
            FieldInfo[] fieldInfo = type.GetFields();

            string[] cols = new string[fieldInfo.Length];
            byte[][] values = new byte[fieldInfo.Length][];

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                cols[i] = fieldInfo[i].Name;
                values[i] = SwitchTypeGetBytes(fieldInfo[i].FieldType.Name, fieldInfo[i].GetValue(obj));//需要强转式
            }
            Insert(tableName, cols, values);
            isEncryption = bit;
        }



        /// <summary>
        /// 取代
        /// </summary>
        public static void Replace<T>(T obj)
        {
            Type type = typeof(T);
            var bit = isEncryption;
            if (type.GetCustomAttributes(typeof(SqliteUnEncrypted), false).Length != 0) isEncryption = false;
            string tableName = type.Name;
            FieldInfo[] fieldInfo = type.GetFields();

            string[] cols = new string[fieldInfo.Length];
            byte[][] values = new byte[fieldInfo.Length][];

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                cols[i] = fieldInfo[i].Name;
                values[i] = SwitchTypeGetBytes(fieldInfo[i].FieldType.Name, fieldInfo[i].GetValue(obj));//需要强转式
            }
            Replace(tableName, cols, values);
            isEncryption = bit;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static void Update<T>(T obj, string condition, string[] valueNames, byte[][] values)
        {
            Type type = typeof(T);
            var bit = isEncryption;
            if (type.GetCustomAttributes(typeof(SqliteUnEncrypted), false).Length != 0) isEncryption = false;

            string tableName = type.Name;
            FieldInfo[] fieldInfo = type.GetFields();

            string[] cols = new string[fieldInfo.Length + valueNames.Length];
            byte[][] datas = new byte[fieldInfo.Length + values.Length][];

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                cols[i] = fieldInfo[i].Name;
                datas[i] = SwitchTypeGetBytes(fieldInfo[i].FieldType.Name, fieldInfo[i].GetValue(obj));//需要强转式
            }
            for (int i = 0; i < valueNames.Length; i++)
            {
                cols[i] = valueNames[i];
                datas[i] = values[i];
            }
            Update(tableName, condition, cols, datas);

            isEncryption = bit;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static void Update<T>(T obj)
        {
            Type type = typeof(T);
            var bit = isEncryption;
            if (type.GetCustomAttributes(typeof(SqliteUnEncrypted), false).Length != 0) isEncryption = false;
            string tableName = type.Name;
            FieldInfo[] fieldInfo = type.GetFields();

            string[] cols = new string[fieldInfo.Length];
            byte[][] datas = new byte[fieldInfo.Length][];

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                cols[i] = fieldInfo[i].Name;
                datas[i] = SwitchTypeGetBytes(fieldInfo[i].FieldType.Name, fieldInfo[i].GetValue(obj));//需要强转式
            }

            Update(tableName, "", cols, datas);
            isEncryption = bit;

        }

        /// <summary>
        /// 筛选
        /// </summary>
        public static List<T> Select<T>()
            where T : new()
        {
            return Select<T>("", null, null);
        }
        /// <summary>
        /// 筛选
        /// </summary>
        public static List<T> Select<T>(string condition, string[] valueNames, byte[][] values)
            where T : new()
        {
            Type type = typeof(T);
            var bit = isEncryption;
            if (type.GetCustomAttributes(typeof(SqliteUnEncrypted), false).Length != 0) isEncryption = false;
            string tableName = type.Name;
            FieldInfo[] fieldInfo = type.GetFields();

            string[] cols = new string[fieldInfo.Length];

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                cols[i] = fieldInfo[i].Name;
            }

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = $"SELECT {ParameterNames(cols)} FROM {tableName}  {((condition == "") ? " " : " WHERE " + condition)}";

            if (condition != "")
            {
                SetParameters(command, valueNames, values);
            }
            Debug.Log(command.CommandText);
            SqliteDataReader reader = command.ExecuteReader();


            List<T> list = new List<T>();

            while (reader.Read())
            {
                T t = new T();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var fileInfo = fieldInfo[i];
                    if (!reader.IsDBNull(i))
                    {
                        object value = SwitchTypeGetValue(fileInfo.FieldType.Name.ToLower(), reader.GetBytesData(fieldInfo[i].Name));
                        fileInfo.SetValue(t, value);
                    }
                }
                list.Add(t);
            }

            isEncryption = bit;

            reader.Close();
            command.Dispose();
            return list;
        }


        /// <summary>
        /// 给反射用的 byte[] 转 object
        /// </summary>
        private static object SwitchTypeGetValue(string typeName, byte[] data)
            => typeName.ToLower() switch
            {
                "short" or
                "int16" => data.GetShort(),

                "short[]" or
                "int16[]" => data.GetShorts(),

                "int" or
                "index" or
                "idx" or
                "key" or
                "int32" => data.GetInt(),

                "int[]" or
                "int32[]" => data.GetInts(),

                "long" or
                "int64" => data.GetLong(),

                "long[]" or
                "int64[]" => data.GetLongs(),

                "single" or
                "float" => data.GetFloat(),

                "single[]" or
                "float[]" => data.GetFloats(),

                "double" => data.GetDouble(),
                "double[]" => data.GetDoubles(),

                "decimal" => data.GetDecimal(),
                "decimal[]" => data.GetDecimals(),

                "boolean" or
                "bool" => data.GetBool(),

                "boolean[]" or
                "bool[]" => data.GetBools(),

                "byte" => data.GetByte(),
                "byte[]" => data,

                "char" => data.GetChar(),

                "string" or
                "remark" or
                "map" or
                "nstring" => data.GetString(),
                "string[]" => data.GetStrings(),

                "datetime" => data.GetDateTime(),
                "datetime[]" => data.GetDateTimes(),

                "guid" => data.GetGuid(),
                "guid[]" => data.GetGuids(),

                "vector2" => data.GetVector2(),
                "vector2[]" => data.GetVector2s(),

                "vector3" => data.GetVector3(),
                "vector3[]" => data.GetVector3s(),

                "vector2int" => data.GetVector2Int(),
                "vector2int[]" => data.GetVector2Ints(),

                "vector3int" => data.GetVector3Int(),
                "vector3int[]" => data.GetVector3Ints(),

                "object" => data.GetObject(),


                _ => data.GetObject()
            };

        /// <summary>
        ///  给反射用的 object 转 byte[]
        /// </summary>
        public static byte[] SwitchTypeGetBytes(string TypeName, object obj)
           => TypeName.ToLower() switch
           {
               "short" or
              "int16" => ((short)obj).GetBytes(),

               "short[]" or
                "int16[]" => ((short[])obj).GetBytes(),

               "int" or
               "index" or
               "idx" or
               "key" or
                "int32" => ((int)obj).GetBytes(),

               "int[]" or
               "int32[]" => ((int[])obj).GetBytes(),

               "long" or
               "int64" => ((long)obj).GetBytes(),

               "long[]" or
               "int64[]" => ((long[])obj).GetBytes(),

               "single" or
               "float" => ((float)obj).GetBytes(),

               "single[]" or
               "float[]" => ((float[])obj).GetBytes(),

               "double" => ((double)obj).GetBytes(),
               "double[]" => ((double[])obj).GetBytes(),

               "decimal" => ((decimal)obj).GetBytes(),
               "decimal[]" => ((decimal[])obj).GetBytes(),

               "bool" or
               "boolean" => ((bool)obj).GetBytes(),

               "byte" => ((byte)obj).GetBytes(),

               "char" => ((char)obj).GetBytes(),

               "string" or
                "remark" or
                "map" or
                "nstring" => ((string)obj).GetBytes(),

               "string[]" => ((string[])obj).GetBytes(),

               "datetime" => ((DateTime)obj).GetBytes(),

               "guid" => ((Guid)obj).GetBytes(),

               "vector2" => ((Vector2)obj).GetBytes(),
               "vector2[]" => ((Vector2[])obj).GetBytes(),

               "vector3" => ((Vector3)obj).GetBytes(),
               "vector3[]" => ((Vector3[])obj).GetBytes(),

               "vector2int" => ((Vector2Int)obj).GetBytes(),
               "vector2int[]" => ((Vector2Int[])obj).GetBytes(),

               "vector3int" => ((Vector3Int)obj).GetBytes(),
               "vector3int[]" => ((Vector3Int[])obj).GetBytes(),

               "object" => obj.GetBytes(),

               "byte[]" => ((byte[])obj),

               _ => obj.GetBytes(),
           };

        #endregion

        public static List<string> Keyword = new List<string>()
        {
          @"ABORT","CREATE","FROM","NATURAL",
            "ACTION","CROSS","FULL","NO",
            "ADD","CURRENT_DATE","GLOB","NOT",
            "AFTER","CURRENT_TIME","GROUP","NOTNULL",
            "ALL","CURRENT_TIMESTAMP","HAVING","NULL",
            "ALTER","DATABASE","IF","OF",
            "ANALYZE","DEFAULT","IGNORE","OFFSET",
            "AND","DEFERRABLE","IMMEDIATE","ON",
            "AS","DEFERRED","IN","OR",
            "ASC","DELETE","INDEX","ORDER",
            "ATTACH","DESC","INDEXED","OUTER",
            "AUTOINCREMENT","DETACH","INITIALLY","PLAN",
            "BEFORE","DISTINCT","INNER","PRAGMA",
            "BEGIN","DROP","INSERT","PRIMARY",
            "BETWEEN","EACH","INSTEAD","QUERY",
            "BY","ELSE","INTERSECT","RAISE",
            "CASCADE","END","INTO","RECURSIVE",
            "CASE","ESCAPE","IS","REFERENCES",
            "CAST","EXCEPT","ISNULL","REGEXP",
            "CHECK","EXCLUSIVE","JOIN","REINDEX",
            "COLLATE","EXISTS","KEY","RELEASE",
            "COLUMN","EXPLAIN","LEFT","RENAME",
            "COMMIT","FAIL","LIKE","REPLACE",
            "CONFLICT","FOR","LIMIT","RESTRICT",
            "CONSTRAINT","FOREIGN","MATCH","RIGHT"
        };


    }



}
