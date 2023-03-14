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
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using WorldTree.Internal;

namespace WorldTree
{

    ///// <summary>
    ///// 组件约束
    ///// </summary>
    //public interface ComponentTo<T> { }
    //public interface ComponentTo { }


    //public class TastNode : Node, ComponentTo<InitialDomain>
    //{


    //}


    //多态测试
    public class NodeAddRule : AddRule<Node>
    {
        public override void OnEvent(Node self)
        {

            World.Log($"NodeAdd: {self.Id} _  {self.Type} ");
        }
    }

    //public class NodeListenerAddRule : ListenerAddRule<Node>
    //{
    //    public override void OnEvent(Node self, INode node)
    //    {
    //        World.Log($"NodeListenerAdd: {self.Id} _  {self.Type} ");

    //    }
    //}

    //public class NodeListenerInitialDomainAddRule : ListenerAddRule<Node, InitialDomain,IRule>
    //{

    //    public override void OnEvent(Node self, InitialDomain node)
    //    {
    //        World.Log($"NodeListenerInitialDomainAdd: {self.Id} _  {self.Type} ");

    //    }
    //}
    //

    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Node
    {

        public float f = 0;

        public ValueBinder<float> valueBinder;

    }

    class _InitialDomain : AddRule<InitialDomain>
    {

        public override async void OnEvent(InitialDomain self)
        {
            self.Branch = self;

            World.Log("初始域启动！！");

            Type type = self.Type;
            var baseTypes = new List<Type>();
            while (type.BaseType != null)
            {
                baseTypes.Add(type.BaseType);
                type = type.BaseType;
            }

            foreach (var baseType in baseTypes)
            {
                Debug.Log(baseType.Name);
            }


            using (await self.AsyncLock(0))
            {
                await self.AsyncDelay(3);
                self.f++;
            }
            World.Log($"初始域!:{self.f}");

        }

    }
    class InitialDomainUpdateRule : UpdateRule<InitialDomain>
    {
        public override async void OnEvent(InitialDomain self, float deltaTime)
        {
            World.Log($"初始域:!!!!!");

            if (Input.GetKeyDown(KeyCode.A))
            {
                using (await self.AsyncLock(self.Id))
                {
                    await self.AsyncDelay(3);
                    self.f++;
                    World.Log($"初始域:{self.f}");
                }

            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                self.AddComponent<TreeNode>().Test().Coroutine();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                self.AddComponent<TreeNode>().SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                self.AddComponent<TreeNode>().SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                self.RemoveComponent<TreeNode>();
            }
        }

    }
}
