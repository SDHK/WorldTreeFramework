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
    public class InitialDomain : Node, IAwake, ComponentOf<INode>
    {
        public TreeValue<float> A;
        public TreeValue<float> B;
        public Perceptron1 perceptron;
        public MultilayerPerceptronManager multilayerPerceptronManager;
    }

    class _InitialDomain : AddRule<InitialDomain>
    {

        public override void OnEvent(InitialDomain self)
        {
            self.Branch = self;

            var a2 = self.AddChild(out TreeArray<TreeArray<int>> _, 10);
            var m2 = self.AddChild(out TreeMatrix2<int> _, 10, 10);


            self.AddChild(out self.A);//获取
            self.AddChild(out self.B);


            //self.A.Bind(self.B);//单绑
            self.A.BindTwoWay(self.B);//双绑


            World.Log("初始域启动！！");

            self.AddChild(out self.perceptron);
            self.perceptron.Test();

            self.perceptron.Train();

            //using (await self.AsyncLock(0))
            //{
            //    await self.AsyncDelay(3);
            //    self.A.Value++;
            //}


            self.AddComponent(out self.multilayerPerceptronManager);

            self.multilayerPerceptronManager.AddLayer(3);
            self.multilayerPerceptronManager.AddLayer(4);
            self.multilayerPerceptronManager.AddLayer(1);
        }
    }
    class InitialDomainUpdateRule : UpdateRule<InitialDomain>
    {
        public override void OnEvent(InitialDomain self, float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                for (int i = 0; i < 1; i++)
                {
                    Test(self);
                }
                //self.perceptron.Train();
                //Test(self).Coroutine();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                for (int i = 0; i < 10; i++)
                {
                    Test(self);
                }
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                for (int i = 0; i < 100; i++)
                {
                    Test(self);
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                for (int i = 0; i < 1000; i++)
                {
                    Test(self);
                }
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                //self.perceptron.DY();
                self.multilayerPerceptronManager.SetInputs(1, 1, 1);

                World.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");

                //self.B.Value += 1;
                //World.Log($"B  A:{self.A.Value}  B:{self.B.Value}");
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                self.multilayerPerceptronManager.SetInputs(1, 0, 1);
                World.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                self.multilayerPerceptronManager.SetInputs(0, 1, 1);
                World.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                self.multilayerPerceptronManager.SetInputs(0, 0, 1);
                World.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
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
            self.multilayerPerceptronManager.SetInputs(0, 0, 1);
            self.multilayerPerceptronManager.SetOutputs(0);

            self.multilayerPerceptronManager.SetInputs(0, 1, 1);
            self.multilayerPerceptronManager.SetOutputs(1);


            self.multilayerPerceptronManager.SetInputs(1, 0, 1);
            self.multilayerPerceptronManager.SetOutputs(1);


            self.multilayerPerceptronManager.SetInputs(1, 1, 1);
            self.multilayerPerceptronManager.SetOutputs(0);

            self.multilayerPerceptronManager.SetInputs(0, 0, 0);
            self.multilayerPerceptronManager.SetOutputs(0);

            self.multilayerPerceptronManager.SetInputs(0, 1, 0);
            self.multilayerPerceptronManager.SetOutputs(1);

            self.multilayerPerceptronManager.SetInputs(1, 0, 0);
            self.multilayerPerceptronManager.SetOutputs(1);

            self.multilayerPerceptronManager.SetInputs(1, 1, 0);
            self.multilayerPerceptronManager.SetOutputs(0);
        }

    }
}
