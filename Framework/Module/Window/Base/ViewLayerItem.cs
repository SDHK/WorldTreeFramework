/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 视图主体、视图组件与容器。

*/

namespace WorldTree
{
	/// <summary>
	/// 加载物体：有资源地址（类型名），触发加载的视图
	/// </summary>
	public abstract class ViewObject : View
	{

	}

	/// <summary>
	/// 子级/组件：不触发加载，由父级在装配时按字段绑定
	/// </summary>
	public abstract class ViewWidget : View
	{

	}

	/// <summary>
	/// 容器级 ViewWidget：能承载子级（窗口、其他 widget）
	/// </summary>
	public abstract class ViewContainer : ViewWidget
	{

	}

	/// <summary>
	/// 层条目：被容器承载、持有槽位的视图
	/// </summary>
	public abstract class ViewLayerItem : ViewObject
		, ComponentOf<ViewContainer>
	{

	}

	/// <summary>
	/// UI 窗口/面板：有自身表现的层条目
	/// </summary>
	public abstract class ViewWindow : ViewLayerItem
	{

	}
}
