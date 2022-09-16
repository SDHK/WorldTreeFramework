using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldTree;

public class Node : Entity
{

}


public class NodeSendSystem : SendSystem<Node,ISendSystem<float>, float>
{
    public override void Event(Node self, float i)
    {
        Debug.Log("!!!" + i);
    }
}



//public interface IAddSendSystem :ISendSystem<float>{ }
public  abstract class NodeAddSendSystem : SendSystem<Node, NodeAddSendSystem, float>{}
public abstract class NodeAddSendSystem1<T> : NodeAddSendSystem
    where T : Node
{
    public override void Event(Node self, float arg1) => Event1(self as T, arg1);
    public abstract void Event1(Node self, float arg1);
}


public class NodeAddSendSystem2 : NodeAddSendSystem1<Node>
{
    public override void Event1(Node self, float arg1)
    {

    }
}
