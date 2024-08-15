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
				ref byte destPointer = ref self.GetWriteRefByte(maxByteCount + 4); // header int长度是4

				//申请数据空间，byte长度int要写到头部，所以要偏移4
				Span<byte> dest = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref destPointer, 4), maxByteCount);
				// 数据写入到dest
				int bytesWritten = Encoding.UTF8.GetBytes(value, dest);

				// 在头中写入写入的 UTF8-length，即 ~length
				Unsafe.WriteUnaligned(ref destPointer, bytesWritten);

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
				ref var spanRef = ref self.GetReadRefByte((length + 1) * 3 + 4);

				string str;
				//读取 utf8Length
				var utf16Length = Unsafe.ReadUnaligned<int>(ref spanRef);

				if (utf16Length <= 0)
				{
					ReadOnlySpan<byte> src = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref spanRef, 4), length);
					value = Encoding.UTF8.GetString(src);
				}
				else
				{
					// 检查格式错误的 utf16Length
					var max = unchecked((length + 1) * 3);
					if (max < 0) max = int.MaxValue;
					if (max < utf16Length)
					{
						self.LogError($"字符串长度错误: {length}.");
					}

					var src = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref spanRef, 4), length);
					value = Encoding.UTF8.GetString(src);
				}
			}
		}
	}
}
