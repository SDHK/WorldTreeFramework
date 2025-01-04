/****************************************

* 作者：闪电黑客
* 日期：2024/11/25 20:43

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 数据库代理基类
	/// </summary>
	public abstract class DataBaseProxy : Node
		, AsComponentBranch
		, ComponentOf<WorldTreeRoot>
	{
		/// <summary>
		/// 数据库引用
		/// </summary>
		public IDataBase DataBase;
	}


}
