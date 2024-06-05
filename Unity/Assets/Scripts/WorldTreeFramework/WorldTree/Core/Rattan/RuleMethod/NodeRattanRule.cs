/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/12 08:25:17

* 描述： 未完成...

*/

namespace WorldTree.Internal
{
	public static class NodeRattanRule
	{
		#region 藤分支处理

		#region 添加

		/// <summary>
		/// 添加藤分支
		/// </summary>
		public static R AddRattan<R>(this INode self) where R : class, IRattan => self.AddRattan(TypeInfo<R>.TypeCode) as R;

		/// <summary>
		/// 添加藤分支
		/// </summary>
		public static IRattan AddRattan(this INode self, long Type)
		{
			var Rattans = self.Rattans;
			if (!Rattans.TryGetValue(Type, out IRattan iRattan))
			{
				Rattans.Add(Type, iRattan = self.PoolGetUnit(Type) as IRattan);
			}
			return iRattan;
		}

		#endregion

		#region 移除 

		/// <summary>
		/// 移除藤分支中的节点
		/// </summary>
		public static void RemoveRattanNode<R>(this INode self, INode node) where R : class, IRattan => self.RemoveRattanNode(TypeInfo<R>.TypeCode, node);

		/// <summary>
		/// 移除藤分支中的节点
		/// </summary>
		public static void RemoveRattanNode(this INode self, long RattanType, INode node)
		{
			if (self.TryGetRattan(RattanType, out IRattan Rattan))
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
		public static bool TryGetRattan<R>(this INode self, out R Rattan) where R : class, IRattan => (Rattan = (self.m_Rattans != null && self.m_Rattans.TryGetValue(TypeInfo<R>.TypeCode, out IRattan IRattan)) ? IRattan as R : null) != null;
		/// <summary>
		/// 尝试获取藤分支
		/// </summary>
		public static bool TryGetRattan(this INode self, long RattanType, out IRattan Rattan) => (Rattan = (self.m_Rattans != null && self.m_Rattans.TryGetValue(RattanType, out Rattan)) ? Rattan : null) != null;
		/// <summary>
		/// 获取藤分支
		/// </summary>
		public static R GetRattan<R>(this INode self) where R : class, IRattan => (self.m_Rattans != null && self.m_Rattans.TryGetValue(TypeInfo<R>.TypeCode, out IRattan iRattan)) ? iRattan as R : null;
		/// <summary>
		/// 获取藤分支
		/// </summary>
		public static IRattan GetRattan(this INode self, long RattanType) => (self.m_Rattans != null && self.m_Rattans.TryGetValue(RattanType, out IRattan iRattan)) ? iRattan : null;

		#endregion

		#endregion
	}
}
