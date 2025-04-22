namespace WorldTree
{
	public static class CreateWorldConfigGroupRule
	{
		class Add : AddRule<CreateWorldConfigGroup>
		{
			protected override void Execute(CreateWorldConfigGroup self)
			{
				int id = 0;
				self.AddGeneric(id++, out CreateWorldConfig config);
				config.Process = 1;
				config.Zone = 1;
				config.WorldType = "GameWorld";
				config.WorldName = "GameWorld";
			}
		}
	}
}
