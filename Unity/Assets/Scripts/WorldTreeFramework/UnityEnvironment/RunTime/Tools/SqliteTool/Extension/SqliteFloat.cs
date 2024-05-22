
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 17:35

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
        /// 获取 float
        /// </summary>
        public static float GetFloatData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetFloat();
        }
        /// <summary>
        /// 获取 float[]
        /// </summary>
        public static float[] GetFloatsData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetFloats();
        }
        /// <summary>
        /// 转为 Float
        /// </summary>
        public static float GetFloat(this byte[] arr)
        {
            return BitConverter.ToSingle(arr, 0);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }


        /// <summary>
        /// 转为 Float[]
        /// </summary>
        public static float[] GetFloats(this byte[] arr)
        {

            if (arr == null) return null;
            byte[] data = new byte[4];
            float[] result = new float[arr.Length / data.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < result.Length; i++)
            {
                stream.Read(data);
                result[i] = data.GetFloat();
            }
            return result;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this float[] value)
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
        /// 转为 float
        /// </summary>
        public static float GetFloat(this string str)
        {
            if (str != "" && str != null)
            {
                return float.Parse(str);
            }
            else
            {
                return default(float);
            }

        }

        /// <summary>
        /// 转为 float[]
        /// </summary>
        public static float[] GetFloats(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => float.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this float[] values)
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
