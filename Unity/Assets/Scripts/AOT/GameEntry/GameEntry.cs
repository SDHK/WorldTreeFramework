/****************************************

* 作者：闪电黑客
* 日期：2024/2/21 11:39

* 描述：游戏启动入口

*/

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

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
		/// 游戏运行模式
		/// </summary>
		public GamePlayMode playMode = GamePlayMode.OfflinePlayMode;

		/// <summary>
		/// 资源包版本号
		/// </summary>
		public string packageVersion;

		/// <summary>
		/// 资源服务器地址
		/// </summary>
		private string netPath = "http://127.0.0.1:9999/FTP/2023-04-20-1108";

		private void Awake()
		{
			instance = this;
		}

		private async void Start()
		{
			AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(a => Debug.Log($"本地程序集：{a.GetName().Name}"));

			StartLoad();
		}

		public void StartLoad()
		{
			//await YooAssetsHelper.Initialize(playMode);
			//await HybridCLRHelper.LoadAOT();
			//await HybridCLRHelper.LoadHotUpdate();
			//StartWorldTree();
		}

		/// <summary>
		/// 启动框架
		/// </summary>
		private void StartWorldTree()
		{
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "WorldTree.CoreUnity");
			gameObject.AddComponent(assembly.GetType("WorldTree.UnityWorldTree"));
		}
	}
}