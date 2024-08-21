using System;
using System.Reflection;
using System.Runtime.Loader;

namespace WorldTree
{
	/// <summary>
	/// 代码加载器
	/// </summary>
	public class CodeLoader : Node
		, ComponentOf<WorldTreeCore>
		, AsAwake
	{
		/// <summary>
		/// 程序集加载上下文
		/// </summary>
		private AssemblyLoadContext assemblyLoadContext;
		
		/// <summary>
		/// 程序集
		/// </summary>
		private Assembly assembly;



		public override void OnCreate()
		{
			base.OnCreate();

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();


		}
	}
}