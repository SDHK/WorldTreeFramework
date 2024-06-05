/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/12 08:25:17

* 描述： 未完成...

*/

namespace WorldTree.Internal
{

	/// <summary>
	/// 节点藤分支处理帮助类 
	/// </summary>
	/// <remarks>无约束不安全的用法</remarks>
	public static class NodeRattanHelper
	{
		#region 藤分支处理

		#region 添加

		/// <summary>
		/// 添加藤分支
		/// </summary>
		public static R AddRattan<R>(INode self) where R : class, IRattan => AddRattan(self, TypeInfo<R>.TypeCode) as R;

		/// <summary>
		/// 添加藤分支
		/// </summary>
		public static IRattan AddRattan(INode self, long Type)
		{
			var Rattans = self.Rattans;
			if (!Rattans.TryGetValue(Type, out IRattan iRattan))
			{
				Rattans.Add(Type, iRattan = self.Core.PoolGetUnit(Type) as IRattan);
			}
			return iRattan;
		}

		#endregion

		#region 移除 

		/// <summary>
		/// 移除藤分支中的节点
		/// </summary>
		public static void RemoveRattanNode<R>(INode self, INode node) where R : class, IRattan => RemoveRattanNode(self, TypeInfo<R>.TypeCode, node);

		/// <summary>
		/// 移除藤分支中的节点
		/// </summary>
		public static void RemoveRattanNode(INode self, long RattanType, INode node)
		{
			if (TryGetRattan(self, RattanType, out IRattan Rattan))
			{
				Rattan.RemoveNode(node.Id);
				if (Rattan.Count == 0)
				{
					//移除藤分支
					self.m_Rattans.Remove(RattanType);
					if (self.m_Rattans.Count == 0)
					{
						self.m_Rattans.Dispose();
						self.m_Rattans = null;
					}
					//释放藤分支
					Rattan.Dispose();
				}
			}
		}

		#endregion

		#region 获取

		/// <summary>
		/// 尝试获取藤分支
		/// </summary>
		public static bool TryGetRattan(INode self, long RattanType, out IRattan Rattan) => (Rattan = (self.m_Rattans != null && self.m_Rattans.TryGetValue(RattanType, out Rattan)) ? Rattan : null) != null;

		/// <summary>
		/// 获取藤分支
		/// </summary>
		public static IRattan GetRattan(INode self, long RattanType) => (self.m_Rattans != null && self.m_Rattans.TryGetValue(RattanType, out IRattan iRattan)) ? iRattan : null;

		#endregion

		#endregion
	}
}
