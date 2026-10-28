/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{
	public static partial class DataBaseManagerRule
	{
		/// <summary>
		/// 给默认组装上数据库来源。重复添加会报错。
		/// </summary>
		public static TSource AddDataSource<TSource>(this DataBaseManager self)
			where TSource : class, INode, IDataBase, AsRule<Awake>, FieldOf<DataBaseManager>
		{
			return self.AddGroupDataSource<TSource>(DataBaseManager.DefaultGroup);
		}

		/// <summary>
		/// 给默认组装上数据库来源（带路径）。重复添加会报错。
		/// </summary>
		public static TSource AddDataSource<TSource>(this DataBaseManager self, string path)
			where TSource : class, INode, IDataBase, AsRule<Awake<string>>, FieldOf<DataBaseManager>
		{
			return self.AddDataSource<TSource>(DataBaseManager.DefaultGroup, path);
		}

		/// <summary>
		/// 给指定组装上数据库来源。重复添加会报错。
		/// </summary>
		public static TSource AddDataSource<TSource>(this DataBaseManager self, string group, string path)
			where TSource : class, INode, IDataBase, AsRule<Awake<string>>, FieldOf<DataBaseManager>
		{
			string key = group ?? DataBaseManager.DefaultGroup;
			if (self.TryGetSource(key, out _))
				throw new Exception($"数据库名重复: {key}");

			self.AddField(out TSource source, path);
			self.AddSource(key, source);
			return source;
		}

		/// <summary>
		/// 给指定组装上数据库来源。重复添加会报错。
		/// </summary>
		public static TSource AddGroupDataSource<TSource>(this DataBaseManager self, string group)
			where TSource : class, INode, IDataBase, AsRule<Awake>, FieldOf<DataBaseManager>
		{
			string key = group ?? DataBaseManager.DefaultGroup;
			if (self.TryGetSource(key, out _))
				throw new Exception($"数据库名重复: {key}");

			self.AddField(out TSource source);
			self.AddSource(key, source);
			return source;
		}

		/// <summary>
		/// 获取集合
		/// </summary>
		public static IDataCollection<T> GetCollection<T>(this DataBaseManager self)
		{
			return self.Source.GetCollection<T>();
		}

		/// <summary>
		/// 尝试获取集合
		/// </summary>
		public static bool TryGetCollection<T>(this DataBaseManager self, out IDataCollection<T> collection)
		{
			return self.Source.TryGetCollection(out collection);
		}

		/// <summary>
		/// 数量
		/// </summary>
		public static int Count<T>(this DataBaseManager self)
		{
			return self.Source.GetCollection<T>().Count();
		}

		/// <summary>
		/// 插入
		/// </summary>
		public static void Insert<T>(this DataBaseManager self, long id, T data)
		{
			self.Source.GetCollection<T>().Insert(id, data);
		}

		/// <summary>
		/// 根据ID查找
		/// </summary>
		public static T Find<T>(this DataBaseManager self, long id)
		{
			return self.Source.GetCollection<T>().Find(id);
		}

		/// <summary>
		/// 查找所有
		/// </summary>
		public static IEnumerable<T> FindAll<T>(this DataBaseManager self)
		{
			return self.Source.GetCollection<T>().FindAll();
		}

		/// <summary>
		/// 更新
		/// </summary>
		public static bool Update<T>(this DataBaseManager self, long id, T data)
		{
			return self.Source.GetCollection<T>().Update(id, data);
		}

		/// <summary>
		/// 删除所有
		/// </summary>
		public static void DeleteAll<T>(this DataBaseManager self)
		{
			self.Source.GetCollection<T>()?.DeleteAll();
		}

		/// <summary>
		/// 删除
		/// </summary>
		public static void Delete<T>(this DataBaseManager self, long id)
		{
			self.Source.GetCollection<T>()?.Delete(id);
		}

		/// <summary>
		/// 异步查询
		/// </summary>
		public static TreeTask<T> FindAsync<T>(this DataBaseManager self, long id)
		{
			return self.Source.GetCollection<T>().FindAsync(id);
		}

		/// <summary>
		/// 异步查询所有
		/// </summary>
		public static TreeTask FindAllAsync<T>(this DataBaseManager self, ICollection<T> outList)
		{
			return self.Source.GetCollection<T>().FindAllAsync(outList);
		}

		/// <summary>
		/// 异步插入
		/// </summary>
		public static TreeTask InsertAsync<T>(this DataBaseManager self, long id, T data)
		{
			return self.Source.GetCollection<T>().InsertAsync(id, data);
		}

		/// <summary>
		/// 异步更新
		/// </summary>
		public static TreeTask<bool> UpdateAsync<T>(this DataBaseManager self, long id, T data)
		{
			return self.Source.GetCollection<T>().UpdateAsync(id, data);
		}

		/// <summary>
		/// 异步删除
		/// </summary>
		public static TreeTask DeleteAsync<T>(this DataBaseManager self, long id)
		{
			return self.Source.GetCollection<T>().DeleteAsync(id);
		}

		/// <summary>
		/// 异步删除所有
		/// </summary>
		public static TreeTask DeleteAllAsync<T>(this DataBaseManager self)
		{
			return self.Source.GetCollection<T>().DeleteAllAsync();
		}

		/// <summary>
		/// 异步计数
		/// </summary>
		public static TreeTask<int> CountAsync<T>(this DataBaseManager self)
		{
			return self.Source.GetCollection<T>().CountAsync();
		}
	}
}
