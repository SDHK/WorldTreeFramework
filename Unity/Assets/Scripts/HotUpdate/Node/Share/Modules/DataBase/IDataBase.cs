/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:18

* 描述：数据库管理器

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 数据集合
	/// </summary>
	public interface IDataCollection : INode
	{
		/// <summary>
		/// 删除
		/// </summary>
		public void Delete(long id);

		/// <summary>
		/// 删除所有
		/// </summary>
		public void DeleteAll();

		/// <summary>
		/// 数量
		/// </summary>
		public int Count();
	}

	/// <summary>
	/// 数据集合接口
	/// </summary>
	public interface IDataCollection<T> : IDataCollection
		, ComponentOf<IDataBase>
	{
		/// <summary>
		/// 插入
		/// </summary>
		public void Insert(long id, T data);

		/// <summary>
		/// 根据Id查找
		/// </summary>
		public T FindById(long id);

		/// <summary>
		/// 更新
		/// </summary>
		public bool Update(long id, T data);

		/// <summary>
		/// 根据条件查找
		/// </summary>
		public IEnumerable<T> Find(Func<T, bool> func);
	}


	/// <summary>
	/// 数据库接口
	/// </summary>
	public interface IDataBase : INode
	{
		/// <summary>
		/// 获取集合
		/// </summary>
		public abstract IDataCollection<T> GetCollection<T>();
		/// <summary>
		/// 尝试获取集合
		/// </summary>
		public abstract bool TryGetCollection<T>(out IDataCollection<T> collection);
	}
}
