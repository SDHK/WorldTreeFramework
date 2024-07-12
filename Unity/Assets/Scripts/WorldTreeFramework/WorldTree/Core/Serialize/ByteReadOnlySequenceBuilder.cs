/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：字节只读序列生成器

*/
using System;
using System.Buffers;

namespace WorldTree
{
	/// <summary>
	/// 字节只读序列生成器
	/// </summary>
	public sealed class ByteReadOnlySequenceBuilder : Unit
	{
		/// <summary>
		/// 片段列表
		/// </summary>
		private UnitList<ByteReadSequenceSegment> segmentList;

		public override void OnCreate()
		{
			Core.PoolGetUnit(out segmentList);
		}

		public override void OnDispose()
		{
			foreach (var item in segmentList) item.Dispose();
			segmentList.Dispose();
			segmentList = null;
		}

		/// <summary>
		/// 添加
		/// </summary>
		public void Add(ReadOnlyMemory<byte> buffer, bool returnToPool)
		{
			Core.PoolGetUnit(out ByteReadSequenceSegment byteReadSequence);
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
			UnitList<ByteReadSequenceSegment> spanList = segmentList;
			for (int i = 0; i < spanList.Count; i++)
			{
				ByteReadSequenceSegment next = i < spanList.Count - 1 ? spanList[i + 1] : null;
				spanList[i].SetRunningIndexAndNext(running, next);
				running += spanList[i].Memory.Length;
			}
			ByteReadSequenceSegment firstSegment = spanList[0];
			ByteReadSequenceSegment lastSegment = spanList[spanList.Count - 1];
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
