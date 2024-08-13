/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 字节序列片段接口
	/// </summary>
	public interface IByteSequence
	{
		/// <summary>
		/// 数据长度
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// 读取片段指针位置
		/// </summary>
		public int ReadPoint { get; }

		/// <summary>
		/// 读取数据的剩余长度
		/// </summary>
		public int ReadRemain { get; }

		/// <summary>
		/// 获取写入操作跨度
		/// </summary>
		public Span<byte> GetWriteSpan(int sizeHint);

		/// <summary>
		/// 获取写入片段引用
		/// </summary>
		public ref byte GetWriteRefByte(int sizeHint);

		/// <summary>
		/// 获取读取操作跨度
		/// </summary>
		public Span<byte> GetReadSpan(int sizeHint);

		/// <summary>
		/// 获取读取片段引用
		/// </summary>
		public ref byte GetReadRefByte(int sizeHint);

		/// <summary>
		/// 添加Byte数组
		/// </summary>
		public void SetBytes(byte[] array);

		/// <summary>
		/// 转换为Byte数组
		/// </summary>
		public byte[] ToBytes();



		/// <summary>
		/// 清空
		/// </summary>
		public void Clear();

		#region 写入


		/// <summary>
		/// 写入固定长度数值
		/// </summary>
			
		public void WriteUnmanaged<T1>(in T1 value1) where T1 : unmanaged;

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		public void WriteUnmanaged<T1, T2>(in T1 value1, in T2 value2)
			where T1 : unmanaged
			where T2 : unmanaged;

		#endregion


		#region 读取

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		public T1 ReadUnmanaged<T1>() where T1 : unmanaged;

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		public T1 ReadUnmanaged<T1>(out T1 value1) where T1 : unmanaged;

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		public void ReadUnmanaged<T1, T2>(out T1 value1, out T2 value2)
			where T1 : unmanaged
			where T2 : unmanaged;

		#endregion

	}

}
