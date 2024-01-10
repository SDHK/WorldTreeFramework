using System;

namespace WorldTree
{

    /// <summary>
    /// LateUpdate法则接口
    /// </summary>
    public interface ILateUpdateRule : ISendRuleBase { }
    /// <summary>
    /// LateUpdate法则
    /// </summary>
    public abstract class LateUpdateRule<T> : SendRuleBase<T, ILateUpdateRule> where T : class, INode, AsRule<ILateUpdateRule> { }

    /// <summary>
    /// LateUpdate法则接口
    /// </summary>
    public interface ILateUpdateTimeRule : ISendRuleBase<TimeSpan> { }
    /// <summary>
    /// LateUpdate法则
    /// </summary>
    public abstract class LateUpdateTimeRule<T> : SendRuleBase<T, ILateUpdateTimeRule, TimeSpan> where T : class, INode, AsRule<ILateUpdateTimeRule> { }

}
