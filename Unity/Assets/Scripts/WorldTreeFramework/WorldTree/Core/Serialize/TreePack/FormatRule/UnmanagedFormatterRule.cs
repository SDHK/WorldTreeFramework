using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree.TreePack.Formatters
{
	public static class UnmanagedFormatterRule
	{
		class Serialize<T> : SerializeRule<ByteSequence, T>
			where T : unmanaged
		{
			protected override void Execute(ByteSequence self, ref T value)
			{
				Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(self.GetWriteSpan(Unsafe.SizeOf<T>())), value);
			}
		}
		class Deserialize<T> : DeserializeRule<ByteSequence, T>
			where T : unmanaged
		{
			protected override void Execute(ByteSequence self, ref T value)
			{
				value = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(self.GetReadSpan(Unsafe.SizeOf<T>())));
			}
		}
	}
}
