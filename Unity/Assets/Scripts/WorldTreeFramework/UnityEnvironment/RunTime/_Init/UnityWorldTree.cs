/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 世界树框架启动器，一切从这里开始
	/// </summary>
	public class UnityWorldTree : MonoBehaviour
	{
		/// <summary>
		/// 世界线管理器
		/// </summary>
		public WorldLineManager WorldLineManager;

		/// <summary>
		/// 启动选项
		/// </summary>
		public Options Options = new();

		/// <summary>
		/// 启动
		/// </summary>
		public void Start()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => Debug.LogError(e.ExceptionObject.ToString());
			// 命令行参数接收
			//Parser.Default.ParseArguments<Options>(System.Environment.GetCommandLineArgs())
			//		   .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
			//		   .WithParsed((o) => Options = o);

			WorldLineManager = new();
			WorldLineManager.Options = Options;
			WorldLineManager.LogType = typeof(UnityWorldLog);


			if (Define.IsEditor) WorldLineManager.SetView(typeof(UnityViewBuilderWorld), typeof(UnityWorldHeart), typeof(UnityWorldTreeNodeViewBuilder));

			var line = WorldLineManager.Create(0, typeof(UnityWorldHeart));
			line.WorldContext.Post(() =>
			{
				line.AddComponent(out MainWorld _);
			});
		}

		/// <summary>
		/// 退出
		/// </summary>
		private void OnApplicationQuit()
		{
			WorldLineManager?.Dispose();
			WorldLineManager = null;
		}

		/// <summary>
		/// 销毁
		/// </summary>
		private void OnDestroy()
		{
			WorldLineManager?.Dispose();
			WorldLineManager = null;
		}
	}
}