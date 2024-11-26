/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:18

* 描述：数据库管理器

*/
using LiteDB;

namespace WorldTree
{
	/// <summary>
	/// 数据集合
	/// </summary>
	public class LiteDBCollection<T> : Node, IDataCollection<T>
		, AsAwake<ILiteCollection<BsonDocument>>
		where T : class, INodeData
	{
		/// <summary>
		/// 数据集合
		/// </summary>
		public ILiteCollection<BsonDocument> collection;

		public void Insert(T data)
		{
			collection.Insert(new BsonDocument
			{
				["_id"] = data.Id,
				["D"] = TreeDataHelper.SerializeNode(data)
			});
		}

		public T FindById(long id)
		{
			BsonDocument aDict = collection.FindById(id);
			return aDict != null ? TreeDataHelper.DeseralizeNode<T>(this, aDict["D"].AsBinary) : null;
		}

		public bool Update(T data)
		{
			return collection.Update(new BsonDocument
			{
				["_id"] = data.Id,
				["D"] = TreeDataHelper.SerializeNode(data)
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
