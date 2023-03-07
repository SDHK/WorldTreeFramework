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
    //内联函数，缩短函数调用时间
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]

    class _InitialDomain : AddRule<InitialDomain>
    {

        public override async void OnEvent(InitialDomain self)
        {
            World.Log("初始域启动！！");

            using (await self.AsyncQueueLock(0))
            {
                await self.AsyncDelay(1);
                self.f++;
                World.Log($"初始域!:{self.f}");
            }

        }

    }
    class InitialDomainUpdateSystem : UpdateRule<InitialDomain>
    {
        public override async void OnEvent(InitialDomain self, float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                using (await self.AsyncQueueLock(0))
                {
                    await self.AsyncDelay(1);
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
