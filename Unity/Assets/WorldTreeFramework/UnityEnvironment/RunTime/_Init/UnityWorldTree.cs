
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述：世界树框架驱动器，一切从这里开始

*/

using UnityEngine;

namespace WorldTree
{

	public class UnityWorldTree : MonoBehaviour
	{
		public WorldTreeCore Core;
		public WorldTreeCore ViewCore;

		private void Start()
		{
			ViewCore = new WorldTreeCore();//调试用的可视化框架
			Core = new WorldTreeCore();//主框架

			ViewCore.Log = Debug.Log;
			ViewCore.LogWarning = Debug.LogWarning;
			ViewCore.LogError = Debug.LogError;

			Core.Log = Debug.Log;
			Core.LogWarning = Debug.LogWarning;
			Core.LogError = Debug.LogError;

			ViewCore.Awake(); //可视化框架初始化

			//可视化节点赋值给主框架
			Core.View = ViewCore.Root.AddChild(out TreeNodeUnityView _, (INode)Core, TypeInfo<INode>.Default);

			Core.Awake();//主框架初始化

			Core.Root.AddComponent(out UnityWorldHeart _, 0).Run();//主框架添加Unity世界心跳，间隔毫秒为0

			Core.Root.AddComponent(out InitialDomain _);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return)) Debug.Log(Core.ToStringDrawTree());
		}
		private void OnApplicationQuit()
		{
			Core.Dispose();
			ViewCore.Dispose();
			Core = null;
			ViewCore = null;
		}

		private void OnDestroy()
		{
			Core?.Dispose();
			ViewCore?.Dispose();
			Core = null;
			ViewCore = null;
		}
	}
}
