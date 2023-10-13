using System.Runtime.CompilerServices;

namespace WorldTree.Internal
{

	public interface ITreeTaskStateMachine : IUnitPoolEventItem
	{
		public void MoveNext();
	}

	public class TreeTaskStateMachine<T> : UnitPoolItem, ITreeTaskStateMachine
		where T : IAsyncStateMachine
	{
		public void SetStateMachine(ref T stateMachine)
		{
			StateMachine = stateMachine;
		}

		private T StateMachine;
		public void MoveNext() => StateMachine.MoveNext();
	}
}
