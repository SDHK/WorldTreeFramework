/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace WorldTree
{
	/// <summary>
	/// Unity的MonoBehaviour线程
	/// </summary>
	public class UnityMonoBehaviourThread : MonoBehaviour
	{
		/// <summary>
		/// 计时器
		/// </summary>
		public System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

		/// <summary>
		/// 更新
		/// </summary>
		public Action<TimeSpan> OnUpdate;
		/// <summary>
		/// 晚更新
		/// </summary>
		public Action<TimeSpan> OnLateUpdate;
		/// <summary>
		/// 固定更新
		/// </summary>
		public Action<TimeSpan> OnFixedUpdate;
		/// <summary>
		/// GUI
		/// </summary>
		public Action<TimeSpan> OnGuiUpdate;
		/// <summary>
		/// 绘制Gizmos
		/// </summary>
		public Action<TimeSpan> OnDrawGizmosUpdate;

		/// <summary>
		/// GUI时间
		/// </summary>
		public DateTime OnGUITime;
		/// <summary>
		/// 绘制Gizmos时间
		/// </summary>
		public DateTime OnDrawGizmosTime;

		/// <summary>
		/// 是否运行
		/// </summary>
		public bool IsRun = false;
		/// <summary>
		/// 是否一帧
		/// </summary>
		public bool IsOneFrame = false;

		/// <summary>
		/// 启动
		/// </summary>
		private void Awake()
		{
			OnGUITime = DateTime.Now;
			OnDrawGizmosTime = DateTime.Now;
		}

		/// <summary>
		/// 更新
		/// </summary>
		private void Update()
		{
			Profiler.BeginSample("SDHK Update");
			//sw.Restart();
			if (IsRun || IsOneFrame)
			{

				OnUpdate?.Invoke(TimeSpan.FromSeconds(Time.deltaTime));
			}
			//sw.Stop();
			//this.Log($"毫秒: {sw.ElapsedMilliseconds}");
			Profiler.EndSample();
		}
		/// <summary>
		/// 晚更新
		/// </summary>
		private void LateUpdate()
		{
			Profiler.BeginSample("SDHK LateUpdate");
			if (IsRun || IsOneFrame)
			{
				OnLateUpdate?.Invoke(TimeSpan.FromSeconds(Time.deltaTime));
				IsOneFrame = false;
			}
			Profiler.EndSample();
		}
		/// <summary>
		/// 固定更新
		/// </summary>
		private void FixedUpdate()
		{
			Profiler.BeginSample("SDHK FixedUpdate");
			OnFixedUpdate?.Invoke(TimeSpan.FromSeconds(Time.fixedDeltaTime));
			Profiler.EndSample();
		}
		/// <summary>
		/// GUI
		/// </summary>
		private void OnGUI()
		{
			Profiler.BeginSample("SDHK OnGUI");
			OnGuiUpdate?.Invoke(DateTime.Now - OnGUITime);
			OnGUITime = DateTime.Now;
			Profiler.EndSample();
		}
		/// <summary>
		/// 绘制Gizmos
		/// </summary>
		private void OnDrawGizmos()
		{
			Profiler.BeginSample("SDHK OnDrawGizmos");
			OnDrawGizmosUpdate?.Invoke(DateTime.Now - OnDrawGizmosTime);
			OnDrawGizmosTime = DateTime.Now;
			Profiler.EndSample();
		}
		/// <summary>
		/// 销毁
		/// </summary>
		private void OnDestroy()
		{
			OnUpdate = null;
			OnLateUpdate = null;
			OnFixedUpdate = null;
			OnGuiUpdate = null;
			OnDrawGizmosUpdate = null;
		}
	}
}
