namespace WorldTree.TreePackFormats
{
	public static class StringFormatRule
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
