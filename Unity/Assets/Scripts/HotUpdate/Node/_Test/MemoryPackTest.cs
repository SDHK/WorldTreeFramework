using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{

	[MemoryPackable]
	public partial class MemoryPackDataTest<T>
	{
		/// <summary>
		/// 测试泛型
		/// </summary>
		public T Test;
		/// <summary>
		/// 测试
		/// </summary>
		public long Name;
		/// <summary>
		/// 测试
		/// </summary>
		public int Age;
		/// <summary>
		/// 测试
		/// </summary>
		public List<int> IntList;
	}

	/// <summary>
	/// 测试内存包
	/// </summary>
	public class MemoryPackTest : Node
		, ComponentOf<InitialDomain>
		, AsAwake
	{
		/// <summary>
		/// 测试数据
		/// </summary>
		public MemoryPackDataTest<string> data;

	}

}
