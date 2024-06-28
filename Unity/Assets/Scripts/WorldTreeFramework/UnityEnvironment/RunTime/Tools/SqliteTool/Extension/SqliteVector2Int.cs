
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 10:56

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace WorldTree.Sample
{
    public static partial class SqliteDataReaderExtension
    {
        /// <summary>
        /// 获取 Vector2Int
        /// </summary>
        public static Vector2Int GetVector2IntData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetVector2Int();
        }

        /// <summary>
        /// 获取 Vector2Ints
        /// </summary>
        public static Vector2Int[] GetVector2IntsData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetVector2Ints();
        }

        /// <summary>
        /// 转为 Vector2Int
        /// </summary>
        public static Vector2Int GetVector2Int(this byte[] arr)
        {
            int[] ints = arr.GetInts();
            return new Vector2Int(ints[0], ints[1]);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this Vector2Int value)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(value.x.GetBytes());
            stream.Write(value.y.GetBytes());
            return stream.ToArray();
        }

        /// <summary>
        /// 转为 Vector2[]
        /// </summary>
        public static Vector2Int[] GetVector2Ints(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] datas = new byte[8];
            Vector2Int[] results = new Vector2Int[arr.Length / datas.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < results.Length; i++)
            {
                stream.Read(datas);
                results[i] = datas.GetVector2Int();
            }
            return results;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        /// <param name="value"></param>
        public static byte[] GetBytes(this Vector2Int[] value)
        {
            if (value == null) return null;
            MemoryStream stream = new MemoryStream();
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i].GetBytes());
            }
            return stream.ToArray();
        }


        /// <summary>
        /// 转为 Vector2Int
        /// </summary>
        public static Vector2Int GetVector2Int(this string str)
        {
            if (str != "" && str != null)
            {
                string[] ints = str.Split('|');
                return new Vector2Int(int.Parse(ints[0]), int.Parse(ints[1]));
            }
            else
            {
                return Vector2Int.zero;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this Vector2Int value)
        {
            return value.x + "|" + value.y;
        }

        /// <summary>
        /// 转为 Vector2Int[]
        /// </summary>
        public static Vector2Int[] GetVector2Ints(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                Vector2Int[] results = new Vector2Int[datas.Length / 2];
                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = (datas[i * 2] + "|" + datas[i * 2 + 1]).GetVector2Int();
                }
                return results;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this Vector2Int[] values)
        {
            string str = "";
            for (int i = 0; i < values.Length; i++)
            {
                str += values[i].GetExcel();
                if (i < values.Length - 1)
                {
                    str += "|";
                }
            }
            return str;
        }
    }
}