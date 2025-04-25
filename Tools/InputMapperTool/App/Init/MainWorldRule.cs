/****************************************

* 作者：闪电黑客
* 日期：2025/4/22 18:28

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 测试枚举
	/// </summary>
	public enum TestEnum
	{
		/// <summary>
		/// 测试1
		/// </summary>
		Test1,
		/// <summary>
		/// 测试2
		/// </summary>
		Test2,
	}

	/// <summary>
	/// a
	/// </summary>
	public interface TestNodeEvent : ISendRule<TestEnum>
	{
	}

	/// <summary>
	/// 测试
	/// </summary>
	public class Test : Node
		, AsTestNodeEvent
	{
		/// <summary>
		/// 字段
		/// </summary>
		public int ConfigId;

		/// <summary>
		/// 属性
		/// </summary>
		public long ConfigName => ConfigId;
	}

	public partial class TestRule
	{
		[RuleSwitch(nameof(Test.ConfigName), 1011)]
		partial class AddSub : TestNodeEventRule<Test>
		{
			protected override void Execute(Test self, TestEnum id)
			{
			}
		}

		[RuleSwitch(nameof(Test.ConfigName), 1012)]
		static OnTestNodeEvent<Test> AddSub1 = (self, id) =>
		{
		};
	}

	/// <summary>
	/// 代码自动生成 静态部分类型 
	/// </summary>
	public partial class TestRule
	{
		partial class RuleSwitch_Test_ConfigName_TestNodeEventRule_Test_ : TestNodeEventRule<Test>
		{
			static AddSub _AddSub = new();
			protected override void Execute(Test self, TestEnum id)
			{
				switch (self.ConfigName)
				{
					case 1011L: _AddSub.Invoke(self, id); break;
					case 1012L: AddSub1.Invoke(self, id); break;
				}
			}
		}
	}
}
