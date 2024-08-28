using System;
using System.IO;
using System.Reflection;

namespace WorldTree
{
	public static class UnityCodeLoaderRule
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
				if (!Define.IsEditor) return;

				self.Log("热重载！！！");

				string hotPath = "Assets/AssetBundles/Dlls/HotUpdateDlls";

				byte[] ruleDllBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.Rule.dll.bytes"));
				byte[] rulePdbBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.Rule.pdb.bytes"));

				byte[] ruleUnityDllBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.RuleUnity.dll.bytes"));
				byte[] ruleUnityPdbBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.RuleUnity.pdb.bytes"));

				Assembly hotfixAssembly = Assembly.Load(ruleDllBytes, rulePdbBytes);
				Assembly hotfixUnityAssembly = Assembly.Load(ruleUnityDllBytes, ruleUnityPdbBytes);

				self.assemblyDict[hotfixAssembly.GetName().Name] = hotfixAssembly;
				self.assemblyDict[hotfixUnityAssembly.GetName().Name] = hotfixUnityAssembly;

				Core.TypeInfo.LoadAssembly(new[] { hotfixAssembly, hotfixUnityAssembly });
				Core.RuleManager.LoadRule();
			}
		}
	}
}