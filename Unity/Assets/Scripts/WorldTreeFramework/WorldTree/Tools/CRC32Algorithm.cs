/****************************************

* 作者： 闪电黑客
* 日期： 2024/03/18 17:08:22

* 描述： CRC32校验工具类
*
*/

using System;
using System.Security.Cryptography;

namespace WorldTree
{
	/// <summary>
	/// 安全代理
	/// </summary>
	public class SafeProxy
	{
		private const uint Poly = 0xedb88320u;
		private readonly uint[] _table = new uint[16 * 256];

		internal SafeProxy()
		{
			Init(Poly);
		}

		public void Init(uint poly)
		{
			var table = _table;
			for (uint i = 0; i < 256; i++)
			{
				uint res = i;
				for (int t = 0; t < 16; t++)
				{
					for (int k = 0; k < 8; k++) res = (res & 1) == 1 ? poly ^ (res >> 1) : (res >> 1);
					table[(t * 256) + i] = res;
				}
			}
		}

		public uint Append(uint crc, byte[] input, int offset, int length)
		{
			uint crcLocal = uint.MaxValue ^ crc;

			uint[] table = _table;
			while (length >= 16)
			{
				var a = table[(3 * 256) + input[offset + 12]]
					^ table[(2 * 256) + input[offset + 13]]
					^ table[(1 * 256) + input[offset + 14]]
					^ table[(0 * 256) + input[offset + 15]];

				var b = table[(7 * 256) + input[offset + 8]]
					^ table[(6 * 256) + input[offset + 9]]
					^ table[(5 * 256) + input[offset + 10]]
					^ table[(4 * 256) + input[offset + 11]];

				var c = table[(11 * 256) + input[offset + 4]]
					^ table[(10 * 256) + input[offset + 5]]
					^ table[(9 * 256) + input[offset + 6]]
					^ table[(8 * 256) + input[offset + 7]];

				var d = table[(15 * 256) + ((byte)crcLocal ^ input[offset])]
					^ table[(14 * 256) + ((byte)(crcLocal >> 8) ^ input[offset + 1])]
					^ table[(13 * 256) + ((byte)(crcLocal >> 16) ^ input[offset + 2])]
					^ table[(12 * 256) + ((crcLocal >> 24) ^ input[offset + 3])];

				crcLocal = d ^ c ^ b ^ a;
				offset += 16;
				length -= 16;
			}

			while (--length >= 0)
				crcLocal = table[(byte)(crcLocal ^ input[offset++])] ^ crcLocal >> 8;

			return crcLocal ^ uint.MaxValue;
		}
	}

	/// <summary>
	/// 这是Crc32算法的.NET安全实现。 该类支持几个方便的静态方法，将CRC返回为UInt32。
	/// </summary>
	public class CRC32Algorithm : HashAlgorithm
	{
		private uint _currentCrc;

		/// <summary>
		/// 初始化一个新的实例类 <see cref="CRC32Algorithm"/> 。
		/// </summary>
		public CRC32Algorithm()
		{
#if !NETCORE13
			HashSizeValue = 32;
#endif
		}

		/// <summary>
		/// 重置算法的内部状态。内部使用。
		/// </summary>
		public override void Initialize()
		{
			_currentCrc = 0;
		}

		/// <summary>
		/// 从给定的缓冲区附加CRC-32
		/// </summary>
		protected override void HashCore(byte[] input, int offset, int length)
		{
			_currentCrc = AppendInternal(_currentCrc, input, offset, length);
		}

		/// <summary>
		/// 计算CRC-32 <see cref="HashCore"/>
		/// </summary>
		protected override byte[] HashFinal()
		{
			if (BitConverter.IsLittleEndian)
				return new[] { (byte)_currentCrc, (byte)(_currentCrc >> 8), (byte)(_currentCrc >> 16), (byte)(_currentCrc >> 24) };
			else
				return new[] { (byte)(_currentCrc >> 24), (byte)(_currentCrc >> 16), (byte)(_currentCrc >> 8), (byte)_currentCrc };
		}

		/// <summary>
		/// 从多个缓冲区计算CRC-32。 多次调用此方法以链接多个缓冲区。
		/// </summary>
		/// <param name="initial">最初的CRC值为算法。对于第一个缓冲区，它为零。 后续缓冲区的初始值应设置为此方法的先前调用返回的CRC值。</param>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <param name="offset">缓冲区中输入数据的偏移量。</param>
		/// <param name="length">缓冲区中输入数据的长度。</param>
		/// <returns>到目前为止处理的所有缓冲区的累积CRC-32。</returns>
		public static uint Append(uint initial, byte[] input, int offset, int length)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (offset < 0 || length < 0 || offset + length > input.Length)
				throw new ArgumentOutOfRangeException("length");
			return AppendInternal(initial, input, offset, length);
		}

		/// <summary>
		/// 从多个缓冲区计算CRC-32。 多次调用此方法以链接多个缓冲区。
		/// </summary>
		/// <param name="initial">最初的CRC值为算法。对于第一个缓冲区，它为零。 后续缓冲区的初始值应设置为此方法的先前调用返回的CRC值。</param>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <returns>到目前为止处理的所有缓冲区的累积CRC-32。</returns>
		public static uint Append(uint initial, byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException();
			return AppendInternal(initial, input, 0, input.Length);
		}

		/// <summary>
		/// 计算输入缓冲区的CRC-32。
		/// </summary>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <param name="offset">缓冲区中输入数据的偏移量。</param>
		/// <param name="length">缓冲区中输入数据的长度。</param>
		/// <returns>缓冲区中数据的CRC-32。</returns>
		public static uint Compute(byte[] input, int offset, int length)
		{
			return Append(0, input, offset, length);
		}

		/// <summary>
		/// 计算输入缓冲区的CRC-32。
		/// </summary>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <returns>缓冲区的CRC-32。</returns>
		public static uint Compute(byte[] input)
		{
			return Append(0, input);
		}

		/// <summary>
		/// 计算输入缓冲区的CRC-32，并在数据结束后写入它（缓冲区应为其保留4个字节的空间）。可以与 <see cref="IsValidWithCrcAtEnd(byte[],int,int)"/>
		/// </summary>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <param name="offset">缓冲区中输入数据的偏移量。</param>
		/// <param name="length">缓冲区中输入数据的长度。</param>
		/// <returns>缓冲区中数据的CRC-32。</returns>
		public static uint ComputeAndWriteToEnd(byte[] input, int offset, int length)
		{
			if (length + 4 > input.Length)
				throw new ArgumentOutOfRangeException("length", "Length of data should be less than array length - 4 bytes of CRC data");
			var crc = Append(0, input, offset, length);
			var r = offset + length;
			input[r] = (byte)crc;
			input[r + 1] = (byte)(crc >> 8);
			input[r + 2] = (byte)(crc >> 16);
			input[r + 3] = (byte)(crc >> 24);
			return crc;
		}

		/// <summary>
		/// 计算输入缓冲区的CRC-32 - 4个字节，并将其写为缓冲区的最后4个字节。可以与 <see cref="IsValidWithCrcAtEnd(byte[])"/>
		/// </summary>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <returns>缓冲区中数据的CRC-32。</returns>
		public static uint ComputeAndWriteToEnd(byte[] input)
		{
			if (input.Length < 4)
				throw new ArgumentOutOfRangeException("input", "Input array should be 4 bytes at least");
			return ComputeAndWriteToEnd(input, 0, input.Length - 4);
		}

		/// <summary>
		/// 验证源缓冲区中CRC-32数据的正确性，假设CRC-32数据位于缓冲区的末尾，以反向字节顺序排列。可以与 <see cref="ComputeAndWriteToEnd(byte[],int,int)"/>
		/// </summary>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <param name="offset">缓冲区中输入数据的偏移量。</param>
		/// <param name="lengthWithCrc">缓冲区中带有CRC-32字节的输入数据的长度。</param>
		/// <returns>校验和是否有效。</returns>
		public static bool IsValidWithCrcAtEnd(byte[] input, int offset, int lengthWithCrc)
		{
			return Append(0, input, offset, lengthWithCrc) == 0x2144DF1C;
		}

		/// <summary>
		/// 验证源缓冲区中CRC-32数据的正确性，假设CRC-32数据位于缓冲区的末尾，以反向字节顺序排列。可以与 <see cref="ComputeAndWriteToEnd(byte[],int,int)"/>
		/// </summary>
		/// <param name="input">包含要进行校验和的数据的输入缓冲区。</param>
		/// <returns>校验和是否有效。</returns>
		public static bool IsValidWithCrcAtEnd(byte[] input)
		{
			if (input.Length < 4)
				throw new ArgumentOutOfRangeException("input", "Input array should be 4 bytes at least");
			return Append(0, input, 0, input.Length) == 0x2144DF1C;
		}

		/// <summary>
		/// 安全代理
		/// </summary>
		private static readonly SafeProxy _proxy = new SafeProxy();

		/// <summary>
		/// 将指定的字节数组的CRC32值附加到当前CRC32值。
		/// </summary>
		/// <param name="initial">初始CRC32值。</param>
		/// <param name="input">要计算CRC32值的输入数据。</param>
		/// <param name="offset">从中开始使用数据的字节数组的偏移量。</param>
		/// <param name="length">用作数据的字节数组中的字节数。</param>
		/// <returns>更新后的CRC32值。</returns>
		private static uint AppendInternal(uint initial, byte[] input, int offset, int length)
		{
			if (length > 0)
			{
				return _proxy.Append(initial, input, offset, length);
			}
			else
				return initial;
		}
	}
}