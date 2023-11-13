/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/28 12:50

* 描述： 

*/

using WorldTree;


public class TestNode : Node
	, AsRule<IFixedUpdateTimeRule>
	, AsRule<ILateUpdateTimeRule>
	, AsRule<IAwakeRule>
{
	public static bool bit = true;

	public async TreeTask Test1(bool b = false)
	{
		World.Log("1！");
		await Test2();
		World.Log("1结束！");

	}

	public async TreeTask Test2(bool b = false)
	{
		World.Log("2！");
		await Test3();
		World.Log("2结束！");
	}

	public async TreeTask Test3(bool b = false)
	{
		World.Log("3！");
		if (b) await this.TreeTaskCompleted();
		World.Log("3结束！");
	}

	public async TreeTask Test()
	{
		await this.AsyncDelay(3);

		World.Log("0！");

		//await T2();


		World.Log("1！");
		await this.TreeTaskCompleted();
		World.Log("2！");

		await this.TreeTaskCompleted();
		World.Log("3！");

		await this.TreeTaskCompleted();
		World.Log("4！");
		//await this.AsyncDelay(10);
		await T5();
		await this.TreeTaskCompleted();
		World.Log("5！");

		await this.TreeTaskCompleted();
		await this.AsyncDelay(3);

	}

	public async TreeTask T2()
	{
		//await this.AsyncDelay(5);

		World.Log("T2 1！");

		await T3();
		//await this.TreeTaskCompleted();
	}
	public async TreeTask T3()
	{
		World.Log("T3 1！");

		//await this.TreeTaskCompleted();
		var tk = await this.TreeTaskTokenCatch();
		World.Log($"TK!!!!!!!{tk.Id}");

		//World.Log(await T4());

		//await T5();

	}

	public async TreeTask<int> T4()
	{
		World.Log("T4 1！");

		await this.TreeTaskCompleted();

		return 10021;
	}

	public async TreeTask T5()
	{
		World.Log("T5 1！");
		await T6();
		await this.TreeTaskCompleted();
	}
	public async TreeTask T6()
	{
		World.Log("T6 1！");
		await T7();

		await this.TreeTaskCompleted();
	}
	public async TreeTask T7()
	{
		World.Log("T7 1！");
		var tk = await this.TreeTaskTokenCatch();
		World.Log($"TK!!!!!!!{tk.Id}");

		await this.TreeTaskCompleted();
	}
	//}
	//class NodeNewSystem : NewRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("NewSystem!");
	//    }
	//}
	//class NodeGetSystem : GetRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("GetSystem!");
	//    }
	//}
	//class NodeAddSystem : AddRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("AddSystem!");
	//    }
	//}
	//class NodeEnableSystem : EnableRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("EnableSystem!");
	//    }
	//}

	//class NodeDisableSystem : DisableRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("DisableSystem!");
	//    }
	//}

	class NodeUpdateSystem : UpdateTimeRule<TestNode>
	{
		protected override void OnEvent(TestNode self, float deltaTime)
		{
			//Debug.Log("UpdateSystem!");
		}
	}


	//class NodeLateUpdateSystem : LateUpdateRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self, float deltaTime)
	//    {
	//        Debug.Log("LateUpdateSystem!");
	//    }
	//}
	//class NodeFixedUpdateSystem : FixedUpdateRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self, float deltaTime)
	//    {
	//        Debug.Log("FixedUpdateSystem!");
	//    }
	//}

	//class NodeRemoveSystem : RemoveRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("RemoveSystem!");
	//    }
	//}
	//class NodeRecycleSystem : RecycleRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("RecycleSystem!");
	//    }
	//}
	//class NodeDestroySystem : DestroyRule<TreeNode>
	//{
	//    protected override void OnEvent(TreeNode self)
	//    {
	//        Debug.Log("DestroySystem!");
	//    }
	//}


}