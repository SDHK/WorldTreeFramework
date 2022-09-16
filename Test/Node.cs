using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldTree;

public class Node : Entity
{

}


public class NodeSendSystem : SendSystem<Node, float>
{
    public override void Event(Node self, float i)
    {
        Debug.Log("!!!" + i);
    }
}



public interface IAddSendSystem : ISendSystem<float> { }

public class NodeAddSendSystem : SendSystem<Node, float>, IAddSendSystem
{
    public override Type SystemType => typeof(IAddSendSystem);
    public override void Event(Node self, float arg1)
    {
        World.Log($"测试事件：{arg1}");
    }
}
