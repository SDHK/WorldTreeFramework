
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/16 20:02

* 描述： 树泛型列表
* 
* 大部分功能复制List源码，内部数组则优化为池获取

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace WorldTree
{
	/// <summary>
	/// 树泛型列表
	/// </summary>
	/// <remarks>默认初始容量为2</remarks>
	public class TreeList<T> : Node,
		IList<T>,
		ICollection<T>,
		IEnumerable<T>,
		IEnumerable,
		IList,
		ICollection,
		IReadOnlyList<T>,
		IReadOnlyCollection<T>,
		ChildOf<INode>,
		ComponentOf<INode>,
		AsAwake,
		AsAwake<int>,
		AsAwake<IEnumerable<T>>
	{
		/// <summary>
		/// 默认容量
		/// </summary>
		public int _defaultCapacity = 1;


		/// <summary>
		/// 数组
		/// </summary>
		public T[] _items;

		/// <summary>
		/// 是否发生改变
		/// </summary>
		public int _version;

		/// <summary>
		/// 同步根
		/// </summary>
		[NonSerialized]
		private object _syncRoot;

		/// <summary>
		/// 真实大小
		/// </summary>
		public int _size = 0;

		/// <summary>
		/// 数量
		/// </summary>
		public int Count { get => this._size; }

		/// <summary>
		/// 是否只读
		/// </summary>
		public bool IsReadOnly => false;
		/// <summary>
		/// 是否固定大小
		/// </summary>
		public bool IsFixedSize => false;
		/// <summary>
		/// 是否同步
		/// </summary>
		public bool IsSynchronized => false;

		/// <summary>
		/// 同步根
		/// </summary>
		/// <remarks>lock锁住来确保在多线程环境中安全地访问集合</remarks>
		public object SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
					Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
				return this._syncRoot;
			}
		}

		/// <summary>
		/// 容量大小
		/// </summary>
		public int Capacity
		{
			get => this._items.Length;

			set
			{
				if (value < this._size) this.LogError("下标小于当前大小");
				if (value == this._items.Length) return;
				if (value > 0)
				{
					var destinationArray = this.PoolGetArray<T>(value);
					if (this._size > 0)
						Array.Copy(this._items, 0, destinationArray, 0, this._size);
					this.PoolRecycle(this._items);
					this._items = destinationArray;
				}
				else
				{
					if (this._items != null)
					{
						this.PoolRecycle(this._items);
					}
					this._items = this.PoolGetArray<T>(_defaultCapacity);
				}
			}
		}

		private void EnsureCapacity(int min)
		{
			if (this._items.Length >= min)
				return;
			int num = this._items.Length == 0 ? 4 : this._items.Length * 2;
			if ((uint)num > 2146435071U)
				num = 2146435071;
			if (num < min)
				num = min;
			this.Capacity = num;
		}


		#region 读取设置
		object IList.this[int index] { get => (object)this[index]; set => this[index] = (T)value; }

		public T this[int index]
		{
			get
			{
				if ((uint)index >= (uint)this._size) { this.LogError("下标溢出"); }
				return _items[index];
			}
			set
			{

				if ((uint)index >= (uint)this._size) { this.LogError("下标溢出"); }
				this._items[index] = value;
				++this._version;
			}

		}



		/// <summary>
		/// 范围获取
		/// </summary>
		/// <remarks>返回的列表会挂载这个列表的父节点下</remarks>
		public TreeList<T> GetRange(int index, int count)
		{
			if (index < 0)
				this.LogError("下标小于0");
			if (count < 0)
				this.LogError("数量小于0");
			if (this._size - index < count)
				this.LogError("无效参数");
			TreeList<T> range = this.Parent.AddTemp(out TreeList<T> _, count);
			Array.Copy(_items, index, range._items, 0, count);
			range._size = count;
			return range;
		}



		#endregion



		#region 查询


		#region 判断

		/// <summary>
		/// 判断包含
		/// </summary>
		public bool Contains(T item)
		{
			if ((object)item == null)
			{
				for (int index = 0; index < this._size; ++index)
				{
					if ((object)this._items[index] == null)
						return true;
				}
				return false;
			}
			EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
			for (int index = 0; index < this._size; ++index)
			{
				if (equalityComparer.Equals(this._items[index], item))
					return true;
			}
			return false;
		}

		/// <summary>
		/// 判断包含
		/// </summary>
		public bool Contains(object value)
		{
			return IsCompatibleObject(value) && this.Contains((T)value);
		}

		/// <summary>
		/// 类型判断
		/// </summary>
		private bool IsCompatibleObject(object value)
		{
			if (value is T) return true;
			return value == null && (object)default(T) == null;
		}

		/// <summary>
		/// 条件判断是否存在
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public bool Exists(Predicate<T> match) => this.FindIndex(match) != -1;

		/// <summary>
		/// 条件判断是否全部通过
		/// </summary>
		public bool TrueForAll(Predicate<T> match)
		{
			if (match == null)
				this.LogError("条件为空");
			for (int index = 0; index < this._size; ++index)
			{
				if (!match(this._items[index]))
					return false;
			}
			return true;
		}

		#endregion

		#region 二分查询

		/// <summary>
		/// 二分查找
		/// </summary>
		/// <param name="index">起始下标</param>
		/// <param name="count">查询数量</param>
		/// <param name="item">查询目标</param>
		/// <param name="comparer">比较器</param>
		/// <returns>下标</returns>
		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			if (index < 0)
				this.LogError("下标溢出");
			if (count < 0)
				this.LogError("数量为负");
			if (this._size - index < count)
				this.LogError("参数无效");
			return Array.BinarySearch<T>(this._items, index, count, item, comparer);
		}
		/// <summary>
		/// 二分查找
		/// </summary>
		public int BinarySearch(T item) => this.BinarySearch(0, this.Count, item, null);
		/// <summary>
		/// 二分查找
		/// </summary>
		public int BinarySearch(T item, IComparer<T> comparer) => this.BinarySearch(0, this.Count, item, comparer);

		#endregion

		#region 指定查询

		/// <summary>
		/// 搜索第一次出现的下标
		/// </summary>
		public int IndexOf(T item)
		{
			return Array.IndexOf(this._items, item, 0, this._size);
		}
		/// <summary>
		/// 搜索第一次出现的下标
		/// </summary>
		public int IndexOf(object value)
		{
			return IsCompatibleObject(value) ? this.IndexOf((T)value) : -1;
		}
		/// <summary>
		/// 搜索第一次出现的下标
		/// </summary>
		public int IndexOf(T item, int index)
		{
			if (index > this._size)
				this.LogError("下标超过大小");
			return Array.IndexOf<T>(this._items, item, index, this._size - index);
		}
		/// <summary>
		/// 搜索第一次出现的下标
		/// </summary>
		public int IndexOf(T item, int index, int count)
		{
			if (index > this._size)
				this.LogError("下标超过大小");
			if (count < 0 || index > this._size - count)
				this.LogError("查询数量为负或超过大小");
			return Array.IndexOf<T>(this._items, item, index, count);
		}


		public int LastIndexOf(T item) => this._size == 0 ? -1 : this.LastIndexOf(item, this._size - 1, this._size);

		public int LastIndexOf(T item, int index)
		{
			if (index >= this._size)
				this.LogError("下标超过大小");
			return this.LastIndexOf(item, index, index + 1);
		}
		public int LastIndexOf(T item, int index, int count)
		{
			if (this.Count != 0 && index < 0)
				this.LogError("下标为0");
			if (this.Count != 0 && count < 0)
				this.LogError("数量为0");
			if (this._size == 0)
				return -1;
			if (index >= this._size)
				this.LogError("下标超过大小");
			if (count > index + 1)
				this.LogError("数量小于下标+1");
			return Array.LastIndexOf<T>(this._items, item, index, count);
		}
		#endregion

		/// <summary>
		/// 条件查找元素的下标
		/// </summary>
		/// <param name="startIndex">起始下标</param>
		/// <param name="count">查询数量</param>
		/// <param name="match">查询委托</param>
		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			if ((uint)startIndex > (uint)this._size)
				this.LogError("起始下标超过大小");
			if (count < 0 || startIndex > this._size - count)
				this.LogError("查询数量为负或超过大小");
			if (match == null)
				this.LogError("查询委托为空");
			int num = startIndex + count;
			for (int index = startIndex; index < num; ++index)
			{
				if (match(this._items[index]))
					return index;
			}
			return -1;
		}
		#region 条件查询

		/// <summary>
		/// 条件查找元素的下标
		/// </summary>
		public int FindIndex(int startIndex, Predicate<T> match) => this.FindIndex(startIndex, this._size - startIndex, match);
		/// <summary>
		/// 条件查找元素的下标
		/// </summary>
		public int FindIndex(Predicate<T> match) => this.FindIndex(0, this._size, match);

		/// <summary>
		/// 从后往前条件查找
		/// </summary>
		/// <param name="match">查询委托</param>
		public T FindLast(Predicate<T> match)
		{
			if (match == null)
				this.LogError("查询委托为空");
			for (int index = this._size - 1; index >= 0; --index)
			{
				if (match(this._items[index]))
					return this._items[index];
			}
			return default(T);
		}
		/// <summary>
		/// 从后往前条件查找元素的下标
		/// </summary>
		public int FindLastIndex(Predicate<T> match) => this.FindLastIndex(this._size - 1, this._size, match);
		/// <summary>
		/// 从后往前条件查找元素的下标
		/// </summary>
		public int FindLastIndex(int startIndex, Predicate<T> match) => this.FindLastIndex(startIndex, startIndex + 1, match);
		/// <summary>
		/// 从后往前条件查找元素的下标
		/// </summary>
		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			if (match == null)
				this.LogError("查询委托为空");

			if (this._size == 0)
			{
				if (startIndex != -1)
					this.LogError("起始下标超过大小");
			}
			else if ((uint)startIndex >= (uint)this._size)
				this.LogError("起始下标超过大小");
			if (count < 0 || startIndex - count + 1 < 0)
				this.LogError("查询数量为负或超过大小");
			int num = startIndex - count;
			for (int lastIndex = startIndex; lastIndex > num; --lastIndex)
			{
				if (match(this._items[lastIndex]))
					return lastIndex;
			}
			return -1;
		}

		#endregion



		#endregion

		#region 添加
		/// <summary>
		/// 添加元素
		/// </summary>
		public void Add(T item)
		{
			if (this._size == this._items.Length)
				this.EnsureCapacity(this._size + 1);
			this._items[this._size++] = item;
			++this._version;
		}

		/// <summary>
		/// 添加元素
		/// </summary>
		public int Add(object value)
		{
			this.Add((T)value);
			return this.Count - 1;
		}

		/// <summary>
		/// 添加元素范围
		/// </summary>
		/// <param name="collection"></param>
		public void AddRange(IEnumerable<T> collection) => this.InsertRange(this._size, collection);


		#endregion

		#region 插入

		/// <summary>
		/// 插入元素
		/// </summary>
		public void Insert(int index, T item)
		{
			if ((uint)index > (uint)this._size) this.LogError("下标溢出");
			if (this._size == this._items.Length) this.EnsureCapacity(this._size + 1);

			if (index < this._size)
				Array.Copy(this._items, index, this._items, index + 1, this._size - index);
			this._items[index] = item;
			++this._size;
			++this._version;
		}

		/// <summary>
		/// 插入元素
		/// </summary>
		public void Insert(int index, object value)
		{
			this.Insert(index, (T)value);
		}
		/// <summary>
		/// 插入区间
		/// </summary>
		public void InsertRange(int index, IEnumerable<T> collection)
		{
			if (collection == null) this.LogError("集合为空");
			if ((uint)index > (uint)this._size) this.LogError("下标溢出");
			if (collection is ICollection<T> objs)
			{
				int count = objs.Count;
				if (count > 0)
				{
					this.EnsureCapacity(this._size + count);
					if (index < this._size)
						Array.Copy(this._items, index, this._items, index + count, this._size - index);
					if (this == objs)
					{
						Array.Copy(this._items, 0, this._items, index, index);
						Array.Copy(this._items, index + count, this._items, index * 2, this._size - index);
					}
					else
					{
						T[] array = new T[count];
						objs.CopyTo(array, 0);
						array.CopyTo(_items, index);
					}
					this._size += count;
				}
			}
			else
			{
				foreach (T obj in collection)
					this.Insert(index++, obj);
			}
			++this._version;
		}

		#endregion

		#region 移除

		/// <summary>
		/// 移除元素
		/// </summary>
		public bool Remove(T item)
		{
			int index = this.IndexOf(item);
			if (index < 0)
				return false;
			this.RemoveAt(index);
			return true;
		}
		/// <summary>
		/// 移除元素
		/// </summary>
		public void Remove(object value)
		{
			if (!IsCompatibleObject(value)) return;
			this.Remove((T)value);
		}

		/// <summary>
		/// 通过下标移除元素
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void RemoveAt(int index)
		{
			if ((uint)index >= (uint)this._size) this.LogError("下标溢出");
			--this._size;
			if (index < this._size)
				Array.Copy(this._items, index + 1, this._items, index, this._size - index);
			this._items[this._size] = default(T);
			++this._version;
		}
		/// <summary>
		/// 条件判断移除全部
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public int RemoveAll(Predicate<T> match)
		{
			if (match == null)
				this.LogError("比较器为空");
			int index1 = 0;
			while (index1 < this._size && !match(this._items[index1]))
				++index1;
			if (index1 >= this._size)
				return 0;
			int index2 = index1 + 1;
			while (index2 < this._size)
			{
				while (index2 < this._size && match(this._items[index2]))
					++index2;
				if (index2 < this._size)
					this._items[index1++] = this._items[index2++];
			}
			Array.Clear(_items, index1, this._size - index1);
			int num = this._size - index1;
			this._size = index1;
			++this._version;
			return num;
		}

		/// <summary>
		/// 范围移除
		/// </summary>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public void RemoveRange(int index, int count)
		{
			if (index < 0)
				this.LogError("下标小于0");
			if (count < 0)
				this.LogError("数量小于0");
			if (this._size - index < count)
				this.LogError("查询数量超过大小");
			if (count <= 0)
				return;
			int size = this._size;
			this._size -= count;
			if (index < this._size)
				Array.Copy(_items, index + count, _items, index, this._size - index);
			Array.Clear(_items, this._size, count);
			++this._version;
		}

		#endregion

		#region 清除
		/// <summary>
		/// 清空列表
		/// </summary>
		public void Clear()
		{
			if (this._size > 0)
			{
				Array.Clear(_items, 0, this._size);
				this._size = 0;
			}
			++this._version;
		}

		#endregion

		#region 拷贝
		/// <summary>
		/// 拷贝
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.CopyTo(array, 0);
		}
		/// <summary>
		/// 拷贝
		/// </summary>
		public void CopyTo(Array array, int index)
		{
			Array.Copy(this._items, 0, array, index, this._size);
		}

		/// <summary>
		/// 拷贝
		/// </summary>
		/// <param name="index"></param>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		/// <param name="count"></param>
		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			if (this._size - index < count)
				this.LogError("无效参数");
			Array.Copy(_items, index, array, arrayIndex, count);
		}


		/// <summary>
		/// 转换所有
		/// </summary>
		/// <typeparam name="TOutput">转换类型</typeparam>
		/// <param name="converter">转换器</param>
		/// <returns>新列表将挂在这个列表的父物体上</returns>
		public TreeList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			if (converter == null)
				this.LogError("转换器为空");
			TreeList<TOutput> outputList = this.Parent.AddTemp(out TreeList<TOutput> _, this._size);
			for (int index = 0; index < this._size; ++index)
				outputList._items[index] = converter(this._items[index]);
			outputList._size = this._size;
			return outputList;
		}

		/// <summary>
		/// 转换到数组
		/// </summary>
		/// <remarks>会创建一个真实大小新数组</remarks>
		public T[] ToArray()
		{
			T[] destinationArray = new T[this._size];
			Array.Copy(_items, 0, destinationArray, 0, this._size);
			return destinationArray;
		}

		/// <summary>
		/// 转换到树数组
		/// </summary>
		/// <remarks>新数组将会挂在当前列表父节点上</remarks>
		public TreeArray<T> ToTreeArray()
		{
			TreeArray<T> destinationArray = this.Parent.AddTemp(out TreeArray<T> _, this._size);
			Array.Copy(_items, 0, destinationArray, 0, this._size);
			return destinationArray;
		}


		/// <summary>
		/// 裁剪掉多余空间
		/// </summary>
		public void TrimExcess()
		{
			if (this._size >= (int)((double)this._items.Length * 0.9))
				return;
			this.Capacity = this._size;
		}

		#endregion





		#region 排序
		/// <summary>
		/// 反向排序
		/// </summary>
		public void Reverse() => this.Reverse(0, this.Count);

		/// <summary>
		/// 反向排序
		/// </summary>
		public void Reverse(int index, int count)
		{
			if (index < 0)
				this.LogError("下标小于0");
			if (count < 0)
				this.LogError("数量小于0");
			if (this._size - index < count)
				this.LogError("数量超过大小");
			Array.Reverse(_items, index, count);
			++this._version;
		}

		/// <summary>
		/// 快速排序
		/// </summary>
		public void Sort() => this.Sort(0, this.Count, null);

		/// <summary>
		/// 快速排序
		/// </summary>
		public void Sort(IComparer<T> comparer) => this.Sort(0, this.Count, comparer);
		/// <summary>
		/// 快速排序
		/// </summary>
		public void Sort(int index, int count, IComparer<T> comparer)
		{
			if (index < 0)
				this.LogError("下标小于0");
			if (count < 0)
				this.LogError("数量小于0");
			if (this._size - index < count)
				this.LogError("数量超过大小");
			Array.Sort<T>(this._items, index, count, comparer);
			++this._version;
		}


		#endregion

		#region 遍历

		public Enumerator GetEnumerator() => new Enumerator(this);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}


		/// <summary>
		/// 迭代器
		/// </summary>
		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private TreeList<T> list;
			private int index;
			private int version;
			private T current;

			internal Enumerator(TreeList<T> list)
			{
				this.list = list;
				this.index = 0;
				this.version = list._version;
				this.current = default(T);
			}

			public void Dispose()
			{
				list = null;
				current = default;
			}

			public bool MoveNext()
			{
				TreeList<T> list = this.list;
				if (this.version != list._version || (uint)this.index >= (uint)list._size)
					return this.MoveNextRare();
				this.current = list._items[this.index];
				++this.index;
				return true;
			}

			private bool MoveNextRare()
			{
				if (this.version != this.list._version)
					this.list.LogError("失败的版本号");
				this.index = this.list._size + 1;
				this.current = default(T);
				return false;
			}

			public T Current
			{
				get => this.current;
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.index == 0 || this.index == this.list._size + 1)
						this.list.LogError("不可遍历");
					return (object)this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				if (this.version != this.list._version)
					this.list.LogError("失败的版本号");
				this.index = 0;
				this.current = default(T);
			}
		}



		#endregion


	}

	public static class TreeListRule
	{
		class AwakeRule_<T> : AwakeRule<TreeList<T>>
		{
			protected override void Execute(TreeList<T> self)
			{
				self._items = self.PoolGetArray<T>(self._defaultCapacity);
			}
		}

		class AwakeRuleCapacity<T> : AwakeRule<TreeList<T>, int>
		{
			protected override void Execute(TreeList<T> self, int capacity)
			{
				if (capacity < 0)
					self.LogError("容量为负");
				if (capacity == 0)
					self.LogError("容量为0");
				else
					self._items = self.PoolGetArray<T>(capacity);
			}
		}

		class AwakeRuleCollection<T> : AwakeRule<TreeList<T>, IEnumerable<T>>
		{
			protected override void Execute(TreeList<T> self, IEnumerable<T> collection)
			{
				if (collection == null)
					self.LogError("集合为空");
				if (collection is ICollection<T> objs)
				{
					int count = objs.Count;
					if (count == 0)
					{
						self._items = self.PoolGetArray<T>(self._defaultCapacity);
					}
					else
					{
						self._items = self.PoolGetArray<T>(count);
						objs.CopyTo(self._items, 0);
						self._size = count;
					}
				}
				else
				{
					self._size = self._defaultCapacity;
					self._items = self.PoolGetArray<T>(self._defaultCapacity);
					foreach (T obj in collection)
						self.Add(obj);
				}
			}
		}

		class RemoveRule_<T> : RemoveRule<TreeList<T>>
		{
			protected override void Execute(TreeList<T> self)
			{
				self.Clear();
				self.PoolRecycle(self._items);
				self._items = null;
				self._size = 0;
				self._version = 0;
			}
		}

	}

	



}
