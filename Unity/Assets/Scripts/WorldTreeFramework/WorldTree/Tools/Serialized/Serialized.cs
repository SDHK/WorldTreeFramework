using System;
using System.Collections.Generic;
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 序列化工具类
	/// </summary>
	public static class Serialized
	{

		//public static byte[] Get(int value)
		//{
		//	//BinaryPrimitives.WriteInt32BigEndian
		//	//BinaryPrimitives.ReadInt16BigEndian(,)
		//	//BitConverter.GetBytes(new object())

		//	//Type.GetTypeCode
		//	return BitConverter.GetBytes(value);
		//}

		//public static byte[] Get(short value)
		//{
		//	return BitConverter.GetBytes(value);
		//}
	}

	/// <summary>
	/// 序列化策略
	/// </summary>
	public class SerializedStrategy : Node
	{

		/// <summary>
		/// a
		/// </summary>
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

			// 数据长度| 类型编号：类型名称

			// 类型编号：类型长度
			// 实例ID：类型编号|数据

			bytes = null;

		}
	}

	/// <summary>
	/// 序列化基本类型
	/// </summary>
	public interface ISerializedBasicType
	{
		/// <summary>
		/// 读取
		/// </summary>
		public void Read<T>(byte[] data, out T value);
		/// <summary>
		/// 写入
		/// </summary>
		public void Write<T>(T value, out byte[] data);
	}



	/// <summary>
	/// 序列化基本类型
	/// </summary>
	public class SerializedBasicType : Node, ISerializedBasicType
	{
		public void Read<T>(byte[] data, out T value)//反射序列化成二进制
		{
			MemoryStream stream = new();
			var bW = new BinaryWriter(stream);

			var bR = new BinaryReader(stream);
			//BR.Read()
			bW.Write(1);

			throw new NotImplementedException();
		}

		public void Write<T>(T value, out byte[] data)//二进制反序列成对象
		{
			throw new NotImplementedException();
		}
	}


}
