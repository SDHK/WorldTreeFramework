/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件

*/
using BM;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{

    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Entity { }
    class _InitialDomain : AddSystem<InitialDomain>
    {

        public override void OnAdd(InitialDomain self)
        {
            World.Log("初始域启动！");
            //await CheckHotfix();

            //从ab包拿到ScriptObject
            //var scriptObj = (await AssetComponent.LoadAsync<Object>(BPath.Assets_AssetBundles_ScriptObjects_TestAssets__asset));


            ////通过预制体获取物体实例
            //GameObject gameObject = self.PoolGet(scriptObj.TestObject);
            ////通过预制体回收实例
            //self.PoolRecycle(scriptObj.TestObject, gameObject);


            ////通过预制体获取物体组件
            //gameObject = self.AddComponent<GameObjectComponent, GameObject>(scriptObj.TestObject).gameObject;
            ////回收组件
            //self.RemoveComponent<GameObjectComponent>();

        }


        private async ETTask CheckHotfix()
        {
#if UNITY_EDITOR
            //重新配置热更路径(开发方便用, 打包移动端需要注释注释)
            //AssetComponentConfig.HotfixPath = Application.dataPath + "/../HotfixBundles/";
#endif
            //默认的分包名字就是Main分包
            AssetComponentConfig.DefaultBundlePackageName = "Main";
            Dictionary<string, bool> updatePackageBundle = new Dictionary<string, bool>()
            {
                {AssetComponentConfig.DefaultBundlePackageName, false},
            };
            UpdateBundleDataInfo updateBundleDataInfo = await AssetComponent.CheckAllBundlePackageUpdate(updatePackageBundle);
            if (updateBundleDataInfo.NeedUpdate)
            {
                World.LogError("需要更新, 大小: " + updateBundleDataInfo.NeedUpdateSize);
                await AssetComponent.DownLoadUpdate(updateBundleDataInfo);
            }
            //TODO  使用密钥要在这里添加密钥
            await AssetComponent.Initialize(AssetComponentConfig.DefaultBundlePackageName, "SDHK");
            World.Log("初始化完成");
        }
    }

    class InitialDomainUpdateSystem : UpdateSystem<InitialDomain>
    {
        public override void Update(InitialDomain self, float deltaTime)
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                self.AddComponent<Node>().Test().Coroutine();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                self.RemoveComponent<Node>();
            }

            AssetComponent.Update();
        }
    }

}
