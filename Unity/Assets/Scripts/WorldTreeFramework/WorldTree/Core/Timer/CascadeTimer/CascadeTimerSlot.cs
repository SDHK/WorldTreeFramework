/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定时器

*/
namespace WorldTree
{
	/// <summary>
	/// 槽位节点
	/// </summary>
	public class CascadeTimerSlot : Node
		, ChildOf<CascadeTimer>
		, AsRule<Awake>
	{
		/// <summary>
		/// 定序器迭代器
		/// </summary>
		public IteratorNodeRef<CascadeTimerData> NumberIterator;
	}

	public static class CascadeTimerSlotRule
	{
		class AddRule : AddRule<CascadeTimerSlot>
		{
			protected override void Execute(CascadeTimerSlot self)
			{
				self.AddTemp(out self.NumberIterator);
			}
		}

		/// <summary>
		/// 添加定时器数据 
		/// </summary>
		public static void Add(this CascadeTimerSlot self, CascadeTimerData data)
		{
			self.NumberIterator.TryAdd(data);
		}
	}
}
