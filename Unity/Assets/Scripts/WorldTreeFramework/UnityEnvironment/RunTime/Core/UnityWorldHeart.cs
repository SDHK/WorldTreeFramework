/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:56:05

* 描述： 世界之心：Unity线程

*/

using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace WorldTree
{
	/// <summary>
	/// 世界之心：Unity线程
	/// </summary>
	public class UnityWorldHeart : WorldHeartBase
		, ComponentOf<WorldTreeRoot>
		, AsAwake<int>
		, AsRule<LateUpdateTime>
		, AsRule<FixedUpdateTime>
		, AsRule<GuiUpdateTime>
		, AsRule<DrawGizmosUpdateTime>
	{
		/// <summary>
		/// 运行线程
		/// </summary>
		public UnityMonoBehaviourThread m_Thread;

		#region 世界脉搏

		public WorldPulse<UpdateTime> worldUpdate;
		public WorldPulse<LateUpdateTime> worldLateUpdate;
		public WorldPulse<FixedUpdateTime> worldFixedUpdate;
		public WorldPulse<GuiUpdateTime> worldGuiUpdate;
		public WorldPulse<DrawGizmosUpdateTime> worldDrawGizmosUpdate;

		#endregion

		#region 全局事件法则

		public GlobalRuleActuator<Enable> enable;
		public GlobalRuleActuator<Disable> disable;
		public GlobalRuleActuator<Update> update;
		public GlobalRuleActuator<UpdateTime> updateTime;
		public GlobalRuleActuator<LateUpdate> lateUpdate;
		public GlobalRuleActuator<LateUpdateTime> lateUpdateTime;
		public GlobalRuleActuator<FixedUpdate> fixedUpdate;
		public GlobalRuleActuator<FixedUpdateTime> fixedUpdateTime;
		public GlobalRuleActuator<GuiUpdate> onGUI;
		public GlobalRuleActuator<GuiUpdateTime> onGUIUpdateTime;
		public GlobalRuleActuator<DrawGizmosUpdate> drawGizmos;
		public GlobalRuleActuator<DrawGizmosUpdateTime> drawGizmosUpdateTime;

		#endregion

		public override void Run()
		{
			isRun = true;
			m_Thread.isRun = true;
		}

		public override void Pause()
		{
			isRun = false;
			m_Thread.isRun = false;
		}

		public override void OneFrame()
		{
			m_Thread.isRun = false;
			m_Thread.isOneFrame = true;
		}
	}

	public static class WorldHeartUnityRule
	{
		private class AwakeRule : AwakeRule<UnityWorldHeart, int>
		{
			protected override void Execute(UnityWorldHeart self, int frameTime)
			{
				self.frameTime = frameTime;

				self.GetOrNewGlobalRuleActuator(out self.enable);
				self.GetOrNewGlobalRuleActuator(out self.update);
				self.GetOrNewGlobalRuleActuator(out self.updateTime);
				self.GetOrNewGlobalRuleActuator(out self.disable);
				self.GetOrNewGlobalRuleActuator(out self.lateUpdate);
				self.GetOrNewGlobalRuleActuator(out self.lateUpdateTime);
				self.GetOrNewGlobalRuleActuator(out self.fixedUpdate);
				self.GetOrNewGlobalRuleActuator(out self.fixedUpdateTime);
				self.GetOrNewGlobalRuleActuator(out self.onGUI);
				self.GetOrNewGlobalRuleActuator(out self.onGUIUpdateTime);
				self.GetOrNewGlobalRuleActuator(out self.drawGizmos);
				self.GetOrNewGlobalRuleActuator(out self.drawGizmosUpdateTime);

				self.AddComponent(out self.worldUpdate, frameTime).Run();
				self.AddComponent(out self.worldLateUpdate, frameTime).Run();
				self.AddComponent(out self.worldFixedUpdate, frameTime).Run();
				self.AddComponent(out self.worldGuiUpdate, frameTime).Run();
				self.AddComponent(out self.worldDrawGizmosUpdate, frameTime).Run();

				self.m_Thread = new GameObject(self.GetType().Name).AddComponent<UnityMonoBehaviourThread>();
				GameObject.DontDestroyOnLoad(self.m_Thread.gameObject);
				self.m_Thread.onUpdate = self.worldUpdate.Update;
				self.m_Thread.onLateUpdate = self.worldLateUpdate.Update;
				self.m_Thread.onFixedUpdate = self.worldFixedUpdate.Update;
				self.m_Thread.onGUI = self.worldGuiUpdate.Update;
				self.m_Thread.onDrawGizmos = self.worldDrawGizmosUpdate.Update;
			}
		}

		private class RemoveRule : RemoveRule<UnityWorldHeart>
		{
			protected override void Execute(UnityWorldHeart self)
			{
				if (self.m_Thread != null) GameObject.Destroy(self.m_Thread.gameObject);
				self.m_Thread = null;

				self.worldUpdate = null;
				self.worldLateUpdate = null;
				self.worldFixedUpdate = null;
				self.worldGuiUpdate = null;
				self.worldDrawGizmosUpdate = null;

				self.enable = null;
				self.update = null;
				self.updateTime = null;
				self.disable = null;
				self.lateUpdate = null;
				self.lateUpdateTime = null;
				self.fixedUpdate = null;
				self.fixedUpdateTime = null;
				self.onGUI = null;
				self.onGUIUpdateTime = null;
				self.drawGizmos = null;
				self.drawGizmosUpdateTime = null;
			}
		}

		private class UpdateTimeRule : UpdateTimeRule<UnityWorldHeart>
		{
			protected override void Execute(UnityWorldHeart self, TimeSpan deltaTime)
			{
				self.enable?.Send();
				self.update?.Send();
				self.updateTime?.Send(deltaTime);
				self.disable?.Send();
			}
		}

		private class LateUpdateTimeRule : LateUpdateTimeRule<UnityWorldHeart>
		{
			protected override void Execute(UnityWorldHeart self, TimeSpan deltaTime)
			{
				self.lateUpdate?.Send();
				self.lateUpdateTime?.Send(deltaTime);
			}
		}

		private class FixedUpdateTimeRule : FixedUpdateTimeRule<UnityWorldHeart>
		{
			protected override void Execute(UnityWorldHeart self, TimeSpan deltaTime)
			{
				self.fixedUpdate?.Send();
				self.fixedUpdateTime?.Send(deltaTime);
			}
		}

		private class GuiUpdateTimeRule : GuiUpdateTimeRule<UnityWorldHeart>
		{
			protected override void Execute(UnityWorldHeart self, TimeSpan deltaTime)
			{
				self.onGUI?.Send();
				self.onGUIUpdateTime?.Send(deltaTime);
			}
		}

		private class DrawGizmosUpdateTimeRule : DrawGizmosUpdateTimeRule<UnityWorldHeart>
		{
			protected override void Execute(UnityWorldHeart self, TimeSpan deltaTime)
			{
				self.drawGizmos?.Send();
				self.drawGizmosUpdateTime?.Send(deltaTime);
			}
		}
	}
}