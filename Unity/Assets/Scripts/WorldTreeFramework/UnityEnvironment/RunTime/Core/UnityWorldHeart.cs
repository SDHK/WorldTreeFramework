﻿/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 世界之心：Unity线程
	/// </summary>
	public class UnityWorldHeart : WorldHeartBase
		, ComponentOf<WorldTreeRoot>
		, AsComponentBranch
		, AsAwake<int>
		, AsLateUpdateTime
		, AsFixedUpdateTime
		, AsGuiUpdateTime
		, AsDrawGizmosUpdateTime
	{
		/// <summary>
		/// 运行线程
		/// </summary>
		public UnityMonoBehaviourThread thread;

		#region 世界脉搏

		/// <summary>
		/// 世界更新
		/// </summary>
		public WorldPulse<UpdateTime> worldUpdate;
		/// <summary>
		/// 世界晚更新
		/// </summary>
		public WorldPulse<LateUpdateTime> worldLateUpdate;
		/// <summary>
		/// 世界固定更新
		/// </summary>
		public WorldPulse<FixedUpdateTime> worldFixedUpdate;
		/// <summary>
		/// 世界GUI更新
		/// </summary>
		public WorldPulse<GuiUpdateTime> worldGuiUpdate;
		/// <summary>
		/// 世界绘制Gizmos更新
		/// </summary>
		public WorldPulse<DrawGizmosUpdateTime> worldDrawGizmosUpdate;

		#endregion

		#region 全局事件法则

		/// <summary>
		/// 启用
		/// </summary>
		public IRuleActuator<Enable> enable;
		/// <summary>
		/// 禁用
		/// </summary>
		public IRuleActuator<Disable> disable;
		/// <summary>
		/// 更新
		/// </summary>
		public IRuleActuator<Update> update;
		/// <summary>
		/// 更新时间
		/// </summary>
		public IRuleActuator<UpdateTime> updateTime;
		/// <summary>
		/// 晚更新
		/// </summary>
		public IRuleActuator<LateUpdate> lateUpdate;
		/// <summary>
		/// 晚更新时间
		/// </summary>
		public IRuleActuator<LateUpdateTime> lateUpdateTime;
		/// <summary>
		/// 固定更新
		/// </summary>
		public IRuleActuator<FixedUpdate> fixedUpdate;
		/// <summary>
		/// 固定更新时间
		/// </summary>
		public IRuleActuator<FixedUpdateTime> fixedUpdateTime;
		/// <summary>
		/// GUI更新
		/// </summary>
		public IRuleActuator<GuiUpdate> onGUI;
		/// <summary>
		/// GUI更新时间
		/// </summary>
		public IRuleActuator<GuiUpdateTime> onGUIUpdateTime;
		/// <summary>
		/// 绘制Gizmos更新
		/// </summary>
		public IRuleActuator<DrawGizmosUpdate> drawGizmos;
		/// <summary>
		/// 绘制Gizmos更新时间
		/// </summary>
		public IRuleActuator<DrawGizmosUpdateTime> drawGizmosUpdateTime;

		#endregion

		public override void Run()
		{
			isRun = true;
			thread.IsRun = true;
		}

		public override void Pause()
		{
			isRun = false;
			thread.IsRun = false;
		}

		public override void OneFrame()
		{
			thread.IsRun = false;
			thread.IsOneFrame = true;
		}
	}

	public static class WorldHeartUnityRule
	{
		private class AwakeRule : AwakeRule<UnityWorldHeart, int>
		{
			protected override void Execute(UnityWorldHeart self, int frameTime)
			{
				self.frameTime = frameTime;

				self.Core.GetGlobalRuleActuator(out self.enable);
				self.Core.GetGlobalRuleActuator(out self.update);
				self.Core.GetGlobalRuleActuator(out self.updateTime);
				self.Core.GetGlobalRuleActuator(out self.disable);
				self.Core.GetGlobalRuleActuator(out self.lateUpdate);
				self.Core.GetGlobalRuleActuator(out self.lateUpdateTime);
				self.Core.GetGlobalRuleActuator(out self.fixedUpdate);
				self.Core.GetGlobalRuleActuator(out self.fixedUpdateTime);
				self.Core.GetGlobalRuleActuator(out self.onGUI);
				self.Core.GetGlobalRuleActuator(out self.onGUIUpdateTime);
				self.Core.GetGlobalRuleActuator(out self.drawGizmos);
				self.Core.GetGlobalRuleActuator(out self.drawGizmosUpdateTime);

				self.AddComponent(out self.worldUpdate, frameTime).Run();
				self.AddComponent(out self.worldLateUpdate, frameTime).Run();
				self.AddComponent(out self.worldFixedUpdate, frameTime).Run();
				self.AddComponent(out self.worldGuiUpdate, frameTime).Run();
				self.AddComponent(out self.worldDrawGizmosUpdate, frameTime).Run();

				self.thread = new GameObject(self.GetType().Name).AddComponent<UnityMonoBehaviourThread>();
				GameObject.DontDestroyOnLoad(self.thread.gameObject);
				self.thread.OnUpdate = self.worldUpdate.Update;
				self.thread.OnLateUpdate = self.worldLateUpdate.Update;
				self.thread.OnFixedUpdate = self.worldFixedUpdate.Update;
				self.thread.OnGuiUpdate = self.worldGuiUpdate.Update;
				self.thread.OnDrawGizmosUpdate = self.worldDrawGizmosUpdate.Update;
			}
		}

		private class RemoveRule : RemoveRule<UnityWorldHeart>
		{
			protected override void Execute(UnityWorldHeart self)
			{
				if (self.thread != null) GameObject.Destroy(self.thread.gameObject);
				self.thread = null;

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