using System;

namespace WorldTree
{

    /// <summary>
    /// FixedUpdate法则接口
    /// </summary>
    public interface IFixedUpdateRule : ISendRuleBase { }

    /// <summary>
    /// FixedUpdate法则
    /// </summary>
    public abstract class FixedUpdateRule<T> : SendRuleBase<T, IFixedUpdateRule> where T : class, INode, AsRule<IFixedUpdateRule> { }


    /// <summary>
    /// FixedUpdate法则接口
    /// </summary>
    public interface IFixedUpdateTimeRule : ISendRuleBase<TimeSpan> { }

    /// <summary>
    /// FixedUpdate法则
    /// </summary>
    public abstract class FixedUpdateTimeRule<T> : SendRuleBase<T, IFixedUpdateTimeRule, TimeSpan> where T : class, INode, AsRule<IFixedUpdateTimeRule> { }
}
