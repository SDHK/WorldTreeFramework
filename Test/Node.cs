using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldTree;

public class Node : Entity
{

}

class NodeUpdateSystem : UpdateSystem<Node>
{
    public override void Update(Node self, float deltaTime)
    {
        Debug.Log("Update");
        if(Input.GetKeyDown(KeyCode.Space))
        {
            self.SetActive(!self.activeToggle);
        }


        if (Input.GetKeyDown(KeyCode.Return))
        {
            self.AddComponent<Node1>().SetActive(!self.AddComponent<Node1>().activeToggle);
        }
    }
}

class NodeEnableSystem : EnableSystem<Node>
{
    public override void OnEnable(Node self)
    {
        Debug.Log("OnEnable!!");
    }
}

class NodeDisableSystem : DisableSystem<Node>
{
    public override void OnDisable(Node self)
    {
        Debug.Log("OnDisable!!");
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