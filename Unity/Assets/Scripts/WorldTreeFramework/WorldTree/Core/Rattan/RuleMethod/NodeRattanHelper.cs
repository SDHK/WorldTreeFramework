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
		public static IRattan AddRattan(INode self, long type)
		{
			UnitDictionary<long, IRattan> rattanDict = self.GetRattanDict;
			if (!rattanDict.TryGetValue(type, out IRattan iRattan))
			{
				rattanDict.Add(type, iRattan = self.Core.PoolGetUnit(type) as IRattan);
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
		public static void RemoveRattanNode(INode self, long rattanType, INode node)
		{
			if (TryGetRattan(self, rattanType, out IRattan Rattan))
			{
				Rattan.RemoveNode(node.Id);
				if (Rattan.Count == 0)
				{
					//移除藤分支
					self.RattanDict.Remove(rattanType);
					if (self.RattanDict.Count == 0)
					{
						self.RattanDict.Dispose();
						self.RattanDict = null;
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
		public static bool TryGetRattan(INode self, long rattanType, out IRattan rattan) => (rattan = (self.RattanDict != null && self.RattanDict.TryGetValue(rattanType, out rattan)) ? rattan : null) != null;

		/// <summary>
		/// 获取藤分支
		/// </summary>
		public static IRattan GetRattan(INode self, long rattanType) => (self.RattanDict != null && self.RattanDict.TryGetValue(rattanType, out IRattan iRattan)) ? iRattan : null;

		#endregion

		#endregion
	}
}