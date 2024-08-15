using System.Runtime.CompilerServices;

namespace WorldTree.TreePack.Formatters
{
	public static class UnmanagedFormatterRule
	{
		class Serialize<T> : TreePackSerializeRule<TreePackByteSequence, T>
		{
			protected override void Execute(TreePackByteSequence self, ref T value)
			{
				Unsafe.WriteUnaligned(ref self.GetWriteRefByte(Unsafe.SizeOf<T>()), value);
			}
		}
		class Deserialize<T> : TreePackDeserializeRule<TreePackByteSequence, T>
		{
			protected override void Execute(TreePackByteSequence self, ref T value)
			{
				value = Unsafe.ReadUnaligned<T>(ref self.GetReadRefByte(Unsafe.SizeOf<T>()));
			}
		}
	}
}
