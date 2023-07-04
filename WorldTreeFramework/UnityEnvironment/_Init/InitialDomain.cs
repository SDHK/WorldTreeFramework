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

    //public class NodeListenerAddRule : ListenerAddRule<InitialDomain>
    //{
    //    public override void OnEvent(InitialDomain self, INode node)
    //    {
    //        World.Log($"NodeListenerAdd: {node.Id} _  {node.Type} ");
    //    }
    //}

    //public class NodeListenerInitialDomainAddRule : ListenerAddRule<InitialDomain, TreeNode, IRule>
    //{
    //    public override void OnEvent(InitialDomain self, TreeNode node)
    //    {
    //        World.Log($"NodeListener InitialDomain Add: {self.Id} _  {self.Type} ");
    //    }
    //}



    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : NodeListener, ComponentOf<INode>
        , AsRule<IFixedUpdateRule>
        , AsRule<ILateUpdateRule>
    {
        public TreeNode node;
        public TreeValue<float> valueFloat;
        public TreeValue<int> valueInt;
        public int int1;

    }

    public static class InitialDomainRule
    {
        class AddRule : AddRule<InitialDomain>
        {
            public override  void OnEvent(InitialDomain self)
            {

                World.Log("初始域启动！！");
                self.ListenerSwitchesEntity<INode>();

                //self.AddChild(out self.valueFloat);
                //self.AddChild(out self.valueInt);
                //self.valueFloat.Bind(self.valueInt);



                //self.AddComponent(out TreeTaskToken treeTaskToken);
                //self.AddComponent(out TreeNode _).Test().Coroutine(treeTaskToken);


                self.AddComponent(out TreeNode _);



            }


        }

        class UpdateRule : UpdateRule<InitialDomain>
        {
            public override void OnEvent(InitialDomain self, float deltaTime)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    self.Test().Coroutine();
                    //self.valueFloat.Value += 1.5f;
                    //World.Log($"A {self.valueFloat.Value} : {self.valueInt.Value}");
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    self.valueInt.Value += 1;
                    World.Log($"S {self.valueFloat.Value} : {self.valueInt.Value}");

                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken);
                    self.AddComponent(out TreeNode _).Test().Coroutine(treeTaskToken);
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken).Stop();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken).Continue();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken).Cancel();
                    //treeTaskToken.Dispose();
                }
            }

        }
    }
    public static class ITest
    {
        public static async TreeTask Test(this InitialDomain self)
        {
            using (await self.AsyncLock(self.Id))
            {
                await self.AsyncDelay(10);
                self.int1 += 1;
                World.Log($"C {self.int1} ");
            }
        }
    }
}
