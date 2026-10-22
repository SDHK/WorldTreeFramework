/****************************************

* 作者：闪电黑客
* 日期：2024/11/25 20:43

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 数据库
	/// </summary>
	public abstract class DataBase : Node
		, AsFieldBranch
		, ComponentOf<World>
	{
		/// <summary>
		/// 数据库实现
		/// </summary>
		public IDataBase Source;
	}


}
