/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 泛型全局单法则执行器

*/

namespace WorldTree
{
	/// <summary>
	/// 全局法则执行器数据
	/// </summary>
	public partial class GlobalRuleExecutorData : Unit, IRuleExecutor<IRule>
	{
		/// <summary>
		/// 法则类型码
		/// </summary>
		public long RuleTypeCode;
	}

	public partial class GlobalRuleExecutorData
	{
		class TreeDataSerialize : TreeDataSerializeRule<GlobalRuleExecutorData>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out GlobalRuleExecutorData obj, false)) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.RuleTypeCode);
			}
		}
		class TreeDataDeserialize : TreeDataDeserializeRule<GlobalRuleExecutorData>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(GlobalRuleExecutorData), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				long ruleTypeCode = self.ReadValue<long>();
				value = self.Core.GetGlobalRuleExecutor(ruleTypeCode);
			}
		}
	}
}
