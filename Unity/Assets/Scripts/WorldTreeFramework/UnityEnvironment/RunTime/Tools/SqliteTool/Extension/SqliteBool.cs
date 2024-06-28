
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 17:59

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
	/// <summary>
	/// SqliteDataReader扩展
	/// </summary>
	public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取 Bool
        /// </summary>
        public static bool GetBoolData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetBool();
        }

        /// <summary>
        /// 获取 Bools
        /// </summary>
        public static bool[] GetBoolsData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetBools();
        }

        /// <summary>
        /// 转为 Bool
        /// </summary>
        public static bool GetBool(this byte[] arr)
        {
            return BitConverter.ToBoolean(arr);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this bool value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 转为 Bool[]
        /// </summary>
        public static bool[] GetBools(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] datas = new byte[1];
            bool[] results = new bool[arr.Length / datas.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < results.Length; i++)
            {
                stream.Read(datas);
                results[i] = datas.GetBool();
            }
            return results;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        /// <param name="value"></param>
        public static byte[] GetBytes(this bool[] value)
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
        /// 转为 Bool
        /// </summary>
        public static bool GetBool(this string str)
        {
            if (str != "" && str != null)
            {
                return Convert.ToBoolean(str);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 转为 Bool[]
        /// </summary>
        public static bool[] GetBools(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => bool.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this bool[] values)
        {
            string str = "";
            for (int i = 0; i < values.Length; i++)
            {
                str += values[i];
                if (i< values.Length-1)
                {
                    str += "|";
                }
            }
            return str;
        }


    }
}