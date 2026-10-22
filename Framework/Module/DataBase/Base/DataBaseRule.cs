/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{
	public static partial class DataBaseRule
	{
		/// <summary>
		/// 给数据库装上实现来源。只有一个来源，重复调用直接返回已有的。
		/// </summary>
		public static TSource AddDataSource<TSource>(this DataBase self)
			where TSource : class, INode, IDataBase, AsRule<Awake>, FieldOf<DataBase>
		{
			if (self.Source is TSource exists) return exists;

			self.AddField(out TSource source);
			self.Source = source;
			return source;
		}

		/// <summary>
		/// 给数据库装上实现来源（带路径）。
		/// </summary>
		public static TSource AddDataSource<TSource>(this DataBase self, string path)
			where TSource : class, INode, IDataBase, AsRule<Awake<string>>, FieldOf<DataBase>
		{
			if (self.Source is TSource exists) return exists;

			self.AddField(out TSource source, path);
			self.Source = source;
			return source;
		}

		/// <summary>
		/// 获取集合
		/// </summary>
		public static IDataCollection<T> GetCollection<T>(this DataBase self)
		{
			return self.Source.GetCollection<T>();
		}

		/// <summary>
		/// 尝试获取集合
		/// </summary>
		public static bool TryGetCollection<T>(this DataBase self, out IDataCollection<T> collection)
		{
			return self.Source.TryGetCollection(out collection);
		}

		/// <summary>
		/// 数量
		/// </summary>
		public static int Count<T>(this DataBase self)
		{
			return self.Source.GetCollection<T>().Count();
		}

		/// <summary>
		/// 插入
		/// </summary>
		public static void Insert<T>(this DataBase self, long id, T data)
		{
			self.Source.GetCollection<T>().Insert(id, data);
		}

		/// <summary>
		/// 根据ID查找
		/// </summary>
		public static T Find<T>(this DataBase self, long id)
		{
			return self.Source.GetCollection<T>().Find(id);
		}

		/// <summary>
		/// 查找所有
		/// </summary>
		public static IEnumerable<T> FindAll<T>(this DataBase self)
		{
			return self.Source.GetCollection<T>().FindAll();
		}

		/// <summary>
		/// 更新
		/// </summary>
		public static bool Update<T>(this DataBase self, long id, T data)
		{
			return self.Source.GetCollection<T>().Update(id, data);
		}

		/// <summary>
		/// 删除所有
		/// </summary>
		public static void DeleteAll<T>(this DataBase self)
		{
			self.Source.GetCollection<T>()?.DeleteAll();
		}

		/// <summary>
		/// 删除
		/// </summary>
		public static void Delete<T>(this DataBase self, long id)
		{
			self.Source.GetCollection<T>()?.Delete(id);
		}
	}
}
