/****************************************

* 作者：闪电黑客
* 日期：2024/11/22 18:11

* 描述：

*/
using LiteDB;
using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 数据集合
	/// </summary>
	public partial class LiteDBCollection<T> : Node, IDataCollection<T>
		, AsRule<Awake<ILiteCollection<BsonDocument>>>
		, AsRule<Awake>
	{
		/// <summary>
		/// 数据集合
		/// </summary>
		public ILiteCollection<BsonDocument> collection;

		[NodeRule(nameof(Awake<ILiteCollection<BsonDocument>>))]
		private static void OnAwakeRule(LiteDBCollection<T> self, ILiteCollection<BsonDocument> collection)
		{
			self.collection = collection;
		}


		public void Insert(long id, T data)
		{
			collection.Insert(new BsonDocument
			{
				["_id"] = id,
				["D"] = TreeDataHelper.Serialize(this, data)
			});
		}

		public T Find(long id)
		{
			BsonDocument aDict = collection.FindById(id);
			return aDict != null ? TreeDataHelper.Deseralize<T>(this, aDict["D"].AsBinary) : default;
		}

		public IEnumerable<T> FindAll()
		{
			foreach (BsonDocument data in collection.FindAll())
			{
				yield return TreeDataHelper.Deseralize<T>(this, data["D"].AsBinary);
			}
		}

		public bool Update(long id, T data)
		{
			return collection.Update(new BsonDocument
			{
				["_id"] = id,
				["D"] = TreeDataHelper.Serialize(this, data)
			});
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

		/// <summary>
		/// 异步查询（LiteDB 同步执行）
		/// </summary>
		public async TreeTask<T> FindAsync(long id)
		{
			await this.TreeTaskCompleted();
			return Find(id);
		}

		/// <summary>
		/// 异步查询所有，结果填充到外部集合（LiteDB 同步执行）
		/// </summary>
		public async TreeTask FindAllAsync(ICollection<T> outList)
		{
			if (outList == null) throw new ArgumentNullException(nameof(outList));

			await this.TreeTaskCompleted();
			outList.Clear();
			foreach (T data in FindAll())
			{
				outList.Add(data);
			}
		}

		/// <summary>
		/// 异步插入（LiteDB 同步执行）
		/// </summary>
		public async TreeTask InsertAsync(long id, T data)
		{
			await this.TreeTaskCompleted();
			Insert(id, data);
		}

		/// <summary>
		/// 异步更新（LiteDB 同步执行）
		/// </summary>
		public async TreeTask<bool> UpdateAsync(long id, T data)
		{
			await this.TreeTaskCompleted();
			return Update(id, data);
		}

		/// <summary>
		/// 异步删除（LiteDB 同步执行）
		/// </summary>
		public async TreeTask DeleteAsync(long id)
		{
			await this.TreeTaskCompleted();
			Delete(id);
		}

		/// <summary>
		/// 异步删除所有（LiteDB 同步执行）
		/// </summary>
		public async TreeTask DeleteAllAsync()
		{
			await this.TreeTaskCompleted();
			DeleteAll();
		}

		/// <summary>
		/// 异步计数（LiteDB 同步执行）
		/// </summary>
		public async TreeTask<int> CountAsync()
		{
			await this.TreeTaskCompleted();
			return Count();
		}
	}
}
