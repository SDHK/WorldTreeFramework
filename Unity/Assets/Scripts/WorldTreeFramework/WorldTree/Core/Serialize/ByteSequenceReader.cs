/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：字节序列只读器

*/
using System;
using System.Buffers;

namespace WorldTree
{

	public static class ByteSequenceReaderRule
	{
		class Add : AddRule<ByteSequenceReader>
		{
			protected override void Execute(ByteSequenceReader self)
			{
				self.Core.PoolGetUnit(out self.segmentList);
			}
		}

		class Remove : RemoveRule<ByteSequenceReader>
		{
			protected override void Execute(ByteSequenceReader self)
			{
				foreach (var item in self.segmentList) item.Dispose();
				self.segmentList.Dispose();
				self.segmentList = null;
			}
		}
	}

	/// <summary>
	/// 字节序列只读器
	/// </summary>
	public sealed class ByteSequenceReader : Node
		,AsAwake
		,AsComponentBranch
		,TempOf<INode>
	{
		/// <summary>
		/// 片段列表
		/// </summary>
		public UnitList<ByteReadSequence> segmentList;

		/// <summary>
		/// 添加
		/// </summary>
		public void Add(ReadOnlyMemory<byte> buffer, bool returnToPool)
		{
			Core.PoolGetUnit(out ByteReadSequence byteReadSequence);
			byteReadSequence.SetBuffer(buffer, returnToPool);
			segmentList.Add(byteReadSequence);
		}

		/// <summary>
		/// 尝试获取单个内存
		/// </summary>
		public bool TryGetSingleMemory(out ReadOnlyMemory<byte> memory)
		{
			if (segmentList.Count == 1)
			{
				memory = segmentList[0].Memory;
				return true;
			}
			memory = default;
			return false;
		}

		/// <summary>
		/// 创建只读序列
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

		/// <summary>
		/// 重置
		/// </summary>
		public void Reset()
		{
			foreach (var item in segmentList)
			{
				item.Dispose();
			}
			segmentList.Clear();
		}
	}
}
