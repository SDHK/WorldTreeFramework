using System;

namespace WorldTree
{
	public static partial class SerializeTestRule
	{
		static OnAdd<SerializeTest> OnAdd = (self) =>
		{
			self.Log($"序列化测试！！！！！");

			// 动态支持法则
			self.Core.RuleManager.SupportGenericRule(TypeInfo<NodeClassDataTest<int>>.TypeCode);


			NodeClassDataTest<int> testData = new();
			testData.TestFloat = 5.789456f;
			testData.TestInt = 798456;
			testData.TestLong = 456123;
			testData.TestDouble = 123.456789;
			testData.TestBool = true;
			testData.ValueT = 987;

			// 获取字节缓存写入器
			self.AddTemp(out ByteSequence sequenceWrite);

			// 序列化法则
			if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<Serialize<NodeClassDataTest<int>>> ruleGroup1))
				ruleGroup1.TrySendRef(sequenceWrite, ref testData);

			byte[] bytes = sequenceWrite.ToBytes();
			self.AddTemp(out ByteSequence sequenceRead).SetBytes(bytes);

			// 反序列化法则
			NodeClassDataTest<int> testData2 = null;
			if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<Deserialize<NodeClassDataTest<int>>> ruleGroup))
				ruleGroup.TrySendRef(sequenceRead, ref testData2);


			self.Log($"测试结果： {testData2.TestFloat}:{testData2.TestInt}:{testData2.TestLong}:{testData2.TestDouble}:{testData2.TestBool}:{testData2.ValueT}");
		};
	}
}
