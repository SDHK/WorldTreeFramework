using System;

namespace WorldTree
{
	/// <summary>
	/// 法则执行器异常
	/// </summary>
	[Serializable]
	public class RuleExecutorException : ApplicationException
	{
		public RuleExecutorException() { }
		public RuleExecutorException(string message)
	   : base(message)
		{ }
		public RuleExecutorException(string message, Exception inner)
		: base(message, inner)
		{ }
	}
}
