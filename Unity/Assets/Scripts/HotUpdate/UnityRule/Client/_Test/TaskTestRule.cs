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

		/// <summary>
		/// 测试
		/// </summary>
		public static async TreeTask Test(this TaskTest self)
		{
			TreeTaskToken taskTokenCatch = await self.TreeTaskTokenCatch();



			self.Log($"A！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");

			while (taskTokenCatch.State != TokenState.Cancel)
			{
				await self.AsyncDelay(6);
				if (taskTokenCatch.State == TokenState.Cancel) return;

				await self.TestB();

				if (taskTokenCatch.State == TokenState.Cancel) return;
				await self.TaskD();
			}
		}

		/// <summary>
		/// 测试B
		/// </summary>
		public static async TreeTask TestB(this TaskTest self)
		{
			await self.TaskC();
			await self.AsyncDelay(4);

			TreeTaskToken taskTokenCatch = await self.TreeTaskTokenCatch();
			self.Log($"B！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");
		}

		/// <summary>
		/// 测试C
		/// </summary>
		public static async TreeTask TaskC(this TaskTest self)
		{
			TreeTaskToken taskTokenCatch = await self.TreeTaskTokenCatch();
			self.Log($"C！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");
		}

		/// <summary>
		/// 测试C
		/// </summary>
		public static async TreeTask TaskD(this TaskTest self)
		{
			TreeTaskToken taskTokenCatch = await self.TreeTaskTokenCatch();
			self.Log($"D！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");
		}
	}
}