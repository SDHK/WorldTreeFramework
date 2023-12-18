/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件

*/
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;
using YooAsset;

namespace WorldTree
{

	//public class TestPoolNodeUpdate : UpdateRule<TestPoolNode>
	//{
	//    protected override void OnEvent(TestPoolNode self, float arg1)
	//    {
	//        World.Log($"TestPoolNode!!!!+{self.Parent.Id}");
	//    }
	//}


	//class TestPoolNodeAdd1 : NodeAddRule<NodePool, TestPoolNode> { }
	//public class TestPoolNode : Node
	//    , ComponentOf<NodePool>
	//    , AsRule<IAwakeRule>
	//    , AsRule<IUpdateRule>
	//{


	//}

	//public class NodeAddRule : AddRule<Node>
	//{
	//    protected override void OnEvent(Node self)
	//    {

	//        // World.Log($"NodeAdd: {self.Id} _  {self.Type} ");
	//    }
	//}

	//public class NodeListenerAddRule : ListenerAddRule<InitialDomain>
	//{
	//    protected override void OnEvent(InitialDomain self, INode node)
	//    {
	//        World.Log($"NodeListenerAdd: {node.Id} _  {node.Type} ");
	//    }
	//}

	//public class NodeListenerInitialDomainAddRule : ListenerAddRule<InitialDomain, TreeNode, IRule>
	//{
	//    protected override void OnEvent(InitialDomain self, TreeNode node)
	//    {
	//        World.Log($"NodeListener InitialDomain Add: {self.Id} _  {self.Type} ");
	//    }
	//}



	/// <summary>
	/// 初始域
	/// </summary>
	public class InitialDomain : DynamicNodeListener, ComponentOf<INode>
		, AsRule<IAwakeRule>
		, AsRule<IFixedUpdateTimeRule>
		, AsRule<ILateUpdateTimeRule>
	{
		public TestNode node;
		public TreeValue<float> valueFloat;
		public TreeValue<int> valueInt;

		public TreeValue<string> valueString;
		public TreeTween<string> treeTween;
		public MultilayerPerceptronManager multilayerPerceptronManager;


		public int int1;

	}

	public static class InitialDomainRule
	{

		class _AddRule : AddRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
			{

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

			protected override void OnEvent(InitialDomain self)
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
					self.LogError("版本号更新失败，可能是找不到服务器");
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
					self.LogError("Mainfest更新失败！");
				}
			}


			int downloadingMaxNum = 10;
			int failedTryAgain = 3;
			int timeout = 60;

			public async TreeTask Download(INode self)
			{
				self.Log("下载!!!");

				var package = YooAssets.GetPackage("DefaultPackage");
				var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);
				//下载数量是0，直接就完成了
				if (downloader.TotalDownloadCount == 0)
				{
					self.Log("下载数量是0，直接就完成了");

					await self.TreeTaskCompleted();
					return;
				}

				//注册一些回调
				//downloader.OnDownloadErrorCallback += OnError;
				//downloader.OnDownloadProgressCallback += OnProcess;
				//downloader.OnDownloadOverCallback += OnOver;
				//downloader.OnStartDownloadFileCallback += OnStartDownOne;

				//开始下载
				self.Log("开始下载???");

				downloader.BeginDownload();
				//等待下载完成
				await self.GetAwaiter(downloader);
				self.Log("等待下载完成???");

				await self.TreeTaskCompleted();

				//检查状态
				if (downloader.Status == EOperationStatus.Succeed)
				{
					self.Log("下载完成");

					AssetOperationHandle handle = await self.GetAwaiter(package.LoadAssetAsync<GameObject>("MainWindow"));

					GameObject go = handle.InstantiateSync();
				}
				else
				{
					self.Log("下载失败");
				}
			}



		}


		class AddRule : AddRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
			{
				//CRC32 a = new CRC32();

				self.Log($"初始域启动！！");
				TreeTaskBase.ExceptionHandler = self.LogError;

				self.AddComponent(out self.multilayerPerceptronManager);

				self.multilayerPerceptronManager.AddLayer(3);
				self.multilayerPerceptronManager.AddLayer(4);
				self.multilayerPerceptronManager.AddLayer(1);

				//self.ListenerSwitchesEntity<INode>();

				//self.AddChild(out self.valueFloat);
				//self.AddChild(out self.valueInt);
				//self.valueFloat.Bind(self.valueInt);


				//获取一个 字符串数值组件 写入初始值"Hello world! 你好世界！"
				//self.AddChild(out self.valueString, "Hello world! 你好世界！");
				////对 字符串数值组件 启动一个渐变动画，从 "Hello world! 你好世界！" 变成 "Hello" 动画时间为10秒
				//self.valueString.GetTween("Hello", 10f);


				////获取一个 Float数值组件 写入初始值 1f
				//self.AddChild(out self.valueFloat, 1f);

				////数值组件Float 单向绑定 数值组件String ,当float值发生变化时，会通知 valueString 并转为字符串
				//self.valueFloat.Bind(self.valueString);

				////对 Float数值组件 启动一个渐变动画，从 1f 变成 10f 动画时间为 3 秒
				//self.valueFloat.GetTween(10f, 3f);


				//self.AddComponent(out TreeTaskToken treeTaskToken);
				//self.AddComponent(out TreeNode _).Test().Coroutine(treeTaskToken);

				//self.AddComponent(out TreeNode _);

			}
		}


		class UpdateRule : UpdateRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
			{
                //if (Input.GetKeyDown(KeyCode.A))
                //{
                //	self.valueFloat.Value += 1.5f;
                //	World.Log($"A {self.valueFloat.Value} : {self.valueInt.Value}");
                //}
                //if (Input.GetKeyDown(KeyCode.S))
                //{
                //	//self.valueInt.Value += 1;
                //	World.Log($"S {self.valueFloat.Value} : {self.valueInt.Value} :{self.valueString.Value}");

                //}

                //World.Log("初始域启动!!!");

				//if (Input.GetKeyDown(KeyCode.Q))
				//{
				//	World.Log("初始域启动");
				//	//self.AddComponent(out TreeTaskToken treeTaskToken).Continue();
				//	//self.AddComponent(out TreeNode _).Test().Coroutine(treeTaskToken);

				//	//self.treeTween.Run().WaitForCompletion().Coroutine(treeTaskToken);
				//	self.AddComponent(out TestNode _).Test().Coroutine();
				//	//self.AddComponent(out TestNode _);

				//	//for (int i = 0; i < 1000; i++)
				//	//{
				//	//	self.AddComponent(out TreeNode _).Test1().Coroutine();
				//	//}
				//}

				//if (Input.GetKeyDown(KeyCode.W))
				//{
				//	self.AddComponent(out TreeTaskToken treeTaskToken).Stop();
				//}

				//if (Input.GetKeyDown(KeyCode.E))
				//{
				//	self.AddComponent(out TreeTaskToken treeTaskToken).Continue();
				//}

				//if (Input.GetKeyDown(KeyCode.R))
				//{
				//	self.AddComponent(out TreeTaskToken treeTaskToken).Cancel();
				//	//treeTaskToken.Dispose();
				//}
			}
		}
		class UpdateRule1 : UpdateRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
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

					self.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");

					//self.B.Value += 1;
					//World.Log($"B  A:{self.A.Value}  B:{self.B.Value}");
				}

				if (Input.GetKeyDown(KeyCode.X))
				{
					self.multilayerPerceptronManager.SetInputs(1, 0, 1);
					self.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
				}

				if (Input.GetKeyDown(KeyCode.C))
				{
					self.multilayerPerceptronManager.SetInputs(0, 1, 1);
					self.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
				}

				if (Input.GetKeyDown(KeyCode.V))
				{
					self.multilayerPerceptronManager.SetInputs(0, 0, 1);
					self.Log($"正向 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
				}
			}

			public void Test(InitialDomain self)
			{

				self.multilayerPerceptronManager.SetInputs(0, 0, 0);
				self.multilayerPerceptronManager.SetOutputs(0);

				self.multilayerPerceptronManager.SetInputs(0, 1, 0);
				self.multilayerPerceptronManager.SetOutputs(1);

				self.multilayerPerceptronManager.SetInputs(1, 0, 0);
				self.multilayerPerceptronManager.SetOutputs(1);

				self.multilayerPerceptronManager.SetInputs(1, 1, 0);
				self.multilayerPerceptronManager.SetOutputs(0);


				self.multilayerPerceptronManager.SetInputs(0, 0, 1);
				self.multilayerPerceptronManager.SetOutputs(0);

				self.multilayerPerceptronManager.SetInputs(0, 1, 1);
				self.multilayerPerceptronManager.SetOutputs(1);


				self.multilayerPerceptronManager.SetInputs(1, 0, 1);
				self.multilayerPerceptronManager.SetOutputs(1);


				self.multilayerPerceptronManager.SetInputs(1, 1, 1);
				self.multilayerPerceptronManager.SetOutputs(0);


			}

		}

	}
}
