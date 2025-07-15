/****************************************

* 作者：闪电黑客
* 日期：2024/11/22 18:11

* 描述：

*/
using LiteDB;

namespace WorldTree
{
	/// <summary>
	/// 单位Bson文档
	/// </summary>
	public class UnitBsonDocument : BsonDocument, IUnit
	{
		[TreeDataIgnore]
		long IWorldTreeBasic.Type { get; set; }
		[TreeDataIgnore]
		public bool IsDisposed { get; set; }
		[TreeDataIgnore]
		public bool IsFromPool { get; set; }
		[TreeDataIgnore]
		public WorldLine Core { get; set; }

		public void Dispose()
		{
			Core.PoolRecycle(this);
		}

		public void OnCreate()
		{
		}

		public void OnDispose()
		{
			this.Clear();
		}
	}
}
