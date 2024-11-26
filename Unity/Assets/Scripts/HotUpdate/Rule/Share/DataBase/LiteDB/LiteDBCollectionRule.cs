/****************************************

* 作者：闪电黑客
* 日期：2024/11/23 17:23

* 描述：

*/

using LiteDB;

namespace WorldTree
{
	public static partial class LiteDBCollectionRule
	{
		class Awake<T> : AwakeRule<LiteDBCollection<T>, ILiteCollection<BsonDocument>>
			where T : class, INodeData
		{
			protected override void Execute(LiteDBCollection<T> self, ILiteCollection<BsonDocument> collection)
			{
				self.collection = collection;
			}
		}
	}
}
