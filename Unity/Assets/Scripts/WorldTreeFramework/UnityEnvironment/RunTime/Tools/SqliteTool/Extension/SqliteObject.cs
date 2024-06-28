
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/28 18:17

* 描述： 

*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree.Sample
{
    public static partial class SqliteDataReaderExtension
    {

        /// <summary>
        /// 获取 Object
        /// </summary>
        public static object GetObjectData(this SqliteDataReader reader, string name)
        {
            return reader.GetBytesData(name).GetObject();
        }

        /// <summary>
        /// 转为 Object
        /// </summary>
        public static object GetObject(this byte[] arr)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(arr, 0, arr.Length);
            stream.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(stream);
            stream.Close();
            return obj;
        }

        /// <summary>
        /// 转为 byte[] :对象定义时需[Serializable]序列化
        /// </summary>
        public static byte[] GetBytes(this object value)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, value);
            byte[] datas = stream.ToArray();
            stream.Close();
            return datas;
        }
    }
}