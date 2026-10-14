/******************************

 * Author: 闪电黑客

 * 日期: 2021/12/13 13:40:18

 * 最后日期: 2021/12/15 18:32:23

 * 最后修改: 闪电黑客

 * 描述:  
  
	单位对象接口
    让IUnit类获得管理的生命周期

******************************/

namespace WorldTree
{
	/// <summary>
	/// 单位对象接口
	/// </summary>
	[TreeDataSerializable]
	public partial interface IUnit : IBasic
	{
		/// <summary>
		/// Id。普通单位和普通节点与 InstanceId 相同。数据节点是雪花 Id，从 1 &lt;&lt; 50 起。
		/// </summary>
		[TreeDataIgnore]
		public long Id { get; set; }

		/// <summary>
		/// 实例 Id。每次取出都换，用来判断对象是不是已经还池。
		/// </summary>
		[TreeDataIgnore]
		public long InstanceId { get; set; }
	}
}
