/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/17 11:55

* 描述： 

*/
namespace WorldTree
{
	public static partial class TreeLoaderRule
	{
		[NodeRule(nameof(AddRule<TreeLoader>))]
		private static void OnAddRule(this TreeLoader self)
		{
			//self.pool.GetObject();
		}


	}
}
