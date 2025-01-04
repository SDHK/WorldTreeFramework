/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
using LiteDB;

namespace WorldTree
{
	public static partial class LiteDBManagerRule
	{
		class Awake : AwakeRule<LiteDBManager, string>
		{
			protected override void Execute(LiteDBManager self, string path)
			{
				self.database = new LiteDatabase(path);
			}
		}

		class Remove : RemoveRule<LiteDBManager>
		{
			protected override void Execute(LiteDBManager self)
			{
				self.database.Dispose();
				self.database = null;
			}
		}
	}
}
