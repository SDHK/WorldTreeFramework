/****************************************

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


    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Node, ComponentOf<INode>
    {
        public TreeNode node;
        public TreeValue<float> valueFloat;
        public TreeValue<int> valueInt;

        public IRuleActuator<ISendRule<float>> ruleActuator;
    }

    class _InitialDomain : AddRule<InitialDomain>
    {
        public override async void OnEvent(InitialDomain self)
        {

            World.Log("初始域启动！！");

            self.AddChild(out self.valueFloat);
            self.AddChild(out self.valueInt);

            self.valueFloat.BindTwoWay(self.valueInt);

            self.valueFloat.GetTween(15f, 5f).SetCurve<CurveBase>().Run().Completed(self.AddComponent(out InitialDomainRuleNode.OnCompleted _));
            self.valueFloat.AddListenerValueChange(self.AddComponent(out InitialDomainRuleNode.Value _));

            await self.AsyncDelay(1);
            self.valueFloat.Dispose();


            ////委托申请赋值
            //self.TryGetRuleActuator(out self.ruleActuator);

            ////委托添加
            //self.ruleActuator.EnqueueReferenced(self.AddComponent(out InitialDomainEvent.Value _));

            ////执行
            //self.ruleActuator.Send(2.5f);
            ////self.ruleActuator.Dispose();
            //test.Dispose();
            //self.ruleActuator.Send(2.7f);

            ////事件调用
            //test.AddComponent(out NodeEvent<float> _).Send(1.02f);

        }
    }
    class InitialDomainUpdateRule : UpdateRule<InitialDomain>
    {
        public override void OnEvent(InitialDomain self, float deltaTime)
        {
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
                self.AddComponent(out TreeNode _).Test().Coroutine();
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

        public void Test(InitialDomain self)
        {


        }

    }



    public static class InitialDomainRuleNode
    {
        public class Value : RuleNode<InitialDomain>, AsRule<ISendRule<float>> { }
        public class OnCompleted : RuleNode<InitialDomain>, AsRule<ISendRule> { }
    }


    public static class InitialDomainRuleNodeRule
    {
        class ValueSendRule : SendRule<InitialDomainRuleNode.Value, float>
        {
            public override void OnEvent(InitialDomainRuleNode.Value self, float value)
            {
                World.Log($"数值变化： {value}");
            }
        }

        class OnCompletedSendRule : SendRule<InitialDomainRuleNode.OnCompleted>
        {
            public override void OnEvent(InitialDomainRuleNode.OnCompleted self)
            {
                World.Log("渐变结束：!!!!");
            }
        }
    }


}
