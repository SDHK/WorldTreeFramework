
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述：世界树框架驱动器，一切从这里开始

*/

using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace WorldTree
{

	public class UnityWorldTree : MonoBehaviour
	{
		public System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

		public WorldTreeCore Core;

		GlobalRuleActuator<ILateUpdateRule> lateUpdate;
		GlobalRuleActuator<ILateUpdateTimeRule> lateUpdateTime;
		GlobalRuleActuator<IFixedUpdateRule> fixedUpdate;
		GlobalRuleActuator<IFixedUpdateTimeRule> fixedUpdateTime;
		GlobalRuleActuator<IGuiUpdateRule> onGUI;


		private void Start()
		{
			Core = new WorldTreeCore();

			Core.Log = Debug.Log;
			Core.LogWarning = Debug.LogWarning;
			Core.LogError = Debug.LogError;
			Core.Awake();

			Core.GetOrNewGlobalRuleActuator(out lateUpdate);
			Core.GetOrNewGlobalRuleActuator(out lateUpdateTime);
			Core.GetOrNewGlobalRuleActuator(out fixedUpdate);
			Core.GetOrNewGlobalRuleActuator(out fixedUpdateTime);
			Core.GetOrNewGlobalRuleActuator(out onGUI);

			Core.Root.AddComponent(out InitialDomain _);
		}

		private void Update()
		{
			Profiler.BeginSample("SDHK");

			//sw.Restart();
			Core.Update(Time.deltaTime);
			//sw.Stop();
			//World.Log($"毫秒: {sw.ElapsedMilliseconds}");

			Profiler.EndSample();

			if (Input.GetKeyDown(KeyCode.Return)) Debug.Log(Core.ToStringDrawTree());
		}

		private void LateUpdate()
		{
			lateUpdate?.Send();
			lateUpdateTime?.Send(Time.deltaTime);
		}
		private void FixedUpdate()
		{
			fixedUpdate?.Send();
			fixedUpdateTime?.Send(Time.fixedDeltaTime);
		}

		private void OnGUI()
		{
			onGUI.Send(0.02f);
		}

		private void OnDestroy()
		{
			Core.Dispose();
			Core = null;
		
			lateUpdate = null;
			lateUpdateTime = null;
			fixedUpdate = null;
			fixedUpdateTime = null;
			//onGUI = null;
		}

	}
}
