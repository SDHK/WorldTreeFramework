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
        , AsRule<IAddRule>
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
        , AsRule<IAwakeRule>
        , AsRule<IAddRule>
        , AsRule<IUpdateRule>
    {
        public TreeNode node;
        public TreeValue<float> valueFloat;
        public TreeValue<int> valueInt;

        public IRuleActuator<ISendRule<float>> ruleActuator;
    }

    class _InitialDomain : AddRule<InitialDomain>
    {

        public override void OnEvent(InitialDomain self)
        {

            World.Log("初始域启动！！");

            self.AddChild(out self.valueFloat);
            self.AddChild(out self.valueInt);

            self.valueFloat.BindTwoWay(self.valueInt);

            //self.SendRule(default(EventAddRule), 1.01f);

            self.AddChild(out Test<float> test);


            //委托申请赋值
            self.TryGetRuleActuator(out self.ruleActuator);

            //委托添加
            self.ruleActuator.EnqueueReferenced(test.AddComponent(out NodeEvent<float> _));

            //执行
            self.ruleActuator.Send(2.5f);
            //self.ruleActuator.Dispose();
            test.Dispose();
            self.ruleActuator.Send(2.7f);

            //事件调用
            test.AddComponent(out NodeEvent<float> _).Send(1.02f);

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


    //主要测试类型
    public class Test<T> : Node, ChildOf<INode>
        , AsRule<IAwakeRule>
        , AsRule<IAddRule>
    {
    }
    //测试类型添加生命周期
    class TestAddRule<T> : AddRule<Test<T>>
    {
        public override void OnEvent(Test<T> self)
        {
            World.Log("Test泛型添加！！");
            self.AddComponent(out NodeEvent<T> _);

        }
    }

    //声明一个事件节点，指定服务于 Test<T>
    public class NodeEvent<T> : EventNode<Test<T>>
        , AsRule<ISendRule<float>> 
    { }

    //实现事件节点的通用事件类型
    class NodeEventSendRule<T> : SendRule<NodeEvent<T>, float>
    {
        public override void OnEvent(NodeEvent<T> self, float arg1)
        {

            World.Log("！！" + arg1);
        }
    }

}
