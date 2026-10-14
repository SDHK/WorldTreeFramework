/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{
	/// <summary>
	/// 保存编译器状态机的池化对象。任务完成时还池。
	/// </summary>
	public class WorldTaskStateMachine : Unit
	{
		/// <summary>
		/// 编译器状态机
		/// </summary>
		private IAsyncStateMachine stateMachine;

		/// <summary>
		/// 装箱保存状态机
		/// </summary>
		public void SetStateMachine<T>(ref T stateMachine) where T : IAsyncStateMachine
			=> this.stateMachine = stateMachine;

		/// <summary>
		/// 继续执行
		/// </summary>
		public void MoveNext() => stateMachine?.MoveNext();

		public override void OnDispose()
		{
			stateMachine = null;
			base.OnDispose();
		}
	}
}
