namespace WorldTree
{
	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd = (self) =>
		{
			self.Log($"序列化测试！！");

			// 获取字节缓存写入器
			self.AddTemp(out ByteSequenceWriter byteBufferWriter);

			// 获取字节只读序列生成器
			self.AddTemp(out ByteSequenceReader byteReadOnlySequenceBuilder);



			// 获取序列化写入器
			UnmanagedSerialize serialize = new(self.Core, byteBufferWriter);

			// 写入基本数值类型
			serialize.Serialize(self.TestFloat);
			serialize.Serialize(self.TestDouble);
			serialize.Serialize(self.TestInt);
			serialize.Serialize(self.TestLong);
			serialize.Serialize(self.TestBool);

			// 写入器转 换获 取字节数组
			byte[] dataBytes = byteBufferWriter.ToArrayAndReset();

			// 添加字节数组 到读取器 (可多次添加)
			byteReadOnlySequenceBuilder.Add(dataBytes, false);

			// 获取反序列化器
			UnmanagedDeSerialize deSerialize = new (byteReadOnlySequenceBuilder.Build());

			// 反序列化基本数值类型
			float testFloat = deSerialize.Deserialize<float>();
			double testDouble = deSerialize.Deserialize<double>();
			int testInt = deSerialize.Deserialize<int>();
			long testLong = deSerialize.Deserialize<long>();
			bool testBool = deSerialize.Deserialize<bool>();

			self.Log($"结果： {testFloat}:{testDouble}:{testInt}:{testLong}:{testBool}");
		};
	}
}
