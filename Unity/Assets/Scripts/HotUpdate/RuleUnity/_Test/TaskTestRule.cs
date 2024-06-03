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

					self.AddChild(out self.treeTaskToken).Continue();
					self.Test().Coroutine(self.treeTaskToken);
				}
				if (Input.GetKeyDown(KeyCode.W))
				{
					self.treeTaskToken.Stop();
				}
				if (Input.GetKeyDown(KeyCode.E))
				{
					self.treeTaskToken.Continue();
				}
				if (Input.GetKeyDown(KeyCode.R))
				{
					self.treeTaskToken.Cancel();
				}
				if (Input.GetKeyDown(KeyCode.T))
				{
					self.treeTaskToken.Dispose();
				}

			}
		}

		public static async TreeTask Test(this TaskTest self)
		{
			//await self.AsyncDelay(1);
			TreeTaskToken TaskTokenCatch = await self.TreeTaskTokenCatch();
			self.Log($"A！令牌捕获:{(TaskTokenCatch == null ? null : TaskTokenCatch.Id)}");
			while (TaskTokenCatch.taskState != TaskState.Cancel)
			{
				await self.AsyncDelay(6);

				self.AddComponent(out TreeTaskToken treeTaskToken).Continue();
				await (self.TestB().SetToken(treeTaskToken) as TreeTask);

				await self.TaskD();

			}

		}

		public static async TreeTask TestB(this TaskTest self)
		{
			await self.TaskC();
			await self.AsyncDelay(2);

			TreeTaskToken TaskTokenCatch = await self.TreeTaskTokenCatch();
			self.Log($"B！令牌捕获:{(TaskTokenCatch == null ? null : TaskTokenCatch.Id)}");
		}




		public static async TreeTask TaskC(this TaskTest self)
		{
			TreeTaskToken TaskTokenCatch = await self.TreeTaskTokenCatch();
			self.Log($"C！令牌捕获:{(TaskTokenCatch == null ? null : TaskTokenCatch.Id)}");
		}
		public static async TreeTask TaskD(this TaskTest self)
		{
			TreeTaskToken TaskTokenCatch = await self.TreeTaskTokenCatch();
			self.Log($"D！令牌捕获:{(TaskTokenCatch == null ? null : TaskTokenCatch.Id)}");
		}
	}
}