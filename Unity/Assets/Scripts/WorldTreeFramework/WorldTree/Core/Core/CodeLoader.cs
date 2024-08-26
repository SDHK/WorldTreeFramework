/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/14 20:30

* 描述： 世界树根
* 挂载核心启动后的管理器组件

*/

using System.Collections.Generic;
using System.Reflection;

namespace WorldTree
{
	/// <summary>
	/// 热重载
	/// </summary>
	public interface HotReload : ISendRule, IMethodRule { }

	/// <summary>
	/// 代码加载器
	/// </summary>
	public class CodeLoader : Node
		, ComponentOf<WorldTreeRoot>
		, AsAwake
		, AsHotReload
	{
		/// <summary>
		/// 程序集
		/// </summary>
		public List<Assembly> assemblyList = new();
	}
}
