/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 异步任务完成类
* 
* 因为没有使用单例，所以必须让构建器执行6号步骤，通过节点创建Task。
* 

*/

namespace WorldTree.Internal
{
	/// <summary>
	/// 异步任务完成类
	/// </summary>
	public class TreeTaskCompleted : TreeTaskBase, ISyncTask
		, ChildOf<INode>
		, AsAwake
        
    {
        public TreeTaskCompleted GetAwaiter() => this;
        public override bool IsCompleted { get; set; }
        public void GetResult() { }
    }
}
