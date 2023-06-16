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



    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Node, ComponentOf<INode>
        , AsRule<IFixedUpdateRule>
        , AsRule<ILateUpdateRule>
    {
        public TreeNode node;
        public TreeValue<float> valueFloat;
        public TreeValue<int> valueInt;

    }

    public static class InitialDomainRule
    {
        class AddRule : AddRule<InitialDomain>
        {
            public override void OnEvent(InitialDomain self)
            {

                World.Log("初始域启动！！");

                using (self.PoolGet(out UnitQueue<int> queue))
                {

                    World.Log("balabal！！");
                }



                //self.AddChild(out self.valueFloat);
                //self.AddChild(out self.valueInt);
                //self.valueFloat.Bind(self.valueInt);



                //self.AddComponent(out TreeTaskToken treeTaskToken);
                //self.AddComponent(out TreeNode _).Test().Coroutine(treeTaskToken);

            }

      
        }

        class UpdateRule : UpdateRule<InitialDomain>
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
                    //self.AddComponent(out TreeNode _);
                    self.AddComponent(out TreeTaskToken treeTaskToken);
                    self.AddComponent(out TreeNode _).Test().Coroutine(treeTaskToken);
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken).Stop();
                    //self.AddComponent(out TreeNode _).SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken).Continue();
                    //self.AddComponent(out TreeNode _).SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken).Cancel();
                    //self.RemoveComponent<TreeNode>();
                }
            }


        }
    }

}
