using System;
using System.Buffers.Binary;
using System.IO;

namespace WorldTree
{

    public static class Serialized
    {
        public static byte[] Get(int value)
        {
            //BinaryPrimitives.WriteInt32BigEndian
            //BinaryPrimitives.ReadInt16BigEndian(,)
            //BitConverter.GetBytes(new object())

            //Type.GetTypeCode
            return BitConverter.GetBytes(value);
        }
    }

    public class SerializedStrategy : Node
    {

        public void GetS<T>(T value, out byte[] bytes)
        {

            bytes = null;

        }

        public void GetFields<T>(T value, out byte[] bytes)
        {

            //数组
            //结构体
            //类型
            //值类型

            //id

            //id ， TypeCore:Length  = int:1|TypeCore:id     (Class)

            //id ， object[]:Length  = TypeCore:id|TypeCore:id|int:1 (Array)

            //id ， <string,obj>:Length  =  string:id|int:1      (Dict)

            //id ， TypeCore:Length  = int:2|TypeCore:id     (Sturt)


            /*
            序列化类型

            单值

            单对象

            对象集

            */


            switch (value)
            {
                case short:
                    break;
                case ushort:
                    break;
                case int:
                    break;
                case null:
                    break;
                default:
                    break;
            }


            bytes = null;

        }
    }

    public interface ISerializedBasicType
    {
        public void Read<T>(byte[] data, out T value);


        public void Write<T>(T value, out byte[] data);


    }

    public class a
    {
        int int1 = 1;
        float float1 = 2f;
    }

    /// <summary>
    /// 序列化基本类型
    /// </summary>
    public class SerializedBasicType : Node, ISerializedBasicType
    {
        public void Read<T>(byte[] data, out T value)
        {
            MemoryStream stream = new();
            var BW = new BinaryWriter(stream);

            var BR = new BinaryReader(stream);
            //BR.Read()
            BW.Write(1);

            throw new NotImplementedException();
        }

        public void Write<T>(T value, out byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
