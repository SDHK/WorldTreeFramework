/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 一个编号分发的管理器
* 
* 后续需要改为根据时间生成
*/

namespace WorldTree
{

	/// <summary>
	/// id管理器
	/// </summary>
	public class IdManager : Node, IListenerIgnorer, ComponentOf<WorldTreeCore>
	{
		public IdManager()
		{
			Type = TypeInfo<IdManager>.TypeCode;
		}

		/// <summary>
		/// 当前递增的id值
		/// </summary>
		public long currentId = 0;

		/// <summary>
		/// 释放后
		/// </summary>
		public override void OnDispose()
		{
			NodeBranchHelper.RemoveBranchNode(this.Parent, this.BranchType, this);//从父节点分支移除
			this.IsRecycle = true;
			this.IsDisposed = true;
		}
	}

	public static class IdManagerRule
	{
		/// <summary>
		/// 获取id后递增
		/// </summary>
		public static long GetId(this IdManager self)
		{
			return self.currentId++;
		}
	}
}
