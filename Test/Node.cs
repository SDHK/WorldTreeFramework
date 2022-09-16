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



public interface IAddSendSystem :ISendSystem<float>{ }
public abstract class NodeAddSendSystem : SendSystem<Node, IAddSendSystem, float>
{
    //public override void Event(Node self, float arg1)
    //{
    //    World.Log($"测试事件：{arg1}");
    //}
   
}
public class NodeAddSendSystem1 : NodeAddSendSystem
{
    public override void Event(Node self, float arg1)
    {
        throw new NotImplementedException();
    }
}
