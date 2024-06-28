
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 18:14

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
        /// 获取 Guid
        /// </summary>
        public static Guid GetGuidData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetGuid();
        }

        /// <summary>
        /// 获取 Guid[]
        /// </summary>
        public static Guid[] GetGuidsData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetGuids();
        }

        /// <summary>
        /// 转为 Guid
        /// </summary>
        public static Guid GetGuid(this byte[] arr)
        {
            return new Guid(arr);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this Guid value)
        {
            return value.ToByteArray();
        }


        /// <summary>
        /// 转为 Guid[]
        /// </summary>
        public static Guid[] GetGuids(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] datas = new byte[16];
            Guid[] results = new Guid[arr.Length / datas.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < results.Length; i++)
            {
                stream.Read(datas);
                results[i] = datas.GetGuid();
            }
            return results;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this Guid[] value)
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
        /// 转为 Guid
        /// </summary>
        public static Guid GetGuid(this string str)
        {
            if (str != "" && str != null)
            {
                return Guid.Parse(str);
            }
            else
            {
                return default(Guid);
            }

        }

        /// <summary>
        /// 转为 Guid[]
        /// </summary>
        public static Guid[] GetGuids(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => Guid.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this Guid[] values)
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