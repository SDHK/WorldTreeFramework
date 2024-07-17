using System.Buffers;

namespace WorldTree
{
	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd = (self) =>
		{
			self.Log($"序列化测试！！");

			// 获取字节缓存写入器
			self.AddTemp(out ByteSequenceWriter byteBufferWriter);

			// 写入基本数值类型
			byteBufferWriter.Serialize(self.TestFloat);
			byteBufferWriter.Serialize(self.TestDouble);
			byteBufferWriter.Serialize(self.TestInt);
			byteBufferWriter.Serialize(self.TestLong);
			byteBufferWriter.Serialize(self.TestBool);

			// 写入器转 换获 取字节数组
			byte[] dataBytes = byteBufferWriter.ToArrayAndReset();

			// 获取字节只读序列生成器
			self.AddTemp(out ByteReadOnlySequenceBuilder byteReadOnlySequenceBuilder);
			// 添加字节数组 到读取器 (可多次添加)
			byteReadOnlySequenceBuilder.Add(dataBytes, false);
			ReadOnlySequence<byte> readOnlySequence =  byteReadOnlySequenceBuilder.Build();

			self.AddTemp(out ByteSequenceReader byteSequenceReader);
			byteSequenceReader.SetReadOnlySequence(readOnlySequence);

			// 反序列化基本数值类型
			float testFloat = byteSequenceReader.Deserialize<float>();
			double testDouble = byteSequenceReader.Deserialize<double>();
			int testInt = byteSequenceReader.Deserialize<int>();
			long testLong = byteSequenceReader.Deserialize<long>();
			bool testBool = byteSequenceReader.Deserialize<bool>();

			self.Log($"结果： {testFloat}:{testDouble}:{testInt}:{testLong}:{testBool}");
		};
	}
}
