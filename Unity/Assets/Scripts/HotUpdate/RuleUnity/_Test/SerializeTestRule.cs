using System;

namespace WorldTree
{
	public static partial class SerializeTestFormatRule
	{
		class SerializeTestRule : SerializeRule<ByteSequence, SerializeTest>
		{
			protected override void Execute(ByteSequence self, ref SerializeTest arg1)
			{
				self.Write(7);
				self.Write(arg1.TestFloat);
				self.WriteDynamic(arg1.TestInt);
				self.WriteDynamic(arg1.TestLong);
				self.Write(arg1.TestDouble);
				self.Write(arg1.TestBool);
				self.Write(arg1.nodeDataTest);
			}
		}

		class DeserializeTestRule : DeserializeRule<ByteSequence, SerializeTest>
		{
			protected override void Execute(ByteSequence self, ref SerializeTest arg1)
			{
				if (arg1 == null) self.Core.PoolGetNode(out arg1);

				self.Read(out int count);
				self.Read(out arg1.TestFloat);
				self.ReadDynamic(out arg1.TestInt);
				self.ReadDynamic(out arg1.TestLong);
				self.Read(out arg1.TestDouble);
				self.Read(out arg1.TestBool);
				self.Read(out arg1.nodeDataTest);
			}
		}
	}


	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd = (self) =>
		{
			self.Log($"序列化测试！！！！！");
			self.TestFloat = 5.789456f;
			self.TestInt = 798456;
			self.TestLong = 456123;
			self.TestDouble = 123.456789;
			self.TestBool = true;
			self.nodeDataTest.Age1 = 123456;

			// 获取字节缓存写入器
			self.AddTemp(out ByteSequence sequenceWrite);

			// 序列化法则
			if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<Serialize<SerializeTest>> ruleGroup1))
				ruleGroup1.TrySendRef(sequenceWrite, ref self);

			byte[] bytes = sequenceWrite.ToBytes();
			self.AddTemp(out ByteSequence sequenceRead).SetBytes(bytes);

			// 反序列化法则
			SerializeTest st1 = null;
			if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<Deserialize<SerializeTest>> ruleGroup))
				ruleGroup.TrySendRef(sequenceRead, ref st1);


			self.Log($"测试结果： {st1.TestFloat}:{st1.TestInt}:{st1.TestLong}:{st1.TestDouble}:{st1.TestBool}:{st1.nodeDataTest.Age1}");
		};
	}
}
