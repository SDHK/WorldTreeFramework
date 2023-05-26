﻿/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件

*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TreeEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using WorldTree.Internal;

namespace WorldTree
{

    public struct Pointer<N>
    where N : class, INode
    {
        public WorldTreeCore core;
        public long id;
        public INode node;
    }


    //public class NodeAddRule : AddRule<Node>
    //{
    //    public override void OnEvent(Node self)
    //    {

    //        // World.Log($"NodeAdd: {self.Id} _  {self.Type} ");
    //    }
    //}

    //public class NodeListenerAddRule : ListenerAddRule<Node>
    //{
    //    public override void OnEvent(Node self, INode node)
    //    {
    //        World.Log($"NodeListenerAdd: {self.Id} _  {self.Type} ");

    //    }
    //}

    //public class NodeListenerInitialDomainAddRule : ListenerAddRule<Node, InitialDomain, IRule>
    //{

    //    public override void OnEvent(Node self, InitialDomain node)
    //    {
    //        World.Log($"NodeListenerInitialDomainAdd: {self.Id} _  {self.Type} ");

    //    }
    //}

    public class TreeNode2<T> : Node, ChildOf<INode>
    {
        public T Value;
    }

    public class TreeNode2AddRule<T> : AddRule<TreeNode2<T>>
    {
        public override void OnEvent(TreeNode2<T> self)
        {
            World.Log($"泛型添加: {typeof(T)}");
        }
    }


    public class TreeNode2Child<T> : TreeNode2<T>, ChildOf<INode>
    {

    }

    public class TreeNode2ChildAddRule<T> : AddRule<TreeNode2Child<T>>
    {
        public override void OnEvent(TreeNode2Child<T> self)
        {
            World.Log($"泛型子类添加: {typeof(T)}");
        }
    }

    //执行节点
    public abstract class ActuatorNodeRule<P, N> : SendRule<N>
        where N : RuleNode<P>, INode, AsRule<ISendRule>
        where P : class, INode
    {
        public override void OnEvent(N self) => OnEvent(self.Node);
        public abstract void OnEvent(P self);
    }


    class SetResultSendRule : ActuatorNodeRule<TreeTask, TreeTask.SetResultRuleNode>
    {
        public override void OnEvent(TreeTask self)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Node, ComponentOf<INode>
    {
        public TreeNode node;
        public TreeValue<float> valueFloat;
        public TreeValue<int> valueInt;

        public IRuleActuator<ISendRuleBase<float>> ruleActuator;

        public class Value : RuleNode<InitialDomain>, AsRule<ISendRuleBase<float>> { }
        public class OnCompleted : RuleNode<InitialDomain>, AsRule<ISendRuleBase> { }
    }



    public static class InitialDomainRule
    {
        class AddRule : AddRule<InitialDomain>
        {
            public override void OnEvent(InitialDomain self)
            {
                GlobalRuleActuator<IUpdateRule> a = new();
                IRuleActuator<IUpdateRule> b = a;
                a.Send(1f);

                World.Log("初始域启动！！");
            }
        }

        class UpdateRule : UpdateRule<InitialDomain>
        {
            public override void OnEvent(InitialDomain self, float deltaTime)
            {
                World.Log("初始域UpdateRule！！");

                if (Input.GetKeyDown(KeyCode.A))
                {
                    self.valueFloat.Value += 1.5f;
                    World.Log($"A {self.valueFloat.Value} : {self.valueInt.Value}");
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    self.valueInt.Value += 1;
                    World.Log($"S {self.valueFloat.Value} : {self.valueInt.Value}");

                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    self.AddComponent(out TreeNode _);
                    //self.AddComponent(out TreeNode _).Test().Coroutine();
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    self.AddComponent(out TreeNode _).SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    self.AddComponent(out TreeNode _).SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    self.RemoveComponent<TreeNode>();
                }
            }


        }


        class ValueSendRule : SendRule<InitialDomain.Value, float>
        {
            public override void OnEvent(InitialDomain.Value self, float value)
            {
                World.Log($"数值变化： {value}");
            }
        }

        class OnCompletedSendRule : SendRule<InitialDomain.OnCompleted>
        {
            public override void OnEvent(InitialDomain.OnCompleted self)
            {
                World.Log("渐变结束：!!!!");
            }
        }


        public static void Test(this InitialDomain self)
        {


        }

    }

}
