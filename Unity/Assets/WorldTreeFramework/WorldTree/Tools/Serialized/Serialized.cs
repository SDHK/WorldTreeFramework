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



			//数据id ， CRC64类型码，数据长度 = 类型码,数据长度：数据byte | 类型码,数据长度：数据id指针 | ....

			//序列化类型标记3

			//根节点数据DateId

			//DateId ， TypeCore:Length  = int,4:1|TypeCore,Length:DateId     (对象情况)

			//DateId ， object[]:Length  = TypeCore,Length:DateId|TypeCore,Length:DateId|int,4:1 (数组情况)

			//DateId ， <string,obj>:Length  =  string,Length:DateId|int,4:1      (字典情况)



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
