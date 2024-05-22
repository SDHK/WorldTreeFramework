
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 10:53

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
        /// 获取 Vector3Int
        /// </summary>
        public static Vector3Int GetVector3IntData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetVector3Int();
        }

        /// <summary>
        /// 获取 Vector3Ints
        /// </summary>
        public static Vector3Int[] GetVector3IntsData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetVector3Ints();
        }

        /// <summary>
        /// 转为 Vector3Int
        /// </summary>
        public static Vector3Int GetVector3Int(this byte[] arr)
        {
            int[] Ints = arr.GetInts();
            return new Vector3Int(Ints[0], Ints[1], Ints[2]);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this Vector3Int value)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(value.x.GetBytes());
            stream.Write(value.y.GetBytes());
            stream.Write(value.z.GetBytes());
            return stream.ToArray();
        }

        /// <summary>
        /// 转为 Vector3Int[]
        /// </summary>
        public static Vector3Int[] GetVector3Ints(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] data = new byte[12];
            Vector3Int[] result = new Vector3Int[arr.Length / data.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < result.Length; i++)
            {
                stream.Read(data);
                result[i] = data.GetVector3Int();
            }
            return result;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        /// <param name="value"></param>
        public static byte[] GetBytes(this Vector3Int[] value)
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
        /// 转为 Vector3Int
        /// </summary>
        public static Vector3Int GetVector3Int(this string str)
        {
            if (str != "" && str != null)
            {
                string[] Ints = str.Split('|');
                return new Vector3Int(int.Parse(Ints[0]), int.Parse(Ints[1]), int.Parse(Ints[2]));
            }
            else
            {
                return Vector3Int.zero;
            }
        }


        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this Vector3Int value)
        {
            return value.x + "|" + value.y + "|" + value.z;
        }

        /// <summary>
        /// 转为 Vector3Int[]
        /// </summary>
        public static Vector3Int[] GetVector3Ints(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                Vector3Int[] result = new Vector3Int[datas.Length / 3];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (datas[i * 3] + "|" + datas[i * 3 + 1] + "|" + datas[i * 3 + 2]).GetVector3Int();
                }
                return result ;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this Vector3Int[] values)
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
