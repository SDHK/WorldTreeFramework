using System.Buffers;

namespace WorldTree
{
	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd1 = (self) =>
		{
			self.Log($"序列化测试！！！！！");

			// 获取字节缓存写入器
			self.AddTemp(out ByteSequence sequenceWrite);

			// 写入基本数值类型
			sequenceWrite.Write(self.TestFloat);
			sequenceWrite.WriteDynamic(self.TestInt);
			sequenceWrite.WriteDynamic(self.TestLong);
			sequenceWrite.Write(self.TestDouble);
			sequenceWrite.Write(self.TestBool);

			byte[] bytes = sequenceWrite.ToBytes();

			self.AddTemp(out ByteSequence sequenceRead).SetBytes(bytes);

			// 读取基本数值类型
			float testFloat = sequenceRead.Read(out float _);
			int testInt = sequenceRead.ReadDynamic(out int _);
			long testLong = sequenceRead.ReadDynamic(out long _);
			double testDouble = sequenceRead.Read(out double _);
			bool testBool = sequenceRead.Read(out bool _);
			self.Log($"测试结果： {testFloat}:{testInt}:{testLong}:{testDouble}:{testBool}");
		};
	}
}
