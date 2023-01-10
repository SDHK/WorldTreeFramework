/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件

*/
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{

    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Entity
    {
        public int index = 0;
    }
    class _InitialDomain : AddSystem<InitialDomain>
    {
        public override  void OnAdd(InitialDomain self)
        {
            //World.Log("初始域启动！");

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
