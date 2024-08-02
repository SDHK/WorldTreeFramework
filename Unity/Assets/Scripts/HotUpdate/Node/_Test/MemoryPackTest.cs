﻿using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 测试数据接口
	/// </summary>
	[MemoryPackable()]
	[MemoryPackUnion(0, typeof(NodeDataTest1))]
	public partial interface INodeDataTest
	{

	}

	/// <summary>
	/// 测试数据
	/// </summary>
	[MemoryPackable(GenerateType.Object)]
	public partial class NodeDataTest1: INodeDataTest
	{
		/// <summary>
		/// 测试
		/// </summary>
		[MemoryPackOrder(1)]
		public int Age;

		/// <summary>
		/// 测试2
		/// </summary>
		[MemoryPackOrder(2)]
		public int Age1;

		/// <summary>
		/// a
		/// </summary>
		[MemoryPackOrder(3)]
		public int Age2;


	}

	/// <summary>
	/// 测试数据
	/// </summary>
	public partial class NodeDataTestSub : NodeDataTest1
	{
		/// <summary>
		/// 测试3
		/// </summary>
		public int AgeSub;
	}

	/// <summary>
	/// 测试数据
	/// </summary>
	//[MemoryPackable]
	[MemoryPackable(GenerateType.VersionTolerant)]
	public partial class NodeDataTest2<T>
	{
		/// <summary>
		/// 测试
		/// </summary>
		[MemoryPackOrder(1)]
		public int Age;

		/// <summary>
		/// 测试
		/// </summary>
		[MemoryPackOrder(3)]
		public float Age3 { get; set; }

		/// <summary>
		/// 测试
		/// </summary>
		[MemoryPackOrder(4)]
		public INodeDataTest Age4;

	}


	/// <summary>
	/// A
	/// </summary>
	[MemoryPackable]
	public partial struct MemoryPackDataTest<T>
		where T : unmanaged
	{
		/// <summary>
		/// 测试泛型
		/// </summary>
		public T Test;

		/// <summary>
		/// ce
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

		/// <summary>
		/// c
		/// </summary>
		public int UID { get; set; }

		/// <summary>
		/// 测试3
		/// </summary>
		public NodeDataTest1 NodeDataTest1;
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
