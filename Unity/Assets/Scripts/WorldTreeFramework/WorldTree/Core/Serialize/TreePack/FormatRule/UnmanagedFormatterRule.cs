using System.Runtime.CompilerServices;

namespace WorldTree.TreePack.Formatters
{
	public static class UnmanagedFormatterRule
	{
		class Serialize<T> : SerializeRule<ByteSequence, T>
		{
			protected override void Execute(ByteSequence self, ref T value)
			{
				Unsafe.WriteUnaligned(ref self.GetWriteRefByte(Unsafe.SizeOf<T>()), value);
			}
		}
		class Deserialize<T> : DeserializeRule<ByteSequence, T>
		{
			protected override void Execute(ByteSequence self, ref T value)
			{
				value = Unsafe.ReadUnaligned<T>(ref self.GetReadRefByte(Unsafe.SizeOf<T>()));
			}
		}
	}
}
