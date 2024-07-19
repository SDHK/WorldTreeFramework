/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：字节序列只读器

*/
using System;
using System.Buffers;

namespace WorldTree
{

	public static class ByteReadOnlySequenceBuilderRule
	{
		class Add : AddRule<ByteReadOnlySequenceBuilder>
		{
			protected override void Execute(ByteReadOnlySequenceBuilder self)
			{
				self.Core.PoolGetUnit(out self.segmentList);
			}
		}

		class Remove : RemoveRule<ByteReadOnlySequenceBuilder>
		{
			protected override void Execute(ByteReadOnlySequenceBuilder self)
			{
				foreach (var item in self.segmentList) item.Dispose();
				self.segmentList.Dispose();
				self.segmentList = null;
			}
		}
	}


	/// <summary>
	/// 字节只读序列生成器
	/// </summary>
	public sealed class ByteReadOnlySequenceBuilder : Node
		, AsAwake
		, AsComponentBranch
		, TempOf<INode>
	{
		/// <summary>
		/// 片段列表
		/// </summary>
		public UnitList<ByteReadSequence> segmentList;

		/// <summary>
		/// 添加
		/// </summary>
		public void Add(ReadOnlyMemory<byte> buffer, bool returnToPool = false)
		{
			Core.PoolGetUnit(out ByteReadSequence byteReadSequence);
			byteReadSequence.SetBuffer(buffer, returnToPool);
			segmentList.Add(byteReadSequence);
		}

		/// <summary>
		/// 清空
		/// </summary>
		public void Clear()
		{
			foreach (var item in segmentList) item.Dispose();
			segmentList.Clear();
		}

		/// <summary>
		/// 创建拼接只读序列
		/// </summary>
		public ReadOnlySequence<byte> Build()
		{
			if (segmentList.Count == 0)
			{
				return ReadOnlySequence<byte>.Empty;
			}
			if (segmentList.Count == 1)
			{
				return new ReadOnlySequence<byte>(segmentList[0].Memory);
			}
			long running = 0;
			UnitList<ByteReadSequence> spanList = segmentList;
			for (int i = 0; i < spanList.Count; i++)
			{
				ByteReadSequence next = i < spanList.Count - 1 ? spanList[i + 1] : null;
				spanList[i].SetRunningIndexAndNext(running, next);
				running += spanList[i].Memory.Length;
			}
			ByteReadSequence firstSegment = spanList[0];
			ByteReadSequence lastSegment = spanList[spanList.Count - 1];
			return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length);
		}

		
	}
}
