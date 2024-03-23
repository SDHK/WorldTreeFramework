/****************************************

* 作者：闪电黑客
* 日期：2024/2/21 11:39

* 描述：游戏启动入口

*/

using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace WorldTree.AOT
{
	/// <summary>
	/// 游戏启动入口
	/// </summary>
	public class GameEntry : MonoBehaviour
	{
		/// <summary>
		/// 游戏启动入口单例
		/// </summary>
		public static GameEntry instance;

		/// <summary>
		/// 进度条
		/// </summary>
		public Slider slider;

		public Text text;

		//http://192.168.31.95:9999
		//http://127.0.0.1:9999
		public string hostServerIP = "http://127.0.0.1:9999";

		/// <summary>
		/// 游戏运行模式
		/// </summary>
		public GamePlayMode playMode = GamePlayMode.OfflinePlayMode;

		/// <summary>
		/// 资源包版本号
		/// </summary>
		[ReadOnly]
		public string packageVersion;

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			Debug.Log($"启动模式:{playMode}");

			//AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(a => Debug.Log($"本地程序集：{a.GetName().Name}"));

			StartLoad();
		}

		public void StartLoad()
		{
			YooAssetsHelper.InitializePackage();

			//StartWorldTree();
		}

		/// <summary>
		/// 启动框架
		/// </summary>
		public async void StartWorldTree()
		{
			await Task.CompletedTask;
#if !UNITY_EDITOR
			await HybridCLRHelper.LoadAOT();
			await HybridCLRHelper.LoadHotUpdate();
#endif
			Debug.Log("启动框架!");

			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "WorldTree.CoreUnity");
			gameObject.AddComponent(assembly.GetType("WorldTree.UnityWorldTree"));
		}
	}
}