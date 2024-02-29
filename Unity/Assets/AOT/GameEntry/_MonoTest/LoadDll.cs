using HybridCLR;
using System.Collections;
using UnityEngine;

namespace WorldTree
{
	public class LoadDll : MonoBehaviour
	{
		private void Start()
		{
			// Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        //Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
			// Editor下无需加载，直接查找获得HotUpdate程序集
			//Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate1");
#endif

			//Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate2.dll.bytes"));

			//Type type = hotUpdateAss.GetType("Hello");
			//         type.GetMethod("Run").Invoke(null, null);
		}

		//		/// <summary>
		//		/// 加载aot的DLL
		//		/// </summary>
		//		/// <returns></returns>
		//		IEnumerator LoadMetadataForAOTDLLs()
		//		{
		////#if !UNITY_EDITOR
		//		HomologousImageMode mode = HomologousImageMode.SuperSet;
		//        //var aotMetadateDllHandle = Addressables.LoadAssetsAsync<TextAsset>("AOTMetadataDLL", null);
		//        yield return aotMetadateDllHandle;
		//        var AOTMetadataDlls = aotMetadateDllHandle.Result;
		//        foreach (var AOTMetadataDll in AOTMetadataDlls)
		//        {
		//            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(AOTMetadataDll.bytes, mode);
		//            Debug.Log($"[HotUpdater] LoadMetadataForAOTAssembly:{AOTMetadataDll.name}. mode:{mode} ret:{err}");
		//        }
		//        //Addressables.Release(aotMetadateDllHandle);
		//		Debug.Log("[HotUpdater] LoadMetadataForAOTDLLs complete!");
		////#endif
		//			yield return null;
		//		}
	}
}