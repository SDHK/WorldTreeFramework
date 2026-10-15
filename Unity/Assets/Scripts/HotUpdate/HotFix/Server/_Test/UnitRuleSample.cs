/****************************************

* 作者：闪电黑客
* 日期：2026/10/15

* 描述：纯单位执行法则的例子。不挂树。

*/
namespace WorldTree.Server
{
	/// <summary>
	/// 单位通知法则
	/// </summary>
	public interface UnitEcho : ISendRule<int>, IMethodRule { }

	/// <summary>
	/// 单位调用法则。最后的 int 是返回值。
	/// </summary>
	public interface UnitAdd : ICallRule<int, int, int>, IMethodRule { }

	/// <summary>
	/// 没有实现类的法则，用来确认找不到法则时不会抛异常。
	/// </summary>
	public interface UnitMissing : ISendRule<int> { }

	/// <summary>
	/// 纯单位。只声明自己能执行的法则，不挂树。
	/// </summary>
	public class UnitRuleSample : Unit, AsRule<UnitEcho>, AsRule<UnitAdd>
	{
		/// <summary>
		/// 通知法则写回来的值
		/// </summary>
		public int Echo;
	}

	/// <summary>
	/// 把参数写进单位，并打一条日志
	/// </summary>
	public class UnitEchoRule : SendRule<UnitRuleSample, UnitEcho, int>
	{
		/// <summary>
		/// 执行通知
		/// </summary>
		protected override void Execute(UnitRuleSample self, int arg1)
		{
			self.Echo = arg1;
			self.Log($"UnitEcho 命中 arg={arg1} id={self.Id}");
		}
	}

	/// <summary>
	/// 返回两数之和
	/// </summary>
	public class UnitAddRule : CallRule<UnitRuleSample, UnitAdd, int, int, int>
	{
		/// <summary>
		/// 执行调用
		/// </summary>
		protected override int Execute(UnitRuleSample self, int arg1, int arg2)
		{
			int sum = arg1 + arg2;
			self.Log($"UnitAdd 命中 {arg1}+{arg2}={sum}");
			return sum;
		}
	}
}
