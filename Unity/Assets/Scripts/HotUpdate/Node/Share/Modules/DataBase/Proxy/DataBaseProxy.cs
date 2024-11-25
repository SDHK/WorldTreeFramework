/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:18

* 描述：数据库代理基类

*/
namespace WorldTree
{
	/// <summary>
	/// 数据库代理基类
	/// </summary>
	public abstract class DataBaseProxy : Node
		, ComponentOf<WorldTreeRoot>
	{
		/// <summary>
		/// 数据库引用
		/// </summary>
		public IDataBase DataBase;
	}


}
