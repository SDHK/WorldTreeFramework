
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 17:42

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
        /// 获取 Double
        /// </summary>
        public static double GetDoubleData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetDouble();
        }

        /// <summary>
        /// 获取 Double[]
        /// </summary>
        public static double[] GetDoublesData(this SqliteDataReader reader, string name)
        {

            return reader.GetBytesData(name).GetDoubles();
        }

        /// <summary>
        /// 转为 Double
        /// </summary>
        public static double GetDouble(this byte[] arr)
        {
            return BitConverter.ToDouble(arr, 0);
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this double value)
        {
            return BitConverter.GetBytes(value);
        }



        /// <summary>
        /// 转为 Double[]
        /// </summary>
        public static double[] GetDoubles(this byte[] arr)
        {

            if (arr == null) return null;
            byte[] datas = new byte[8];
            double[] results = new double[arr.Length / datas.Length];
            MemoryStream stream = new MemoryStream(arr);

            for (int i = 0; i < results.Length; i++)
            {
                stream.Read(datas);
                results[i] = datas.GetDouble();
            }
            return results;
        }

        /// <summary>
        /// 转为 byte
        /// </summary>
        public static byte[] GetBytes(this double[] value)
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
        /// 转为 double
        /// </summary>
        public static double GetDouble(this string str)
        {
            if (str != "" && str != null)
            {
                return Convert.ToDouble(str);
            }
            else
            {
                return default(double);
            }
        }

        /// <summary>
        /// 转为 double[]
        /// </summary>
        public static double[] GetDoubles(this string str)
        {
            if (str != "" && str != null)
            {
                string[] datas = str.Split('|');
                return datas.Select((data) => double.Parse(data)).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转为 String
        /// </summary>
        public static string GetExcel(this double[] values)
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
