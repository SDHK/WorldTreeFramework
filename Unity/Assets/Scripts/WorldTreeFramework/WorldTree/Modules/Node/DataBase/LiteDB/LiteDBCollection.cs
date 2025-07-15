/****************************************

* 作者：闪电黑客
* 日期：2024/11/22 18:11

* 描述：

*/
using LiteDB;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 数据集合
	/// </summary>
	public class LiteDBCollection<T> : Node, IDataCollection<T>
		, AsAwake<ILiteCollection<BsonDocument>>
	{
		/// <summary>
		/// 数据集合
		/// </summary>
		public ILiteCollection<BsonDocument> collection;


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


	}
}
