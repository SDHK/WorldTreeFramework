using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class HybridCLREditor
{
	[MenuItem("HybridCLR/CopyDlls")]
	public static void CopyDlls()
	{
		CopyHotUpdateDlls();
		CopyAotDll();
	}

	public static void CopyHotUpdateDlls()
	{
		string externalHotUpdatePath = HybridCLRSettings.Instance.externalHotUpdateAssembliyDirs[0];
		string toDir = "Assets/AssetBundles/Dlls/HotUpdateDlls";
		if (Directory.Exists(toDir)) Directory.Delete(toDir, true);
		Directory.CreateDirectory(toDir);
		AssetDatabase.Refresh();

		Debug.Log("从 " + externalHotUpdatePath + "复制到" + toDir);

		foreach (var hotUpdateAssemblie in HybridCLRSettings.Instance.hotUpdateAssemblies)
		{
			File.Copy(Path.Combine(externalHotUpdatePath, $"{hotUpdateAssemblie}.dll"), Path.Combine(toDir, $"{hotUpdateAssemblie}.dll.bytes"), true);
			File.Copy(Path.Combine(externalHotUpdatePath, $"{hotUpdateAssemblie}.pdb"), Path.Combine(toDir, $"{hotUpdateAssemblie}.pdb.bytes"), true);
		}
		Debug.Log("CopyHotUpdateDlls 完成 !!!");

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public static void CopyAotDll()
	{
		BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
		string fromDir = Path.Combine(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir, target.ToString());
		string toDir = "Assets/AssetBundles/Dlls/AotDlls";
		if (Directory.Exists(toDir)) Directory.Delete(toDir, true);
		Directory.CreateDirectory(toDir);
		AssetDatabase.Refresh();

		Debug.Log("从 " + fromDir + "复制到" + toDir);

		foreach (string aotDll in AOTGenericReferences.PatchedAOTAssemblyList)
		{
			File.Copy(Path.Combine(fromDir, aotDll), Path.Combine(toDir, $"{aotDll}.bytes"), true);
		}

		//foreach (string aotDll in HybridCLRSettings.Instance.patchAOTAssemblies)
		//{
		//	File.Copy(Path.Combine(fromDir, aotDll), Path.Combine(toDir, $"{aotDll}.bytes"), true);
		//}

		Debug.Log("CopyAotDlls 完成 !!!");

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	[MenuItem("HybridCLR/MyGenAll")]
	public static void GenAll()
	{
		BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
		CompileDllCommand.CompileDll(target);
		Il2CppDefGeneratorCommand.GenerateIl2CppDef();

		// 这几个生成依赖HotUpdateDlls
		LinkGeneratorCommand.GenerateLinkXml(target);

		//这一步使用我们自己的
		// 生成裁剪后的aot dll
		StripAOTDllCommand.GenerateStripedAOTDlls();

		//GenerateStripedAOTDlls(target, EditorUserBuildSettings.selectedBuildTargetGroup);

		// 桥接函数生成依赖于AOT dll，必须保证已经build过，生成AOT dll
		MethodBridgeGeneratorCommand.GenerateMethodBridge(target);
		ReversePInvokeWrapperGeneratorCommand.GenerateReversePInvokeWrapper(target);
		AOTReferenceGeneratorCommand.GenerateAOTGenericReference(target);
	}
}