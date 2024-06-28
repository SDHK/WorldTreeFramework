
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 17:38

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree.Sample
{
    public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取 Short
        /// </summary>
        public static short GetShortData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetShort();
        }

        /// <summary>
        /// 获取 Short[]
        /// </summary>
        public static short[] GetShortsData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetShorts();
        }

        /// <summary>
        /// 转为 Short
        /// </summary>
        public static short GetShort(this byte[] arr)
        {
            return BitConverter.ToInt16(arr, 0);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }


        /// <summary>
        /// 转为 Short[]
        /// </summary>
        public static short[] GetShorts(this byte[] arr)
        {

            if (arr == null) return null;
            byte[] datas = new byte[2];
            short[] results = new short[arr.Length / datas.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < results.Length; i++)
            {
                stream.Read(datas);
                results[i] = datas.GetShort();
            }
            return results;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this short[] value)
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
        /// 转为 short
        /// </summary>
        public static short GetShort(this string str)
        {
            if (str != "" && str != null)
            {
                return short.Parse(str);
            }
            else
            {
                return default(short);
            }

        }
        /// <summary>
        /// 转为 short[]
        /// </summary>
        public static short[] GetShorts(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => short.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this short[] values)
        {
            string str = "";
            for (int i = 0; i < values.Length; i++)
            {
                str += values[i];
                if (i < values.Length - 1)
                {
                    str += "|";
                }
            }
            return str;
        }

    }
}
