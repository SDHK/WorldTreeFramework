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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WorldTree.Internal;
using YooAsset;

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

        public TreeValue<string> valueString;
        public TreeTween<string> treeTween;

        public int int1;

    }

    public static class InitialDomainRule
    {

        class _AddRule : AddRule<InitialDomain>
        {
            public override async void OnEvent(InitialDomain self)
            {
                World.Log("资源加载！！！");

//                // 初始化资源系统
//                YooAssets.Initialize();

//                // 创建默认的资源包
//                var package = YooAssets.CreatePackage("DefaultPackage");
//                YooAssets.SetDefaultPackage(package);

//#if UNITY_EDITOR //编辑器模式
//                var initParameters = new EditorSimulateModeParameters();
//                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
//#else //非编辑器模式
//                //var initParameters = new OfflinePlayModeParameters();
//#endif
//                var res = await self.GetAwaiter(package.InitializeAsync(initParameters));


//                package = YooAssets.GetPackage("DefaultPackage");
//                AssetOperationHandle handle = await self.GetAwaiter(package.LoadAssetAsync<GameObject>("MainWindow"));

//                GameObject go = handle.InstantiateSync();
            }
        }


        class _AddRule1 : AddRule<InitialDomain>
        {

            public override  void OnEvent(InitialDomain self)
            {
                //World.Log("初始域启动2！！");

                //// 初始化资源系统
                //YooAssets.Initialize();
                //// 创建默认的资源包
                //ResourcePackage package = YooAssets.CreatePackage("DefaultPackage");
                //YooAssets.SetDefaultPackage(package);

                //await self.AsyncDelay(1);

                //await NetInitializeYooAsset(self, package);

                //await UpdatePack(self, package);

                //await UpdatePack1(self, package);


                //await Download(self);
                //World.Log("初始域启动2完成！！");

            }

            //初始化
            public async TreeTask NetInitializeYooAsset(INode self, ResourcePackage package)
            {
                string netPath = "http://192.168.31.157/FTP/2023-07-13-1169";

                var initParameters = new HostPlayModeParameters();

                initParameters.QueryServices = new QueryStreamingAssetsFileServices();

                //主资源服务器地址
                initParameters.DefaultHostServer = netPath;
                //备用资源服务器地址
                initParameters.FallbackHostServer = netPath;

                await self.GetAwaiter(package.InitializeAsync(initParameters));
            }

            // 内置文件查询服务类，这个类只需要返回ApplicationstreamingAsset下面的文件存在性就好
            private class QueryStreamingAssetsFileServices : IQueryServices
            {
                public bool QueryStreamingAssets(string fileName)
                {
                    //StreamingAssetsHelper.cs是太空战机里提供的一个查询脚本。
                    string buildinFolderName = YooAssets.GetStreamingAssetBuildinFolderName();
                    return File.Exists($"{Application.streamingAssetsPath}/{buildinFolderName}/{fileName}");
                }
            }
            string packageVersion = "2023-4-20-1108";

            public async TreeTask UpdatePack(INode self, ResourcePackage package)
            {
                //2.获取资源版本
                var operation = package.UpdatePackageVersionAsync();
                await self.GetAwaiter(operation);
                if (operation.Status != EOperationStatus.Succeed)
                {
                    World.LogError("版本号更新失败，可能是找不到服务器");
                    return;
                }
                //这是获取到的版本号，在下一个步骤要用
                packageVersion = operation.PackageVersion;
            }


            public async TreeTask UpdatePack1(INode self, ResourcePackage package)
            {
                //3.获取补丁清单
                var op = package.UpdatePackageManifestAsync(packageVersion);
                await self.GetAwaiter(op);
                if (op.Status != EOperationStatus.Succeed)
                {
                    World.LogError("Mainfest更新失败！");
                }
            }


            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            int timeout = 60;

            public async TreeTask Download(INode self)
            {
                World.Log("下载!!!");

                var package = YooAssets.GetPackage("DefaultPackage");
                var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);
                //下载数量是0，直接就完成了
                if (downloader.TotalDownloadCount == 0)
                {
                    World.Log("下载数量是0，直接就完成了");

                    await self.TreeTaskCompleted();
                    return;
                }

                //注册一些回调
                //downloader.OnDownloadErrorCallback += OnError;
                //downloader.OnDownloadProgressCallback += OnProcess;
                //downloader.OnDownloadOverCallback += OnOver;
                //downloader.OnStartDownloadFileCallback += OnStartDownOne;

                //开始下载
                World.Log("开始下载???");

                downloader.BeginDownload();
                //等待下载完成
                await self.GetAwaiter(downloader);
                World.Log("等待下载完成???");

                await self.TreeTaskCompleted();

                //检查状态
                if (downloader.Status == EOperationStatus.Succeed)
                {
                    World.Log("下载完成");

                    AssetOperationHandle handle = await self.GetAwaiter(package.LoadAssetAsync<GameObject>("MainWindow"));

                    GameObject go = handle.InstantiateSync();
                }
                else
                {
                    World.Log("下载失败");
                }
            }



        }


        class AddRule : AddRule<InitialDomain>
        {
            public override void OnEvent(InitialDomain self)
            {

                World.Log("初始域启动！！");
                //self.ListenerSwitchesEntity<INode>();

                //self.AddChild(out self.valueFloat);
                //self.AddChild(out self.valueInt);
                //self.valueFloat.Bind(self.valueInt);


                //self.AddChild(out self.valueString, "Hello world! 你好世界！");
                //self.treeTween = self.valueString.GetTween("Hello", 10f);

                //self.AddComponent(out TreeTaskToken treeTaskToken);
                //self.AddComponent(out TreeNode _).Test().Coroutine(treeTaskToken);

                //self.AddComponent(out TreeNode _);

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
                    //self.valueInt.Value += 1;
                    World.Log($"S {self.valueFloat.Value} : {self.valueInt.Value} :{self.valueString.Value}");

                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    self.AddComponent(out TreeTaskToken treeTaskToken).Continue();
                    //self.treeTween.Run().WaitForCompletion().Coroutine(treeTaskToken);
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
