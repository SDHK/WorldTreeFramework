namespace WorldTree
{
	public static partial class EntryRule
	{
		class Add : AddRule<Entry>
		{
			protected override void Execute(Entry self)
			{
				self.Log("入口！！！");
				self.AddComponent(out InitialDomain _);
			}
		}
	}
}
