/****************************************

* 作者：闪电黑客
* 日期：2024/5/20 15:39

* 描述：分支类型补充生成器

*/
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace WorldTree.SourceGenerator
{

	/// <summary>
	/// 源代码生成器基类
	/// </summary>
	public abstract class SourceGeneratorBase<ConfigT> : ISourceGenerator
		where ConfigT : ProjectGeneratorsConfig, new()
	{
		/// <summary>
		/// 项目生成配置集合
		/// </summary>
		public ConfigT Configs = new();

		/// <summary>
		/// 同一进程里的编译器服务器会并行编多个程序集。生成器里有跨执行共享的静态集合，必须串行 Execute。
		/// </summary>
		private static readonly object ExecuteGate = new object();

		public virtual void Execute(GeneratorExecutionContext context)
		{
			if (!Configs.TryGetValue(context.Compilation.AssemblyName, out var types)) return;
			if (!types.Contains(this.GetType())) return;
			lock (ExecuteGate)
			{
				ExecuteCore(context);
			}
		}

		public abstract void Initialize(GeneratorInitializationContext context);
		public abstract void ExecuteCore(GeneratorExecutionContext context);

	}


	/// <summary>
	/// 项目生成配置集合
	/// </summary>
	public abstract class ProjectGeneratorsConfig
		: Dictionary<string, HashSet<Type>>
	{
		/// <summary>
		/// 生成器的参数数量
		/// </summary>
		public static int ArgumentCount = 0;
	}

}
