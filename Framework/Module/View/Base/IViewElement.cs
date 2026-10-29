/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 平台控件接口。

*/

namespace WorldTree
{
	/// <summary>
	/// 视图元素：具体平台实现的显示元素接口，绑定到 ViewBind。
	/// </summary>
	public interface IViewElement { }



	//V和VB都是纯数据，V是初始化数据，VB负责桥接具体平台元素。

	//Register子级组件> Open，Refresh，Close> UnRegister子级组件，方法写到View里。
	//Close融入Dispose
	//show融入IsActive
	//由于视图都有层级概念，所以分支考虑使用ListNode？
	//父级需要知道子级的Open事件

	//容器组件需要知道子组件的Open事件
	//如果View可以作为任何对象。那么场景根节点可以是View，而内部物体可以视为一个Item。
}
