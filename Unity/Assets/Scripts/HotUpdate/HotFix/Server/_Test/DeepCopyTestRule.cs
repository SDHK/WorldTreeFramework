namespace WorldTree.Server
{

	public static partial class DeepCopyTestRule
	{
		[NodeRule(nameof(AddRule<DeepCopyTest>))]
		private static void OnAddRule(this DeepCopyTest self)
		{
			CopyTest copySource = new CopyTest();
			copySource.CopyA = new CopyTestA();
			copySource.CopyARef = copySource.CopyA;
			copySource.CopyA.CopyTestB = new CopyTestB();
			copySource.CopyA.CopyTestB.ValueString = "测试字符串";


			CopyTestDict1 testDict = new CopyTestDict1();

			copySource.ValueDict = testDict;

			testDict.Value1 = 123987;
			testDict.Value11 = "字典子类";
			testDict.Add(1, 100);
			testDict.Add(2, 200);

			self.AddTemp(out TreeCopier treeCopy);
			CopyTest copyTarget = null;
			treeCopy.CopyTo(copySource, ref copyTarget);


			self.Log($"对比字段A {copySource.CopyA == copyTarget.CopyA}");
			self.Log($"对比字段ARef {copySource.CopyARef == copyTarget.CopyARef}");

			self.Log($"原类型引用比较 {copySource.CopyA == copySource.CopyARef}");
			self.Log($"目标类型还原引用 {copyTarget.CopyA == copyTarget.CopyARef}");
		}
	}
}