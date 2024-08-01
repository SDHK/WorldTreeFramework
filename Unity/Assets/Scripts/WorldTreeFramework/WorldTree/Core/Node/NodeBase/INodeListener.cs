
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 15:57

* 描述： 节点监听器最底层接口
* 
* 

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 监听器忽略标记
	/// </summary>
	/// <remarks>避免核心启动时监听处理出现死循环</remarks>
	public interface IListenerIgnorer { }

	/// <summary>
	/// 节点监听器接口
	/// </summary>
	public interface INodeListener : INode
	    , AsListenerAddRule
		, AsListenerRemoveRule
	{ }

	
}
