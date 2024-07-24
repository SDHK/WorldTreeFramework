using System;

namespace WorldTree
{
	public static partial class SerializeTestFormatRule
	{
		class SerializeTestRule : SerializeRule<ByteSequence, SerializeTest>
		{
			protected override void Execute(ByteSequence self, SerializeTest arg1)
			{
				self.Write(6);
				self.Write(arg1.TestFloat);
				self.Write(arg1.TestInt);
				self.Write(arg1.TestDouble);
				self.Write(arg1.TestLong);
				self.Write(arg1.TestDouble);
				self.Write(arg1.TestBool);
			}
		}

		class DeserializeTestRule : DeserializeRule<ByteSequence, SerializeTest>
		{
			protected override void Execute(ByteSequence self, SerializeTest arg1)
			{
				self.Read(out int count);
				self.Read(out arg1.TestFloat);
				self.Read(out arg1.TestInt);
				self.Read(out arg1.TestDouble);
				self.Read(out arg1.TestLong);
				self.Read(out arg1.TestDouble);
				self.Read(out arg1.TestBool);

				//new SerializeTest();
			}
		}
	}






	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd = (self) =>
		{
			self.Log($"序列化测试！！！！！");

			// 获取字节缓存写入器
			self.AddTemp(out ByteSequence sequenceWrite);


			// 写入基本数值类型
			sequenceWrite.Write(self.TestFloat);
			sequenceWrite.WriteDynamic(self.TestInt);
			sequenceWrite.WriteDynamic(self.TestLong);
			sequenceWrite.Write(self.TestDouble);
			sequenceWrite.Write(self.nodeDataTest);
			sequenceWrite.Write(self.TestBool);


			byte[] bytes = sequenceWrite.ToBytes();

			self.AddTemp(out ByteSequence sequenceRead).SetBytes(bytes);

			// 读取基本数值类型
			float testFloat = sequenceRead.Read(out float _);
			int testInt = sequenceRead.ReadDynamic(out int _);
			long testLong = sequenceRead.ReadDynamic(out long _);
			double testDouble = sequenceRead.Read(out double _);
			NodeDataTest nodeDataTest = sequenceRead.Read(out NodeDataTest _);
			bool testBool = sequenceRead.Read(out bool _);

			self.Log($"测试结果： {testFloat}:{testInt}:{testLong}:{testDouble}:{testBool}:{nodeDataTest.Age1}");


			//NodeRuleHelper.SendRule(sequenceWrite, TypeInfo<Serialize<SerializeTest>>.Default, self);

		};


	}
}
