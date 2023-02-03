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
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WorldTree
{

    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Entity
    {
        public int index = 0;
    }
    //内联函数，缩短函数调用时间
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    class _InitialDomain : AddSystem<InitialDomain>
    {
        public override async void OnAdd(InitialDomain self)
        {
            self.TrySendSystem<IAddSystem>();
            self.TrySendSystem<IRemoveSystem>();

            //Dictionary<string,int> dic = self.AddComponent<EntityDictionary<string,int>>().Value;



            //World.Log("初始域启动！");
            GameObject gameObject = await Addressables.InstantiateAsync("MainWindow").Task;

            //GameObject gameObject = await self.AddressablesInstantiateAsync("MainWindow");

            //self.Root.AddComponent<WindowManager>().Show<MainWindow>().Coroutine();

        }


    }

    class InitialDomainUpdateSystem : UpdateSystem<InitialDomain>
    {
        public override void Update(InitialDomain self, float deltaTime)
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                self.AddComponent<Node>()
                    .Test().Coroutine()
                    ;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                self.RemoveComponent<Node>();
            }

        }
    }
}
