using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace WorldTree
{
	public static class NetCodeLoaderRule
	{
		class Add : AddRule<CodeLoader>
		{
			protected override void Execute(CodeLoader self)
			{
				self.assemblyDict.Clear();
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					self.assemblyDict[assembly.GetName().Name] = assembly;
				}
			}
		}


		class HotReload : HotReloadRule<CodeLoader>
		{
			protected override void Execute(CodeLoader self)
			{
				self.Log("热重载！！！");

				if (!File.Exists("./HotReload/Rule.dll"))
				{
					self.Log("Rule.dll不存在！！！");
					return;
				}

				//assemblyLoadContext?.Unload();
				//GC.Collect();
				AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext("Rule", true);
				byte[] dllBytes = File.ReadAllBytes("./HotReload/Rule.dll");
				byte[] pdbBytes = File.ReadAllBytes("./HotReload/Rule.pdb");
				Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

				self.assemblyDict[hotfixAssembly.GetName().Name] = hotfixAssembly;

				Core.TypeInfo.LoadAssembly([hotfixAssembly]);
				Core.RuleManager.LoadRule();
			}
		}
	}
}