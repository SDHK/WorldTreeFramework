/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:20

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{
	public static partial class DataBaseManagerRule
	{
		/// <summary>
		/// 插入
		/// </summary>
		public static void Insert<T>(this DataBaseManager self, long id, T data)
		{
			self.GetCollection<T>().Insert(id, data);
		}

		/// <summary>
		/// 根据ID查找
		/// </summary>
		public static T FindById<T>(this DataBaseManager self, long id)
		{
			return self.GetCollection<T>().FindById(id);
		}

		/// <summary>
		/// 查找
		/// </summary>
		public static IEnumerable<T> Find<T>(this DataBaseManager self, System.Func<T, bool> func)
		{
			return self.GetCollection<T>()?.Find(func);
		}

		/// <summary>
		/// 删除
		/// </summary>
		public static void Delete<T>(this DataBaseManager self, long id)
		{
			self.GetCollection<T>()?.Delete(id);
		}
	}
}
