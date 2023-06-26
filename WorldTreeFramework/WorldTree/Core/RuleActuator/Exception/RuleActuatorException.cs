using System;

namespace WorldTree
{
    /// <summary>
    /// 法则执行器异常
    /// </summary>
    [Serializable]
    public class RuleActuatorException : ApplicationException
    {
        public RuleActuatorException() { }
        public RuleActuatorException(string message)
       : base(message)
        { }
        public RuleActuatorException(string message, Exception inner)
        : base(message, inner)
        { }
    }
}
