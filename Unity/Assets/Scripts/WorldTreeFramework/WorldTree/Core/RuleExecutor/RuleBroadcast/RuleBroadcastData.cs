/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 法则广播执行器数据：设计用于编辑器在法则不存在时，也能编辑保存全局事件

*/

namespace WorldTree
{
	/// <summary>
	/// 法则广播执行器数据
	/// </summary>
	public partial class RuleBroadcastData : Unit, RuleBroadcast<IGlobalRule>
	{
		/// <summary>
		/// 法则类型码
		/// </summary>
		public long RuleType;

		public void Clear() { }

		public bool TryAdd(INode node)
		{
			return false;
		}
	}

	public partial class RuleBroadcastData
	{
		class TreeDataSerialize : TreeDataSerializeRule<RuleBroadcastData>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out RuleBroadcastData obj, false)) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.RuleType);
			}
		}
		class TreeDataDeserialize : TreeDataDeserializeRule<RuleBroadcastData>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(RuleBroadcastData), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				long ruleTypeCode = self.ReadValue<long>();
				value = self.Core.GetRuleBroadcast(ruleTypeCode);
			}
		}
	}
}
