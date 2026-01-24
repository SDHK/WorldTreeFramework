/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 计数器
	/// </summary>
	public class CounterCall : Node, ComponentOf<INode>, TempOf<INode>
		, AsChildBranch
		, AsRule<Awake<int>>
		, AsRule<TreeTaskTokenEvent>

	{
		/// <summary>
		/// 是否运行
		/// </summary>
		public bool isRun = false;
		/// <summary>
		/// 计数
		/// </summary>
		public int count = 0;
		/// <summary>
		/// 计数结束
		/// </summary>
		public int countOut = 0;


		/// <summary>
		/// 计数结束回调
		/// </summary>
		public RuleMulticast<ISendRule> Callback;

		public override string ToString()
		{
			return $"CounterCall : {count} , {countOut}";
		}
	}

	public static partial class CounterCallRule
	{
		private class AwakeRule : AwakeRule<CounterCall, int>
		{
			protected override void Execute(CounterCall self, int count)
			{
				self.count = count;
				self.countOut = count;
				self.isRun = true;
				self.AddChild(out self.Callback);
			}
		}

		private class UpdateRule : UpdateRule<CounterCall>
		{
			protected override void Execute(CounterCall self)
			{
				if (self.IsActive && self.isRun)
				{
					self.count++;
					if (self.count >= self.countOut)
					{
						self.Callback.Send();
						self.Dispose();
					}
				}
			}
		}

		private class RemoveRule : RemoveRule<CounterCall>
		{
			protected override void Execute(CounterCall self)
			{
				self.isRun = false;
				self.Callback = null;
			}
		}

		private class TreeTaskTokenEventRule : TreeTaskTokenEventRule<CounterCall>
		{
			protected override void Execute(CounterCall self, TokenState state)
			{
				switch (state)
				{
					case TokenState.Running:
						self.isRun = true;
						break;

					case TokenState.Stop:
						self.isRun = false;
						break;

					case TokenState.Cancel:
						self.isRun = false;
						self.Callback.Send();
						self.Dispose();
						break;
				}
			}
		}
	}
}