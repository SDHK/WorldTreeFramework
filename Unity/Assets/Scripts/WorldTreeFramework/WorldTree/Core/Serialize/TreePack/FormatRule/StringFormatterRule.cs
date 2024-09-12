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
				self.WriteString(value);
			}
		}
		class Deserialize : TreePackDeserializeRule<TreePackByteSequence, string>
		{
			protected override void Execute(TreePackByteSequence self, ref string value)
			{
				value = self.ReadString();
			}
		}
	}
}
