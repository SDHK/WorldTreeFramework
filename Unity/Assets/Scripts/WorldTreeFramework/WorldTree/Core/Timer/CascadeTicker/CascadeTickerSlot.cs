/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定序器槽位

*/
namespace WorldTree
{
	/// <summary>
	/// 槽位节点
	/// </summary>
	public class CascadeTickerSlot : Node
		, ChildOf<CascadeTicker>
		, AsRule<Awake>
	{
		/// <summary>
		/// 定序器迭代器
		/// </summary>
		public IteratorNodeRef<CascadeTickerData> TickIterator;
	}

	public static class CascadeTickerSlotRule
	{
		class AddRule : AddRule<CascadeTickerSlot>
		{
			protected override void Execute(CascadeTickerSlot self)
			{
				self.AddTemp(out self.TickIterator);
			}
		}

		/// <summary>
		/// 添加定序器数据 
		/// </summary>
		public static void Add(this CascadeTickerSlot self, CascadeTickerData data)
		{
			self.TickIterator.TryAdd(data);
		}
	}
}
