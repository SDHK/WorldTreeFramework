/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 给节点添加组件的法则
* 
* 用于节点的饿汉单例
* 
* 法则是可多播的，执行顺序由名字排序
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 给节点添加组件的法则
    /// </summary>
    /// <typeparam name="N">节点</typeparam>
    /// <typeparam name="C">组件</typeparam>
    /// <remarks>用于节点的饿汉单例</remarks>
    public abstract class NodeAddRule<N, C> : AddRule<N>
        where N : class, INode
        where C : class, INode, ComponentOf<N>
    {
        public override void OnEvent(N self)
        {
            self.AddComponent(out C _);
        }
    }

    /// <summary>
    /// 给根节点添加组件的法则
    /// </summary>
    /// <typeparam name="C">根节点组件</typeparam>
    /// <remarks>用于根节点的饿汉单例</remarks>
    public abstract class RootAddRule<C> : NodeAddRule<WorldTreeRoot, C> where C : class, INode, ComponentOf<WorldTreeRoot> { }

}
