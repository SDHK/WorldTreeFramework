using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using static Sirenix.OdinInspector.Editor.UnityPropertyEmitter;

namespace WorldTree
{

	public static class Serialized
	{
		public static byte[] Get(object value)
		{

		 object.ReferenceEquals(value, null);
			//循环引用问题
			//先拿出所有引用类型,存入字典<object,long>
			//然后在对每个引用类型遍历序列化，遇到引用类型就跳过，遇到值类型就序列化

			//当前类型
			(Type, object) current;

			Stack<(Type, object)> stack = new();

			stack.Push((value.GetType(),value));

			while (stack.Count != 0)
			{
				current = stack.Pop();

				//处理类型转为byte[]
				Handle(current);

				//反射获取类型
				Type type = current.Item1;

				//反射获取字段
				var fields = type.GetFields();
				foreach (var field in fields)
				{
					stack.Push((field.FieldType,field.GetValue(current)));
				}

				//反射获取属性
				var properties = type.GetProperties();
				foreach (var property in properties)
				{
					stack.Push((property.PropertyType,property.GetValue(current)));
				}
			}


			void Handle((Type, object) value)
			{
				//value.Item1.IsValueType
			}

			return null;
		}


		public static byte[] Get(int value)
		{
			//BinaryPrimitives.WriteInt32BigEndian
			//BinaryPrimitives.ReadInt16BigEndian(,)
			//BitConverter.GetBytes(new object())

			//Type.GetTypeCode
			return BitConverter.GetBytes(value);
		}

		public static byte[] Get(short value)
		{
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

			// 数据长度| 类型编号：类型名称

			// 类型编号：类型长度
			// 实例ID：类型编号|数据


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
		public void Read<T>(byte[] data, out T value)//反射序列化成二进制
		{
			MemoryStream stream = new();
			var BW = new BinaryWriter(stream);

			var BR = new BinaryReader(stream);
			//BR.Read()
			BW.Write(1);

			throw new NotImplementedException();
		}

		public void Write<T>(T value, out byte[] data)//二进制反序列成对象
		{
			throw new NotImplementedException();
		}
	}


}
