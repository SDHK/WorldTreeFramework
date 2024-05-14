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
		public T t;
		public long Name;
		public int Age;
		public List<int> ints;
	}

	public class MemoryPackTest : Node
		, ComponentOf<InitialDomain>
		, AsAwake
	{
		public MemoryPackDataTest<string> data;

	}

}
