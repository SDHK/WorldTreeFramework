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

		public static async TreeTask T2(this TaskTest self)
		{
			//await self.AsyncDelay(5);

			self.Log("T2 1！");

			await self.T3();

			//await self.TreeTaskCompleted();
		}

		public static async TreeTask T3(this TaskTest self)
		{
			self.Log("T3 1！");

			//await self.TreeTaskCompleted();
			var tk = await self.TreeTaskTokenCatch();
			self.Log($"TK!!!!!!!{tk.Id}");

			//World.Log(await T4());

			//await T5();
		}

		public static async TreeTask<int> T4(this TaskTest self)
		{
			self.Log("T4 1！");

			await self.TreeTaskCompleted();

			return 10021;
		}

		public static async TreeTask T5(this TaskTest self)
		{
			self.Log("T5 1！");
			await self.T6();
			await self.TreeTaskCompleted();
		}

		public static async TreeTask T6(this TaskTest self)
		{
			self.Log("T6 1！");
			await self.T7();

			await self.TreeTaskCompleted();
		}

		public static async TreeTask T7(this TaskTest self)
		{
			self.Log("T7 1！");
			var tk = await self.TreeTaskTokenCatch();
			self.Log($"TK!!!!!!!{tk?.Id}");

			await self.TreeTaskCompleted();
		}

		public static async TreeTask TaskRun(this TaskTest self, Action action)
		{
			await self.Core.worldContext.AddChild(out TreeTaskSwitchWorld _, self.Core.worldContext);
			action?.Invoke();
		}

	}
}