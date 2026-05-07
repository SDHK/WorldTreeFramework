/****************************************

* 作者：闪电黑客
* 日期：2024/6/3 10:29

* 描述：

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 任务测试
	/// </summary>
	public partial class TaskTest : Node, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 树任务
		/// </summary>
		public TreeTask treeTask;
		/// <summary>
		/// 树任务令牌
		/// </summary>
		public TreeTaskToken treeTaskToken;

		[NodeRule(nameof(UpdateRule<TaskTest>))]
		private static void OnUpdateRule(TaskTest self)
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


		/// <summary>
		/// 测试
		/// </summary>
		public async TreeTask Test()
		{
			TreeTaskToken taskTokenCatch = await this.TreeTaskTokenCatch();
			//taskTokenCatch.TimeOut(8).Coroutine();



			//this.Log($"A！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");

			while (taskTokenCatch.State != TokenState.Cancel)
			{
				await this.TestB();
				await this.AsyncDelay(TimeSpan.FromSeconds(1));

				if (taskTokenCatch.State == TokenState.Cancel) return;

				await this.TaskC();
				await this.AsyncDelay(TimeSpan.FromSeconds(6));

				if (taskTokenCatch.State == TokenState.Cancel) return;
				await this.TaskD();
			}
		}

		/// <summary>
		/// 测试B
		/// </summary>
		public async TreeTask TestB()
		{
			//await this.TaskC();
			//await this.AsyncDelay(4);

			TreeTaskToken taskTokenCatch = await this.TreeTaskTokenCatch();
			this.Log($"B！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");
		}

		/// <summary>
		/// 测试C
		/// </summary>
		public async TreeTask TaskC()
		{
			TreeTaskToken taskTokenCatch = await this.TreeTaskTokenCatch();
			this.Log($"C！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");
		}

		/// <summary>
		/// 测试C
		/// </summary>
		public async TreeTask TaskD()
		{
			TreeTaskToken taskTokenCatch = await this.TreeTaskTokenCatch();
			this.Log($"D！令牌捕获:{(taskTokenCatch == null ? null : taskTokenCatch.Id)}");
		}

	}

	/// <summary>
	/// 测试
	/// </summary>
	public class TestClass1
	{
		/// <summary>
		/// 测试方法
		/// </summary>
		public void Test() { }
	}
}