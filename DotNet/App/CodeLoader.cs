using System;
using System.IO;
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
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.GetName().Name == "Node")
				{
					this.assembly = assembly;
					break;
				}
			}
			Assembly hotfixAssembly = this.LoadHotfix();

			Core.TypeInfo.ReLoadAssembly(new[] { hotfixAssembly });
		}

		/// <summary>
		/// 加载热更程序集
		/// </summary>
		private Assembly LoadHotfix()
		{
			//assemblyLoadContext?.Unload();
			//GC.Collect();
			assemblyLoadContext = new AssemblyLoadContext("Rule", true);
			byte[] dllBytes = File.ReadAllBytes("./Rule.dll");
			byte[] pdbBytes = File.ReadAllBytes("./Rule.pdb");
			Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
			return hotfixAssembly;
		}
	}
}