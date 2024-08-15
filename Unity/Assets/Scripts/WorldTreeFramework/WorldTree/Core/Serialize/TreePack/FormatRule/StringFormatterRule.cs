using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WorldTree.TreePack.Formatters
{
	public static class StringFormatterRule
	{
		class Serialize : TreePackSerializeRule<TreePackByteSequence, string>
		{
			protected override void Execute(TreePackByteSequence self, ref string value)
			{
				if (value == null)
				{
					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
					return;
				}
				if (value.Length == 0)
				{
					self.WriteUnmanaged(0);
					return;
				}

				// (int ~utf8-byte-count, int utf16-length, utf8-bytes)
				ReadOnlySpan<char> source = value.AsSpan();
				// UTF8.GetMaxByteCount -> (length + 1) * 3
				int maxByteCount = (source.Length + 1) * 3;

				// write utf16-length ，字符长度
				self.WriteUnmanaged(source.Length);

				//申请总空间，包含utf8长度和数据

				// 由于不知道空间大小，所以字符数*3只是获取一个可能的最大空间，字符最小可能是只占1个字节
				// 头部需要写入byte真实长度，int长度偏移+4
				ref byte destPointer = ref self.GetWriteRefByte(maxByteCount + 4);

				//申请数据空间，byte长度int要写到头部，所以要偏移4
				Span<byte> dest = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref destPointer, 4), maxByteCount);

				// 数据写入到dest，此时拿到了byte的真实长度
				int bytesWritten = Encoding.UTF8.GetBytes(value, dest);

				// 在头部写入 真实长度
				Unsafe.WriteUnaligned(ref destPointer, bytesWritten);

				// 重新定位指针，裁剪空间
				self.Current.SetPoint(bytesWritten + 4);
			}
		}
		class Deserialize : TreePackDeserializeRule<TreePackByteSequence, string>
		{
			protected override void Execute(TreePackByteSequence self, ref string value)
			{
				if (self.ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				else if (length == 0)
				{
					value = string.Empty;
					return;
				}
				else if (self.ReadRemain < length)
				{
					self.LogError($"字符串长度超出数据长度: {length}.");
					value = null;
					return;
				}

				// 获取总数据，包含utf8长度和数据
				//*3只是一个可能的最大空间，字符最小可能是只占1个字节

				//读取 utf8ByteLength
				var utf8BytesLength = self.ReadUnmanaged<int>();

				ref var spanRef = ref self.GetReadRefByte(utf8BytesLength);

				if (utf8BytesLength <= 0)
				{
					ReadOnlySpan<byte> src = MemoryMarshal.CreateReadOnlySpan(ref spanRef, utf8BytesLength);
					value = Encoding.UTF8.GetString(src);
				}
				else
				{
					if (self.ReadRemain < utf8BytesLength)
					{
						self.LogError($"字符串长度超出数据长度: {utf8BytesLength}.");
					}
					var src = MemoryMarshal.CreateReadOnlySpan(ref spanRef, utf8BytesLength);
					value = Encoding.UTF8.GetString(src);
				}
			}
		}
	}
}
