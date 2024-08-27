namespace WorldTree
{
	public static partial class EntryRule
	{
		static OnAdd<Entry> onAdd = (self) =>
		{
			self.Log("入口！！！");
			self.AddComponent(out InitialDomain _);
		};
	}
}
