using UnityEngine;
using YooAsset;

public class GameEntry : MonoBehaviour
{
	public ResourcePackage package;

	// Start is called before the first frame update
	void Start()
    {
		

	}

	public void Load()
	{

		// 初始化资源系统
		YooAssets.Initialize();

		// 创建默认的资源包
		package = YooAssets.CreatePackage("DefaultPackage");

		// 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
		YooAssets.SetDefaultPackage(package);


		var initParameters = new OfflinePlayModeParameters();

		package.InitializeAsync(initParameters).Completed += (a) =>
		{
			package.LoadAssetAsync<GameObject>("MainWindow").Completed += GameEntry_Completed;
		};
	}



	private void GameEntry_Completed(AssetHandle obj)
	{
		GameObject gameObject = obj.AssetObject as GameObject;
		Debug.Log($"YooAsset ??? : {gameObject.name}");


	}

}
