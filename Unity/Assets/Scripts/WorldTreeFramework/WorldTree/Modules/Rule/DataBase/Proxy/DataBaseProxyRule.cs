﻿/****************************************

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
		{
			return self.DataBase.GetCollection<T>();
		}

		/// <summary>
		/// 尝试获取集合
		/// </summary>
		public static bool TryGetCollection<T>(this DataBaseProxy self, out IDataCollection<T> collection)
		{
			return self.DataBase.TryGetCollection(out collection);
		}

		/// <summary>
		/// 数量
		/// </summary>
		public static int Count<T>(this DataBaseProxy self)
		{
			return self.DataBase.GetCollection<T>().Count();
		}

		/// <summary>
		/// 插入
		/// </summary>
		public static void Insert<T>(this DataBaseProxy self, long id, T data)
		{
			self.DataBase.GetCollection<T>().Insert(id, data);
		}

		/// <summary>
		/// 根据ID查找
		/// </summary>
		public static T Find<T>(this DataBaseProxy self, long id)
		{
			return self.DataBase.GetCollection<T>().Find(id);
		}

		/// <summary>
		/// 查找所有
		/// </summary>
		public static IEnumerable<T> FindAll<T>(this DataBaseProxy self)
		{
			return self.DataBase.GetCollection<T>().FindAll();
		}

		/// <summary>
		/// 更新
		/// </summary>
		public static bool Update<T>(this DataBaseProxy self, long id, T data)
		{
			return self.DataBase.GetCollection<T>().Update(id, data);
		}

		/// <summary>
		/// 删除所有
		/// </summary>
		public static void DeleteAll<T>(this DataBaseProxy self)
		{
			self.DataBase.GetCollection<T>()?.DeleteAll();
		}

		/// <summary>
		/// 删除
		/// </summary>
		public static void Delete<T>(this DataBaseProxy self, long id)
		{
			self.DataBase.GetCollection<T>()?.Delete(id);
		}
	}
}
