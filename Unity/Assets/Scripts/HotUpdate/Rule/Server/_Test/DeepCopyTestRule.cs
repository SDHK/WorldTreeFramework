namespace WorldTree.Server
{

	public static partial class DeepCopyTestRule
	{
		[NodeRule(nameof(AddRule<DeepCopyTest>))]
		private static void OnAdd(this DeepCopyTest self)
		{
			CopyTest1 copyTest1 = new CopyTest1();
			copyTest1.Value2 = new CopyTestStruct1()
			{
				Value1 = 123,
				Value2 = 456f,
				Value3 = "789"
			};

			CopyTestDict1 testDict = new CopyTestDict1();



			copyTest1.ValueDict = testDict;

			testDict.Value1 = 123987;
			testDict.Value11 = "字典子类";
			testDict.Add(1, 100);
			testDict.Add(2, 200);

			self.AddTemp(out TreeCopyExecutor treeCopy);
			CopyTest1 copyTest2 = null;
			treeCopy.CloneObject(copyTest1, ref copyTest2);

			self.Log($"copyTest1.Value2.Value1 = {copyTest1.Value2.Value2}");
			self.Log($"copyTest2.Value2.Value1 = {copyTest2.Value2.Value2}");


			self.Log($"字典深拷贝验证 {copyTest1.ValueDict == copyTest2.ValueDict}");
			self.Log($"copyTest2.ValueDict.Value11 = {((CopyTestDict1)copyTest2.ValueDict).Value11}");
		}
	}
}