using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 测试类
	/// </summary>
	class MyClass
	{
		/// <summary>
		/// 整型字段
		/// </summary>
		public int IntField;
		/// <summary>
		/// 长整型字段
		/// </summary>
		public long LongField;
		/// <summary>
		/// 浮点型字段
		/// </summary>
		public float FloatField;
	}

	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsCurveEvaluate
		, AsComponentBranch
	{

		public DotNetTestNode()
		{

			MyClass obj = new MyClass
			{
				IntField = 123,
				LongField = 4567890123456789,
				FloatField = 123.45f
			};

			byte[] bytes = new byte[sizeof(int) + sizeof(long) + sizeof(float)];

			unsafe
			{
				fixed (byte* pBytes = bytes)
				{
					byte* pCurrent = pBytes;

					// Copy IntField
					byte* intFieldAsBytePtr = (byte*)Unsafe.AsPointer(ref Unsafe.As<int, byte>(ref obj.IntField));
					Unsafe.CopyBlockUnaligned(pCurrent, intFieldAsBytePtr, sizeof(int));
					pCurrent += sizeof(int);

					// Copy LongField
					byte* longFieldAsBytePtr = (byte*)Unsafe.AsPointer(ref Unsafe.As<long, byte>(ref obj.LongField));
					Unsafe.CopyBlockUnaligned(pCurrent, longFieldAsBytePtr, sizeof(long));
					pCurrent += sizeof(long);

					// Copy FloatField
					byte* floatFieldAsBytePtr = (byte*)Unsafe.AsPointer(ref Unsafe.As<float, byte>(ref obj.FloatField));
					Unsafe.CopyBlockUnaligned(pCurrent, floatFieldAsBytePtr, sizeof(float));
				}
			}

			Console.WriteLine("Bytes:");
			foreach (var b in bytes)
			{
				Console.WriteLine(b);
			}

			MyClass newObj = new MyClass();

			unsafe
			{
				fixed (byte* pBytes = bytes)
				{
					byte* pCurrent = pBytes;

					// Copy back IntField
					newObj.IntField = Unsafe.Read<int>(pCurrent);
					pCurrent += sizeof(int);

					// Copy back LongField
					newObj.LongField = Unsafe.Read<long>(pCurrent);
					pCurrent += sizeof(long);

					// Copy back FloatField
					newObj.FloatField = Unsafe.Read<float>(pCurrent);
				}
			}

			// 现在newObj包含了从bytes数组中反序列化得到的值
			Console.WriteLine($"IntField: {newObj.IntField}, LongField: {newObj.LongField}, FloatField: {newObj.FloatField}");
		}

	}

	/// <summary>
	/// DotNetTestNodeRule
	/// </summary>
	public static partial class DotNetTestNodeRule
	{
		private static OnUpdate<DotNetTestNode> Update = (self) =>
		{
			self.Log($"初始更新！！！");
		};

		private static OnUpdateTime<DotNetTestNode> UpdateTime = (self, timeSpan) =>
		{
			self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		};

		private static OnCurveEvaluate<DotNetTestNode> OnCurveEvaluate = (self, time) =>
		{
			self.Log($"曲线更新！！！{time}");
			return time;
		};

		private static OnEnable<DotNetTestNode> Enable1 = (self) =>
		{
			self.Log("激活1！！");
		};

		private static OnEnable<DotNetTestNode> Enable2 = (self) =>
		{
			self.Log("激活2！！");
		};

		private static OnEnable<DotNetTestNode> Enable3 = (self) =>
		{
			self.Log("激活3！！");
		};

		private static OnDisable<DotNetTestNode> Disable = (self) =>
		{
			self.Log("失活！！");
		};

		private static OnAdd<DotNetTestNode> Add = (self) =>
		{
			self.Log(" 初始化！！！");
		};

		private static OnRemove<DotNetTestNode> Remove = (self) =>
		{
			self.Log($"初始关闭！！");
		};
	}
}