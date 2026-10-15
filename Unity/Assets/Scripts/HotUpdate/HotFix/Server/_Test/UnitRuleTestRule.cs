/****************************************

* 作者：闪电黑客
* 日期：2026/10/15

* 描述：

*/
namespace WorldTree.Server
{
	/// <summary>
	/// 纯单位法则测试
	/// </summary>
	public static partial class UnitRuleTestRule
	{
		/// <summary>
		/// 取出单位并执行法则
		/// </summary>
		[NodeRule(nameof(AddRule<UnitRuleTest>))]
		private static void OnAddRule(this UnitRuleTest self)
		{
			self.World.PoolGetUnit(out UnitRuleSample unit);
			unit.UnitEcho(7);
			int sum = unit.UnitAdd(3, 4);
			bool missing = NodeRuleHelper.TrySendRule(unit, default(UnitMissing), 1);
			bool pass = unit.Echo == 7 && sum == 7 && missing == false;
			self.Log($"UnitRule {(pass ? "通过" : "失败")} Echo={unit.Echo} Call={sum} Missing={missing}");
			unit.Dispose();
		}
	}
}
