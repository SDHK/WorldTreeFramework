/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 单例法则
* 
* 实现思路为给根节点挂组件
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 饿汉单例法则接口
    /// </summary>
    public interface ISingletonEagerRule : ISendRule { }


    /// <summary>
    /// 饿汉单例法则：生成组件挂在根节点下
    /// </summary>
    public abstract class SingletonEagerRule<N> : SendRuleBase<ISingletonEagerRule, N>, ISingletonEagerRule
        where N : class, INode, ComponentOfNode
    {
        public override void Invoke(INode self)
        {
            self.AddComponent(out N _);
        }
    }
}
