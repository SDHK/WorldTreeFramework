/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:18

* 描述：数据库管理器

*/
using LiteDB;

namespace WorldTree
{
	/// <summary>
	/// 数据库管理器
	/// </summary>
	public abstract class LiteDBManager : Node, IDataBase
		, AsComponentBranch
		, AsAwake<string>
	{
		/// <summary>
		/// 数据库
		/// </summary>
		public LiteDatabase database;

		public IDataCollection<T> GetCollection<T>()
		{
			if (!this.TryGetComponent(out IDataCollection<T> node))
			{
				ILiteCollection<T> collection = database.GetCollection<T>();
				if (collection != null)
				{
					node = this.AddComponent(default(IDataCollection<T>), out LiteDBCollection<T> _, collection);
				}
			}
			return node;
		}

		public bool TryGetCollection<T>(out IDataCollection<T> collection)
		{
			if (!this.TryGetComponent(out collection))
			{
				collection = GetCollection<T>();
			}
			return collection != null;
		}
	}
}
