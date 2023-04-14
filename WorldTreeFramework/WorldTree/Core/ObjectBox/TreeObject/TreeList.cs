
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/16 20:02

* 描述： 树泛型列表

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
    public class TreeList<T> : Node,
        IList<T>,
        ICollection<T>,
        IEnumerable<T>,
        IEnumerable,
        IList,
        ICollection,
        IReadOnlyList<T>,
        IReadOnlyCollection<T>,
        IAwake,
        ChildOf<INode>
    {
        /// <summary>
        /// 数组
        /// </summary>
        public TreeArray<T> _items;

        /// <summary>
        /// 是否发生改变
        /// </summary>
        private int _version;

        /// <summary>
        /// 同步根
        /// </summary>
        [NonSerialized]
        private object _syncRoot;

        /// <summary>
        /// 真实大小
        /// </summary>
        public int _size;

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
                    Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), (object)null);
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
                if (value < this._size) World.LogError("下标小于当前大小");
                if (value == this._items.Length) return;
                if (value > 0)
                {
                    this.AddChild(out TreeArray<T> destinationArray, value);
                    if (this._size > 0)
                        Array.Copy(this._items, 0, destinationArray.array, 0, this._size);
                    this._items.Dispose();
                    this._items = destinationArray;
                }
                else
                {
                    //初始化
                    this.AddChild(out this._items, 0);
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
        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        T IList<T>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)this._size) { World.LogError("下标溢出"); }
                return _items[index];
            }
            set
            {

                if ((uint)index >= (uint)this._size) { World.LogError("下标溢出"); }
                this._items[index] = value;
                ++this._version;
            }

        }
        #endregion



        #region 查询

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

        #endregion

        #region 插入

        /// <summary>
        /// 插入元素
        /// </summary>
        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)this._size) World.LogError("下标溢出");
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
            if (collection == null) World.LogError("集合为空");
            if ((uint)index > (uint)this._size) World.LogError("下标溢出");
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
                        array.CopyTo((Array)this._items, index);
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
            if ((uint)index >= (uint)this._size) World.LogError("下标溢出");
            --this._size;
            if (index < this._size)
                Array.Copy(this._items, index + 1, this._items, index, this._size - index);
            this._items[this._size] = default(T);
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
                Array.Clear((Array)this._items, 0, this._size);
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

        #endregion


        #region 遍历

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion


    }


    class TreeListRemoveRule<T> : RemoveRule<TreeList<T>>
    {
        public override void OnEvent(TreeList<T> self)
        {
            self._items.Dispose();
            self._items = null;
        }
    }



}
