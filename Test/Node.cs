using UnityEngine;
using UnityEngine.EventSystems;
using WorldTree;
using WorldTree.Internal;

public class Node : Entity
{

    public static bool bit = true;
    public async AsyncTask Test()
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

    public async AsyncTask T2()
    {
        World.Log("T2 1！");

        await this.AsyncDelay(3);

        World.Log("T2 2！");

        await this.AsyncTaskCompleted();

        await T3();

        await this.AsyncTaskCompleted();


    }
    public async AsyncTask T3()
    {
        World.Log("T3 1！");

        await this.AsyncDelay(3);

        World.Log("T3 2！");

        await this.AsyncTaskCompleted();

        World.Log(await T4());

    }

    public async AsyncTask<int> T4()
    {
        World.Log("T4 1！");

        await this.AsyncDelay(3);
        World.Log("T4 2！");

        await this.AsyncTaskCompleted();

        return 10021;
    }
}
//class NodeNewSystem : NewSystem<Node>
//{
//    public override void OnNew(Node self)
//    {
//        Debug.Log("OnNew!");
//    }
//}
//class NodeGetSystem : GetSystem<Node>
//{
//    public override void OnGet(Node self)
//    {
//        Debug.Log("OnGet!");
//    }
//}
//class NodeAddSystem : AddSystem<Node>
//{
//    public override void OnAdd(Node self)
//    {
//    }
//}
//class NodeEnableSystem : EnableSystem<Node>
//{
//    public override void OnEnable(Node self)
//    {
//        Debug.Log("OnEnable!");
//    }
//}

//class NodeDisableSystem : DisableSystem<Node>
//{
//    public override void OnDisable(Node self)
//    {
//        Debug.Log("OnDisable!");
//    }
//}

//class NodeUpdateSystem : UpdateSystem<Node>
//{
//    public override void Update(Node self, float deltaTime)
//    {
//        Debug.Log("Update!");
//    }
//}


//class NodeLateUpdateSystem : LateUpdateSystem<Node>
//{
//    public override void LateUpdate(Node self, float deltaTime)
//    {
//        Debug.Log("LateUpdate!");
//    }
//}
//class NodeFixedUpdateSystem : FixedUpdateSystem<Node>
//{
//    public override void FixedUpdate(Node self, float deltaTime)
//    {
//        Debug.Log("FixedUpdate!");
//    }
//}

//class NodeRemoveSystem : RemoveSystem<Node>
//{
//    public override void OnRemove(Node self)
//    {
//        Debug.Log("OnRemove!");
//    }
//}
//class NodeRecycleSystem : RecycleSystem<Node>
//{
//    public override void OnRecycle(Node self)
//    {
//        Debug.Log("OnRecycle!");
//    }
//}
//class NodeDestroySystem : DestroySystem<Node>
//{
//    public override void OnDestroy(Node self)
//    {
//        Debug.Log("OnDestroy!");
//    }
//}








public class Node1 : Entity
{

}

class Node1AddSystem : AddSystem<Node1>
{
    public override void OnAdd(Node1 self)
    {
        Debug.Log("Node1 OnAdd!!");
    }
}
class Node1EnableSystem : EnableSystem<Node1>
{
    public override void OnEnable(Node1 self)
    {
        Debug.Log("Node1 OnEnable!!");
    }
}

class Node1DisableSystem : DisableSystem<Node1>
{
    public override void OnDisable(Node1 self)
    {
        Debug.Log("Node1 OnDisable!!");
    }
}


public class Node2 : Entity
{

}
class Node2EnableSystem : EnableSystem<Node2>
{
    public override void OnEnable(Node2 self)
    {
        Debug.Log("Node2 OnEnable!!");
    }
}

class Node2DisableSystem : DisableSystem<Node2>
{
    public override void OnDisable(Node2 self)
    {
        Debug.Log("Node2 OnDisable!!");
    }
}