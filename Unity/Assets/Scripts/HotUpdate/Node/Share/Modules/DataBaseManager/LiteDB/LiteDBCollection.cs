/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:18

* 描述：数据库管理器

*/
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WorldTree
{
	/// <summary>
	/// 数据集合
	/// </summary>
	public class LiteDBCollection<T> : Node, IDataCollection<T>
		, AsAwake<ILiteCollection<T>>
	{
		/// <summary>
		/// 数据集合
		/// </summary>
		public ILiteCollection<T> collection;

		public void Insert(long id, T data)
		{
			collection.Insert(data);
		}

		public IEnumerable<T> Find(Func<T, bool> func)
		{
			return collection.Find(func as Expression<Func<T, bool>>);
		}

		public T FindById(long id)
		{
			return collection.FindById(id);
		}

		public void Delete(long id)
		{
			collection.Delete(id);
		}

		public void DeleteAll()
		{
			collection.DeleteAll();
		}

		public int Count()
		{
			return collection.Count();
		}
	}
}
