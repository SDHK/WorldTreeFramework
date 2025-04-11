/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
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
		/// 启动
		/// </summary>
		public void Start()
		{
			WorldLineManager = new();
			WorldLineManager.LogType = typeof(UnityWorldLog);
			WorldLineManager.LogLevel = LogLevel.All;

			if (Define.IsEditor) WorldLineManager.SetView(typeof(UnityWorldHeart), typeof(UnityViewBuilderWorld), typeof(UnityWorldTreeNodeViewBuilder));

			WorldLineManager.Create(0, typeof(UnityWorldHeart), 0, typeof(MainWorld));
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