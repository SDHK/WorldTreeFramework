/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/14 20:30

* 描述： 热重载组件

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
		, ComponentOf<World>
		, AsAwake
		, AsHotReload
	{
		/// <summary>
		/// 程序集加载上下文
		/// </summary>
		public object AssemblyLoadContext = null;

		/// <summary>
		/// 程序集字典
		/// </summary>
		public Dictionary<string, Assembly> assemblyDict = new();
	}
}
