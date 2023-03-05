using UnityEngine;
using UnityEngine.EventSystems;
using WorldTree;
using WorldTree.Internal;





public class TreeNode : Node
{

    public static bool bit = true;
    public async TreeTask Test()
    {

        World.Log("1！");

        await this.AsyncDelay(3);

        await T2();

        //World.Log("2！");
        //await this.AsyncDelay(3);
        //World.Log("3！");

        //await this.AsyncDelay(3);

        //World.Log("4！");
        //await this.AsyncDelay(3);
        //World.Log("5！");
        //await this.AsyncDelay(3);
        //World.Log("6！");
        //await this.AsyncDelay(3);
        //World.Log("7！");
    }

    public async TreeTask T2()
    {
        World.Log("T2 1！");

        await this.AsyncDelay(3);

        World.Log("T2 2！");

        await this.AsyncTaskCompleted();

        await T3();

        await this.AsyncTaskCompleted();


    }
    public async TreeTask T3()
    {
        World.Log("T3 1！");

        await this.AsyncDelay(3);

        World.Log("T3 2！");

        await this.AsyncTaskCompleted();

        World.Log(await T4());

    }

    public async TreeTask<int> T4()
    {
        World.Log("T4 1！");

        await this.AsyncDelay(3);
        World.Log("T4 2！");

        await this.AsyncTaskCompleted();

        return 10021;
    }
}
class NodeNewSystem : NewSystem<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("NewSystem!");
    }
}
class NodeGetSystem : GetSystem<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("GetSystem!");
    }
}
class NodeAddSystem : AddRule<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("AddSystem!");
    }
}
class NodeEnableSystem : EnableRule<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("EnableSystem!");
    }
}

class NodeDisableSystem : DisableRule<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("DisableSystem!");
    }
}

class NodeUpdateSystem : UpdateRule<TreeNode>
{
    public override void OnEvent(TreeNode self, float deltaTime)
    {
        Debug.Log("UpdateSystem!");
    }
}


class NodeLateUpdateSystem : LateUpdateSystem<TreeNode>
{
    public override void OnEvent(TreeNode self, float deltaTime)
    {
        Debug.Log("LateUpdateSystem!");
    }
}
class NodeFixedUpdateSystem : FixedUpdateSystem<TreeNode>
{
    public override void OnEvent(TreeNode self, float deltaTime)
    {
        Debug.Log("FixedUpdateSystem!");
    }
}

class NodeRemoveSystem : RemoveRule<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("RemoveSystem!");
    }
}
class NodeRecycleSystem : RecycleSystem<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("RecycleSystem!");
    }
}
class NodeDestroySystem : DestroySystem<TreeNode>
{
    public override void OnEvent(TreeNode self)
    {
        Debug.Log("DestroySystem!");
    }
}








public class Node1 : Node
{

}

class Node1AddSystem : AddRule<Node1>
{
    public override void OnEvent(Node1 self)
    {
        Debug.Log("Node1 OnAdd!!");
    }
}
class Node1EnableSystem : EnableRule<Node1>
{
    public override void OnEvent(Node1 self)
    {
        Debug.Log("Node1 OnEnable!!");
    }
}

class Node1DisableSystem : DisableRule<Node1>
{
    public override void OnEvent(Node1 self)
    {
        Debug.Log("Node1 OnDisable!!");
    }
}


public class Node2 : Node
{

}
class Node2EnableSystem : EnableRule<Node2>
{
    public override void OnEvent(Node2 self)
    {
        Debug.Log("Node2 OnEvent!!");
    }
}

class Node2DisableSystem : DisableRule<Node2>
{
    public override void OnEvent(Node2 self)
    {
        Debug.Log("Node2 OnEvent!!");
    }
}