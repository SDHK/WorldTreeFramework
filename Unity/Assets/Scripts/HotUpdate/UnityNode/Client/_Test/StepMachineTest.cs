namespace WorldTree
{
	/// <summary>
	/// 步骤机测试 
	/// </summary>
	public class StepMachineTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<Awake>
		, AsRule<Update>
	{
		/// <summary> 步骤机 </summary>
		public StepMachine stepMachine;
	}
}
