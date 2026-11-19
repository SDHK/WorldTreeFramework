/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 视图主体、视图组件与容器。

*/

namespace WorldTree
{
	/// <summary>
	/// 容器级 ViewWidget：能承载子级（层条目、其他 widget）
	/// </summary>
	public abstract class ViewContainer : View
	{

	}

	/// <summary>
	/// 视图主体：能承载组件
	/// </summary>
	public abstract class ViewObject : View
	{

	}

	/// <summary>
	/// 层条目：挂在层的 ViewBranch 上、持有槽位的视图
	/// </summary>
	public abstract class ViewLayerItem : ViewObject
		, ViewOf<ViewLayer>
	{

	}

	/// <summary>
	/// 界面：有自身表现的层条目
	/// </summary>
	public abstract class ViewForm : ViewLayerItem
	{

	}


}
