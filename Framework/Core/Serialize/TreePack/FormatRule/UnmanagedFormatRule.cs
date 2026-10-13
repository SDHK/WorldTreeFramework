/****************************************

* 作者：闪电黑客
* 日期：2024/8/12 11:49

* 描述：

*/
using System.Runtime.CompilerServices;

namespace WorldTree.TreePackFormats
{
	public static class UnmanagedFormatRule
	{
		class Serialize<T> : TreePackSerializeUnmanagedRule<T>
		{
			protected override void Execute(TreePackByteSequence self, ref T value)
			{
				Unsafe.WriteUnaligned(ref self.GetWriteRefByte(Unsafe.SizeOf<T>()), value);
			}
		}
		class Deserialize<T> : TreePackDeserializeUnmanagedRule<T>
		{
			protected override void Execute(TreePackByteSequence self, ref T value)
			{
				value = Unsafe.ReadUnaligned<T>(ref self.GetReadRefByte(Unsafe.SizeOf<T>()));
			}
		}
	}
}
