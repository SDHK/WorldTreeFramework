using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd = (self) =>
		{
			self.Log($"序列化测试！！");

			// 获取字节缓存写入器
			self.Core.PoolGetUnit(out ByteBufferWriter byteBufferWriter);

			// 获取树形序列化写入器外壳
			TreeSerializeWriter treeSerializeWriter = new(self.Core, byteBufferWriter);

			// 写入基本数值类型
			treeSerializeWriter.SerializeUnmanaged(self.TestFloat);
			treeSerializeWriter.SerializeUnmanaged(self.TestDouble);
			treeSerializeWriter.SerializeUnmanaged(self.TestInt);
			treeSerializeWriter.SerializeUnmanaged(self.TestLong);
			treeSerializeWriter.SerializeUnmanaged(self.TestBool);


			// 写入器转 换获 取字节数组
			byte[] dataBytes = byteBufferWriter.ToArrayAndReset();

			// 获取字节只读序列生成器
			self.Core.PoolGetUnit(out ByteReadOnlySequenceBuilder byteReadOnlySequenceBuilder);

			// 添加字节数组 到读取器 (可多次添加)
			byteReadOnlySequenceBuilder.Add(dataBytes, false);

			// 融合多个数组 生成只读序列
			ReadOnlySequence<byte> readOnlySequence = byteReadOnlySequenceBuilder.Build();
			// 获取树形序列化读取器 外壳
			TreeSerializeRead treeSerializeRead = new (readOnlySequence);

			// 反序列化基本数值类型
			float testFloat = treeSerializeRead.DeserializeUnmanage<float>();
			double testDouble = treeSerializeRead.DeserializeUnmanage<double>();
			int testInt = treeSerializeRead.DeserializeUnmanage<int>();
			long testLong = treeSerializeRead.DeserializeUnmanage<long>();
			bool testBool = treeSerializeRead.DeserializeUnmanage<bool>();

			self.Log($"结果： {testFloat}:{testDouble}:{testInt}:{testLong}:{testBool}");

			byteBufferWriter.Dispose();
			byteReadOnlySequenceBuilder.Dispose();
		};
	}
}
