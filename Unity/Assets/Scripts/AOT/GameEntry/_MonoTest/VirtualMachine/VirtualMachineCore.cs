using System;
using System.Collections.Generic;

namespace VM
{
	/// <summary>
	/// 单位接口
	/// </summary>
	public interface IUnit : IDisposable
	{
		public VirtualMachineCore Core { get; set; }
	}

	/// <summary>
	/// 单位基类
	/// </summary>
	public class Unit : IUnit
	{
		public VirtualMachineCore Core { get; set; }
		public void Dispose() => Core.PoolRecycle(this);
	}

	/// <summary>
	/// 虚拟机核心
	/// </summary>
	public class VirtualMachineCore : IDisposable
	{
		/// <summary>
		/// 对象池
		/// </summary>
		Dictionary<Type, Queue<IUnit>> ObjectPool = new();

		/// <summary>
		/// 池获取
		/// </summary>
		public T PoolGet<T>() where T : IUnit, new()
		{
			Type type = typeof(T);
			if (!ObjectPool.TryGetValue(type, out var queue))
			{
				queue = new Queue<IUnit>();
				ObjectPool[type] = queue;
			}
			return (queue.Count != 0) ? (T)queue.Dequeue() : new T() { Core = this };
		}

		/// <summary>
		/// 池回收
		/// </summary>
		public void PoolRecycle<T>(T obj) where T : IUnit
		{
			Type type = typeof(T);
			if (!ObjectPool.TryGetValue(type, out var queue))
			{
				queue = new Queue<IUnit>();
				ObjectPool[type] = queue;
			}
			queue.Enqueue(obj);
		}

		public void Dispose() { }
	}

	/// <summary>
	/// 优化的虚拟机栈
	/// </summary>
	public class VMStack
	{
		private VarValue[] _stack;
		private int _top = 0;

		public VMStack(int capacity = 1024)
		{
			_stack = new VarValue[capacity];
		}

		public void Push(VarValue value)
		{
			if (_top >= _stack.Length)
				Array.Resize(ref _stack, _stack.Length * 2);

			_stack[_top++] = value;
		}

		public VarValue Pop()
		{
			return _top > 0 ? _stack[--_top] : default;
		}

		public void PushLong(long value) => Push(value);
		public void PushDouble(double value) => Push(value);
		public void PushBool(bool value) => Push(value);
		public void PushString(string value) => Push(value);
	}
}
