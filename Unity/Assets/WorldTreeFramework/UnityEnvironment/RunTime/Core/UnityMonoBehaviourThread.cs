/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/10 02:44:15

* 描述：Unity的MonoBehaviour线程
* 用于驱动框架运行

*/

using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.TextCore.Text;

namespace WorldTree
{
	/// <summary>
	/// Unity的MonoBehaviour线程
	/// </summary>
	public class UnityMonoBehaviourThread : MonoBehaviour
	{
		public System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();


		public Action<TimeSpan> onUpdate;
		public Action<TimeSpan> onLateUpdate;
		public Action<TimeSpan> onFixedUpdate;
		public Action<TimeSpan> onGUI;
		public Action<TimeSpan> onDrawGizmos;

		public DateTime onGUITime;
		public DateTime onDrawGizmosTime;

		public bool isRun = false;

		public bool isOneFrame = false;


		private void Awake()
		{
			onGUITime = DateTime.Now;
			onDrawGizmosTime = DateTime.Now;
		}

		private void Update()
		{
			Profiler.BeginSample("SDHK Update");
			//sw.Restart();
			if (isRun || isOneFrame)
			{

				onUpdate?.Invoke(TimeSpan.FromSeconds(Time.deltaTime));
			}
			//sw.Stop();
			//this.Log($"毫秒: {sw.ElapsedMilliseconds}");
			Profiler.EndSample();
		}
		private void LateUpdate()
		{
			Profiler.BeginSample("SDHK LateUpdate");
			if (isRun || isOneFrame)
			{
				onLateUpdate?.Invoke(TimeSpan.FromSeconds(Time.deltaTime));
				isOneFrame = false;
			}
			Profiler.EndSample();
		}

		private void FixedUpdate()
		{
			Profiler.BeginSample("SDHK FixedUpdate");
			onFixedUpdate?.Invoke(TimeSpan.FromSeconds(Time.fixedDeltaTime));
			Profiler.EndSample();
		}

		private void OnGUI()
		{

			Profiler.BeginSample("SDHK OnGUI");
			onGUI?.Invoke(DateTime.Now - onGUITime);
			onGUITime = DateTime.Now;
			Profiler.EndSample();
		}

		private void OnDrawGizmos()
		{
			Profiler.BeginSample("SDHK OnDrawGizmos");
			onDrawGizmos?.Invoke(DateTime.Now - onDrawGizmosTime);
			onDrawGizmosTime = DateTime.Now;
			Profiler.EndSample();
		}

		private void OnDestroy()
		{
			onUpdate = null;
			onLateUpdate = null;
			onFixedUpdate = null;
			onGUI = null;
			onDrawGizmos = null;
		}
	}
}
