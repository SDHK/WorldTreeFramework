using System;
using UnityEngine;

namespace WorldTree
{
	public static class TaskTestRule
	{
		private class Update : UpdateRule<TaskTest>
		{
			protected override void Execute(TaskTest self)
			{
				if (Input.GetKeyDown(KeyCode.Q))
				{
					self.Log($"异步启动:！！！！！");

					self.AddComponent(out TreeTaskToken treeTaskToken).Continue();
					self.Test().Coroutine(treeTaskToken);
				}

				if (Input.GetKeyDown(KeyCode.W))
				{
					self.AddComponent(out TreeTaskToken _).Stop();
				}

				if (Input.GetKeyDown(KeyCode.E))
				{
					self.AddComponent(out TreeTaskToken _).Continue();
				}

				if (Input.GetKeyDown(KeyCode.R))
				{
					self.AddComponent(out TreeTaskToken treeTaskToken).Cancel();
					treeTaskToken.Dispose();
				}
			}
		}

		public static async TreeTask Test(this TaskTest self)
		{
			TreeTaskToken TaskTokenCatch = await self.TreeTaskTokenCatch();

			self.Log($"0！令牌捕获:{(TaskTokenCatch == null ? null : TaskTokenCatch.Id)}");

			await self.AsyncDelay(3);
			self.Log("1！");
			await self.TreeTaskCompleted();
			self.Log("2！");

			await self.TreeTaskCompleted();
			self.Log("计时");
			await self.AsyncDelay(3);
			self.Log("3！");
		}



		public static async TreeTask TaskRun(this TaskTest self, Action action)
		{
			await self.Core.worldContext.AddChild(out TreeTaskSwitchWorld _, self.Core.worldContext);
			action?.Invoke();
		}

	}
}