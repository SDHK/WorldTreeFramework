/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:28:38

* 描述： 树泛型队列

*/

using System.Collections.Generic;
using System.Threading;

namespace WorldTree
{
	/// <summary>
	/// 并发列表
	/// </summary>
	public class ConcurrentList<T>
	{
		/// <summary>
		/// 列表
		/// </summary>
		private readonly List<T> objList = new();
		/// <summary>
		/// 读写锁
		/// </summary>
		private readonly ReaderWriterLockSlim lockSlim = new();

		/// <summary>
		/// 添加
		/// </summary>
		public void Add(T item)
		{
			lockSlim.EnterWriteLock();
			try
			{
				objList.Add(item);
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		/// <summary>
		/// 移除
		/// </summary>
		public bool Remove(T item)
		{
			lockSlim.EnterWriteLock();
			try
			{
				return objList.Remove(item);
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		/// <summary>
		/// 获取
		/// </summary>
		public T this[int index]
		{
			get
			{
				lockSlim.EnterReadLock();
				try
				{
					return objList[index];
				}
				finally
				{
					lockSlim.ExitReadLock();
				}
			}
			set
			{
				lockSlim.EnterWriteLock();
				try
				{
					objList[index] = value;
				}
				finally
				{
					lockSlim.ExitWriteLock();
				}
			}
		}

		/// <summary>
		/// 数量
		/// </summary>
		public int Count
		{
			get
			{
				lockSlim.EnterReadLock();
				try
				{
					return objList.Count;
				}
				finally
				{
					lockSlim.ExitReadLock();
				}
			}
		}

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear()
		{
			lockSlim.EnterWriteLock();
			try
			{
				objList.Clear();
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		/// <summary>
		/// 查找
		/// </summary>
		public int FindIndex(System.Predicate<T> match)
		{
			lockSlim.EnterReadLock();
			try
			{
				return objList.FindIndex(match);
			}
			finally
			{
				lockSlim.ExitReadLock();
			}
		}
	}

}
