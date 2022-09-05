using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldTree;

public class Node : Entity
{
    
}


public class NodeSendSystem : SendSystem<Node,float>
{
    public override void Event(Node self,float i)
    {
        Debug.Log("!!!"+i );
    }
}
