/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 18:11

* 描述：

*/
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
				string hotPath = "Temp/Bin/Debug";

				byte[] ruleModuleDllBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.ModuleRule.dll"));
				byte[] ruleModulePdbBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.ModuleRule.pdb"));

				byte[] ruleDllBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.Rule.dll"));
				byte[] rulePdbBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.Rule.pdb"));

				byte[] ruleUnityDllBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.UnityRule.dll"));
				byte[] ruleUnityPdbBytes = File.ReadAllBytes(Path.Combine(hotPath, "WorldTree.UnityRule.pdb"));

				Assembly hotfixModuleAssembly = Assembly.Load(ruleModuleDllBytes, ruleModulePdbBytes);

				Assembly hotfixAssembly = Assembly.Load(ruleDllBytes, rulePdbBytes);
				Assembly hotfixUnityAssembly = Assembly.Load(ruleUnityDllBytes, ruleUnityPdbBytes);

				self.assemblyDict[hotfixModuleAssembly.GetName().Name] = hotfixModuleAssembly;
				self.assemblyDict[hotfixAssembly.GetName().Name] = hotfixAssembly;
				self.assemblyDict[hotfixUnityAssembly.GetName().Name] = hotfixUnityAssembly;

				Core.TypeInfo.LoadAssembly(new[] { hotfixModuleAssembly, hotfixAssembly, hotfixUnityAssembly });
				Core.RuleManager.LoadRule();
			}
		}
	}
}