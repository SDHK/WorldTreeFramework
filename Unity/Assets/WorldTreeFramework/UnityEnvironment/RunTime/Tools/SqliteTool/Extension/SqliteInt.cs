
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 17:33

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork
{
    public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取int
        /// </summary>
        public static int GetIntData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetInt();
        }

        /// <summary>
        /// 获取int[]
        /// </summary>
        public static int[] GetIntsData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetInts();
        }

        /// <summary>
        /// 转为 int 
        /// </summary>
        public static int GetInt(this byte[] arr)
        {
            if (arr == null) return default(int);
            return BitConverter.ToInt32(arr, 0);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        /// <param name="value"></param>
        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }



        /// <summary>
        /// 转为int[]
        /// </summary>
        public static int[] GetInts(this byte[] arr)
        {
            if (arr == null) return null;
            byte[] data = new byte[4];
            int[] result = new int[arr.Length / data.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < result.Length; i++)
            {
                stream.Read(data);
                result[i] = data.GetInt();
            }
            return result;
        }


        /// <summary>
        /// 转为 byte
        /// </summary>
        /// <param name="value"></param>
        public static byte[] GetBytes(this int[] value)
        {
            if(value==null)return null;
            MemoryStream stream = new MemoryStream();
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i].GetBytes());
            }
            return stream.ToArray();
        }




        /// <summary>
        /// 转为 int
        /// </summary>
        public static int GetInt(this string str)
        {
            if (str != "" && str != null)
            {
                return int.Parse(str);
            }
            else
            {
                return default(int);
            }
        }

        /// <summary>
        /// 转为 int[]
        /// </summary>
        public static int[] GetInts(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => int.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this int[] values)
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
