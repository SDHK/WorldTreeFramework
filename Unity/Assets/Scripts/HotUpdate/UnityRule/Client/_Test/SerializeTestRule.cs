using System;

namespace WorldTree
{
	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd = (self) =>
		{
			int? a = null;

			self.Log($"序列化测试！！！！！");

			NodeClassDataTest<int, float> testData = new();
			testData.TestFloat = 5.789456f;
			testData.TestInt = 798456;
			testData.TestLong = 456123;
			testData.TestDouble = 123.456789;
			testData.TestBool = true;
			//testData.ValueT1 = 987;
			testData.ValueT2 = 45.321f;

			// 获取字节缓存写入器
			self.AddTemp(out TreePackByteSequence sequenceWrite).Serialize(testData);

			byte[] bytes = sequenceWrite.ToBytes();
			self.Log($"序列化{bytes.Length}");

			self.AddTemp(out TreePackByteSequence sequenceRead).SetBytes(bytes);

			NodeClassDataTest<int, float> testData2 = null;
			// 反序列化法则
			sequenceRead.Deserialize(ref testData2);

			self.Log($"测试结果： {testData2.TestFloat}:{testData2.TestInt}:{testData2.TestLong}" +
				$":{testData2.TestDouble}:{testData2.TestBool}:{testData2.ValueT1}:{testData2.ValueT2}");
		};
	}
}
