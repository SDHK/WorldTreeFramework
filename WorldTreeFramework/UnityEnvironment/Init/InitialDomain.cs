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
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using WorldTree.Internal;

namespace WorldTree
{
    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Node
    {

        public float f = 0;

        public ValueBinder<float> valueBinder;

    }

    class _InitialDomainSendSystemAsyncBase : CallSystemAsync<InitialDomain, float>
    {
        public override async TreeTask<float> OnEvent(InitialDomain self)
        {
            World.Log("延迟！1");
            await self.AsyncDelay(3);
            World.Log("延迟！2");


            self.SendSystem<IAddSystem>();

            return 10f;
        }
    }


    //内联函数，缩短函数调用时间
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]

    class _InitialDomain : AddSystem<InitialDomain>
    {
        public override async void OnEvent(InitialDomain self)
        {
            World.Log("初始域启动！！!!");
            var a = await self.CallAsync<float>();
            World.Log(a);

            World.Log("初始域启动！！!!??");


            //self.valueBinder = self.AddChildren<ValueBinder<float>>();

            //self.valueBinder.bindObject = self;
            //self.valueBinder.SetValue = (e, t) => (e as InitialDomain).f = t;
            //self.valueBinder.GetValue = e => (e as InitialDomain).f;

            //self.Root.AddComponent<WindowManager>().Show<MainWindow>().Coroutine();



            //GetGroundPoint(Vector3(1,1), )


            self.SendSystem<IAddSystem>();



        }

    }
    class InitialDomainUpdateSystem : UpdateSystem<InitialDomain>
    {
        public override void OnEvent(InitialDomain self, float deltaTime)
        {

            //self.valueBinder.Value = deltaTime;

            
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
