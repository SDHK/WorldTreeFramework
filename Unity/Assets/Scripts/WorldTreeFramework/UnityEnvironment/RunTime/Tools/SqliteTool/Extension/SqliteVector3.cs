
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:56

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
        /// 获取 Vector3
        /// </summary>
        public static Vector3 GetVector3Data(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetVector3();
        }

        /// <summary>
        /// 获取 Vector3s
        /// </summary>
        public static Vector3[] GetVector3sData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetVector3s();
        }

        /// <summary>
        /// 转为 Vector3
        /// </summary>
        public static Vector3 GetVector3(this byte[] arr)
        {
            float[] floats = arr.GetFloats();
            return new Vector3(floats[0], floats[1], floats[2]);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this Vector3 value)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(value.x.GetBytes());
            stream.Write(value.y.GetBytes());
            stream.Write(value.z.GetBytes());
            return stream.ToArray();
        }

        /// <summary>
        /// 转为 Vector3[]
        /// </summary>
        public static Vector3[] GetVector3s(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] datas = new byte[12];
            Vector3[] results = new Vector3[arr.Length / datas.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < results.Length; i++)
            {
                stream.Read(datas);
                results[i] = datas.GetVector3();
            }
            return results;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this Vector3[] value)
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
        /// 转为 Vector3
        /// </summary>
        public static Vector3 GetVector3(this string str)
        {
            if (str != "" && str != null)
            {
                string[] floats = str.Split('|');
                return new Vector3(float.Parse(floats[0]), float.Parse(floats[1]), float.Parse(floats[2]));
            }
            else
            {
                return Vector3.zero;
            }
        }


        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this Vector3 value)
        {
            return value.x + "|" + value.y + "|" + value.z;
        }

        /// <summary>
        /// 转为 Vector3[]
        /// </summary>
        public static Vector3[] GetVector3s(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                Vector3[] results = new Vector3[datas.Length / 3];
                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = (datas[i * 3] + "|" + datas[i * 3 + 1] + "|" + datas[i * 3 + 2]).GetVector3();
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
        public static string GetExcel(this Vector3[] values)
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
