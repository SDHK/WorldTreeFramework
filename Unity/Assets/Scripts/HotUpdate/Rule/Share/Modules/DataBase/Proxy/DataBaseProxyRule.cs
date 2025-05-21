/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{
	public static partial class DataBaseProxyRule
	{

		/// <summary>
		/// 获取集合
		/// </summary>
		public static IDataCollection<T> GetCollection<T>(this DataBaseProxy self)
			where T : class, INodeData
		{
			return self.DataBase.GetCollection<T>();
		}

		/// <summary>
		/// 尝试获取集合
		/// </summary>
		public static bool TryGetCollection<T>(this DataBaseProxy self, out IDataCollection<T> collection)
			where T : class, INodeData
		{
			return self.DataBase.TryGetCollection(out collection);
		}

		/// <summary>
		/// 数量
		/// </summary>
		public static int Count<T>(this DataBaseProxy self)
			where T : class, INodeData
		{
			return self.DataBase.GetCollection<T>().Count();
		}

		/// <summary>
		/// 插入
		/// </summary>
		public static void Insert<T>(this DataBaseProxy self, T data)
			where T : class, INodeData
		{
			self.DataBase.GetCollection<T>().Insert(data.Id, data);
		}

		/// <summary>
		/// 根据ID查找
		/// </summary>
		public static T Find<T>(this DataBaseProxy self, long id)
			where T : class, INodeData
		{
			return self.DataBase.GetCollection<T>().Find(id);
		}

		/// <summary>
		/// 查找所有
		/// </summary>
		public static IEnumerable<T> FindAll<T>(this DataBaseProxy self)
			where T : class, INodeData
		{
			return self.DataBase.GetCollection<T>().FindAll();
		}

		/// <summary>
		/// 更新
		/// </summary>
		public static bool Update<T>(this DataBaseProxy self, T data)
			where T : class, INodeData
		{
			return self.DataBase.GetCollection<T>().Update(data.Id, data);
		}

		/// <summary>
		/// 删除所有
		/// </summary>
		public static void DeleteAll<T>(this DataBaseProxy self)
			where T : class, INodeData
		{
			self.DataBase.GetCollection<T>()?.DeleteAll();
		}

		/// <summary>
		/// 删除
		/// </summary>
		public static void Delete<T>(this DataBaseProxy self, long id)
			where T : class, INodeData
		{
			self.DataBase.GetCollection<T>()?.Delete(id);
		}
	}
}
