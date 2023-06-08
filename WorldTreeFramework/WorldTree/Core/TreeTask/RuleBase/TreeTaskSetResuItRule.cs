using WorldTree;

namespace WorldTree
{
   
    /// <summary>
    /// 树任务设置结果法则接口
    /// </summary>
    public interface ITreeTaskSetResuItRule : ISendRuleBase { }
    /// <summary>
    /// 树任务设置结果法则
    /// </summary>
    public abstract class TreeTaskSetResuItRule<N> : SendRuleBase<N, ITreeTaskSetResuItRule> where N : class, INode, AsRule<ITreeTaskSetResuItRule> { }


    /// <summary>
    /// 泛型树任务设置结果法则接口
    /// </summary>
    public interface ITreeTaskSetResuItRule<T1> : ISendRuleBase<T1> { }
    /// <summary>
    /// 泛型树任务设置结果法则
    /// </summary>
    public abstract class TreeTaskSetResuItRule<N,T1> : SendRuleBase<N, ITreeTaskSetResuItRule<T1>,T1> where N : class, INode, AsRule<ITreeTaskSetResuItRule<T1>> { }


}
