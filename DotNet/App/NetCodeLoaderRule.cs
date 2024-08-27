using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace WorldTree
{


	public static class NetCodeLoaderRule
	{
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

				self.assemblyList.Clear();
				//assemblyLoadContext?.Unload();
				//GC.Collect();
				AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext("Rule", true);
				byte[] dllBytes = File.ReadAllBytes("./HotReload/Rule.dll");
				byte[] pdbBytes = File.ReadAllBytes("./HotReload/Rule.pdb");
				Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					switch (assembly.GetName().Name)
					{
						case "App":
							self.assemblyList.Add(assembly); break;
						case "Core":
							self.assemblyList.Add(assembly); break;
						case "Node":
							self.assemblyList.Add(assembly); break;
					}
				}

				self.assemblyList.Add(hotfixAssembly);
				Core.TypeInfo.ReLoadAssembly(self.assemblyList);
				Core.RuleManager.LoadRule();
			}
		}
	}
}