
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 10:49

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
        /// 获取 Vector2
        /// </summary>
        public static Vector2 GetVector2Data(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetVector2();
        }

        /// <summary>
        /// 获取 Vector2s
        /// </summary>
        public static Vector2[] GetVector2sData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetVector2s();
        }

        /// <summary>
        /// 转为 Vector2
        /// </summary>
        public static Vector2 GetVector2(this byte[] arr)
        {
            float[] floats = arr.GetFloats();
            return new Vector2(floats[0], floats[1]);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this Vector2 value)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(value.x.GetBytes());
            stream.Write(value.y.GetBytes());
            return stream.ToArray();
        }

        /// <summary>
        /// 转为 Vector2[]
        /// </summary>
        public static Vector2[] GetVector2s(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] datas = new byte[8];
            Vector2[] results = new Vector2[arr.Length / datas.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < results.Length; i++)
            {
                stream.Read(datas);
                results[i] = datas.GetVector2();
            }
            return results;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        /// <param name="value"></param>
        public static byte[] GetBytes(this Vector2[] value)
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
        /// 转为 Vector2
        /// </summary>
        public static Vector2 GetVector2(this string str)
        {
            if (str != "" && str != null)
            {
                string[] floats = str.Split('|');
                return new Vector2(float.Parse(floats[0]), float.Parse(floats[1]));
            }
            else
            {
                return Vector2.zero;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this Vector2 value)
        {
            return value.x + "|" + value.y;
        }


        /// <summary>
        /// 转为 Vector2[]
        /// </summary>
        public static Vector2[] GetVector2s(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                Vector2[] results = new Vector2[datas.Length / 2];
                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = (datas[i * 2] + "|" + datas[i * 2 + 1]).GetVector2();
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
        public static string GetExcel(this Vector2[] values)
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
