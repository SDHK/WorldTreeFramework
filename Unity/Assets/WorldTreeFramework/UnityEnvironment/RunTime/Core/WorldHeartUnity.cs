/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:56:05

* 描述： 世界之心：Unity线程

*/

using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 世界之心：Unity线程
	/// </summary>
	public class WorldHeartUnity : WorldHeartBase
		, ComponentOf<WorldTreeRoot>
		, AsRule<IAwakeRule<int>>
		, AsRule<ILateUpdateTimeRule>
		, AsRule<IFixedUpdateTimeRule>
		, AsRule<IGuiUpdateTimeRule>
		, AsRule<IDrawGizmosUpdateTimeRule>
	{
		/// <summary>
		/// 运行线程
		/// </summary>
		public UnityMonoBehaviourThread m_Thread;

		#region 世界脉搏

		public WorldPulse<IUpdateTimeRule> worldUpdate;
		public WorldPulse<ILateUpdateTimeRule> worldLateUpdate;
		public WorldPulse<IFixedUpdateTimeRule> worldFixedUpdate;
		public WorldPulse<IGuiUpdateTimeRule> worldGuiUpdate;
		public WorldPulse<IDrawGizmosUpdateTimeRule> worldDrawGizmosUpdate;

		#endregion


		#region 全局事件法则

		public GlobalRuleActuator<IEnableRule> enable;
		public GlobalRuleActuator<IDisableRule> disable;
		public GlobalRuleActuator<IUpdateRule> update;
		public GlobalRuleActuator<IUpdateTimeRule> updateTime;
		public GlobalRuleActuator<ILateUpdateRule> lateUpdate;
		public GlobalRuleActuator<ILateUpdateTimeRule> lateUpdateTime;
		public GlobalRuleActuator<IFixedUpdateRule> fixedUpdate;
		public GlobalRuleActuator<IFixedUpdateTimeRule> fixedUpdateTime;
		public GlobalRuleActuator<IGuiUpdateRule> onGUI;
		public GlobalRuleActuator<IGuiUpdateTimeRule> onGUIUpdateTime;
		public GlobalRuleActuator<IDrawGizmosUpdateRule> drawGizmos;
		public GlobalRuleActuator<IDrawGizmosUpdateTimeRule> drawGizmosUpdateTime;

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
		class AwakeRule : AwakeRule<WorldHeartUnity, int>
		{
			protected override void OnEvent(WorldHeartUnity self, int frameTime)
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

		class RemoveRule : RemoveRule<WorldHeartUnity>
		{
			protected override void OnEvent(WorldHeartUnity self)
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



		class UpdateTimeRule : UpdateTimeRule<WorldHeartUnity>
		{
			protected override void OnEvent(WorldHeartUnity self, TimeSpan deltaTime)
			{
				self.enable?.Send();
				self.update?.Send();
				self.updateTime?.Send(deltaTime);
				self.disable?.Send();
			}
		}

		class LateUpdateTimeRule : LateUpdateTimeRule<WorldHeartUnity>
		{
			protected override void OnEvent(WorldHeartUnity self, TimeSpan deltaTime)
			{
				self.lateUpdate?.Send();
				self.lateUpdateTime?.Send(deltaTime);
			}
		}

		class FixedUpdateTimeRule : FixedUpdateTimeRule<WorldHeartUnity>
		{
			protected override void OnEvent(WorldHeartUnity self, TimeSpan deltaTime)
			{
				self.fixedUpdate?.Send();
				self.fixedUpdateTime?.Send(deltaTime);
			}
		}

		class GuiUpdateTimeRule : GuiUpdateTimeRule<WorldHeartUnity>
		{
			protected override void OnEvent(WorldHeartUnity self, TimeSpan deltaTime)
			{
				self.onGUI?.Send();
				self.onGUIUpdateTime?.Send(deltaTime);
			}
		}

		class DrawGizmosUpdateTimeRule : DrawGizmosUpdateTimeRule<WorldHeartUnity>
		{
			protected override void OnEvent(WorldHeartUnity self, TimeSpan deltaTime)
			{
				self.drawGizmos?.Send();
				self.drawGizmosUpdateTime?.Send(deltaTime);
			}
		}
	}
}
