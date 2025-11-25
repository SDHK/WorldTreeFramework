/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定时器

*/
namespace WorldTree
{

	/// <summary>
	/// 级联定时器数据
	/// </summary>
	public class CascadeTimerData : Node
		, ChildOf<CascadeTimer>
		, AsRule<Awake>
	{
		/// <summary>
		/// 定时时间戳 
		/// </summary>
		public long TimeStamp;

		/// <summary>
		/// 执行节点 
		/// </summary>
		public NodeRef<INode> Node;

		/// <summary>
		/// 事件规则类型 
		/// </summary>
		public long RuleType;

		/// <summary>
		/// 是否为空 
		/// </summary>
		public bool IsNull => Node.IsNull;
	}



	/// <summary>
	/// 槽位节点
	/// </summary>
	public class CascadeTimerSlot : Node
		, ChildOf<CascadeTimer>
		, AsRule<Awake>
	{
		/// <summary>
		/// 级联定时器数据
		/// </summary>
		public TreeList<NodeRef<CascadeTimerData>> TimerDataList;
	}


	public static class CascadeTimerSlotRule
	{
		class AddRule : AddRule<CascadeTimerSlot>
		{
			protected override void Execute(CascadeTimerSlot self)
			{
				self.AddTemp(out self.TimerDataList);
			}
		}

		/// <summary>
		/// 添加定时器数据 
		/// </summary>
		public static void Add(this CascadeTimerSlot self, CascadeTimerData data)
		{
			self.TimerDataList.Add(data);
		}
	}


}
