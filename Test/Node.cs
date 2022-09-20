using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldTree;

public class Node : Entity
{
}
class NodeNewSystem : NewSystem<Node>
{
    public override void OnNew(Node self)
    {
        Debug.Log("OnNew!");
    }
}
class NodeGetSystem : GetSystem<Node>
{
    public override void OnGet(Node self)
    {
        Debug.Log("OnGet!");
    }
}
class NodeEnableSystem : EnableSystem<Node>
{
    public override void OnEnable(Node self)
    {
        Debug.Log("OnEnable!");
    }
}
class NodeUpdateSystem : UpdateSystem<Node>
{
    public override void Update(Node self, float deltaTime)
    {
        Debug.Log("Update!");
    }
}
class NodeLateUpdateSystem : LateUpdateSystem<Node>
{
    public override void LateUpdate(Node self, float deltaTime)
    {
        Debug.Log("LateUpdate!");
    }
}
class NodeFixedUpdateSystem : FixedUpdateSystem<Node>
{
    public override void FixedUpdate(Node self, float deltaTime)
    {
        Debug.Log("FixedUpdate!");
    }
}
class NodeDisableSystem : DisableSystem<Node>
{
    public override void OnDisable(Node self)
    {
        Debug.Log("OnDisable!");
    }
}
class NodeRemoveSystem : RemoveSystem<Node>
{
    public override void OnRemove(Node self)
    {
        Debug.Log("OnRemove!");
    }
}
class NodeDestroySystem : DestroySystem<Node>
{
    public override void OnDestroy(Node self)
    {
        Debug.Log("OnDestroy!");
    }
}








public class Node1 : Entity
{

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