/****************************************

* 作者：闪电黑客
* 日期：2024/11/20 20:18

* 描述：

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
		where T : class, INodeData
	{
		/// <summary>
		/// 插入
		/// </summary>
		public void Insert(T data);

		/// <summary>
		/// 根据Id查找
		/// </summary>
		public T Find(long id);

		/// <summary>
		/// 查找所有
		/// </summary>
		public IEnumerable<T> FindAll();

		/// <summary>
		/// 更新
		/// </summary>
		public bool Update(T data);

	}


	/// <summary>
	/// 数据库接口
	/// </summary>
	public interface IDataBase : INode
	{
		/// <summary>
		/// 获取集合
		/// </summary>
		public abstract IDataCollection<T> GetCollection<T>() where T : class, INodeData;

		/// <summary>
		/// 尝试获取集合
		/// </summary>
		public abstract bool TryGetCollection<T>(out IDataCollection<T> collection) where T : class, INodeData;
	}

}
