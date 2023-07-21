
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 10:35

* 描述： 

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 异步加载资源法则接口
    /// </summary>
    public interface ILoadAssetAsyncRule : ICallRuleAsync<string, Object> { }
    /// <summary>
    /// 异步加载资源法则
    /// </summary>
    public abstract class LoadAssetAsyncRule<N> : CallRuleAsyncBase<N, ILoadAssetAsyncRule, string, Object> where N : class, INode, AsRule<ILoadAssetAsyncRule> { }
}
