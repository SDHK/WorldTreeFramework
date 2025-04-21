namespace WorldTree
{
	public static class CreateWorldConfigGroupRule
	{
		class Add : AddRule<CreateWorldConfigGroup>
		{
			protected override void Execute(CreateWorldConfigGroup self)
			{
				self.AddGeneric(1, out CreateWorldConfig config);
				config.Process = 1;
				config.Zone = 1;
				config.WorldType = "GameWorld";
				config.WorldName = "GameWorld";
			}
		}
	}
}
