/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:20

* 描述：

*/
namespace WorldTree
{
	public static partial class DataBaseManagerRule
	{
		private static OnAdd<DataBaseManager> OnAdd = (self) =>
		{


		};

		/// <summary>
		/// 插入
		/// </summary>
		public static void Insert<T>(this DataBaseManager self, T data)
		{

		}

		/// <summary>
		/// 根据ID查找
		/// </summary>
		public static void FindById(this DataBaseManager self, long id)
		{

		}

		/// <summary>
		/// 查找
		/// </summary>
		public static void Find<T>(this DataBaseManager self, System.Func<T, bool> func)
		{

		}

		/// <summary>
		/// 删除
		/// </summary>
		public static void Delete(this DataBaseManager self, long id)
		{

		}
	}
}
