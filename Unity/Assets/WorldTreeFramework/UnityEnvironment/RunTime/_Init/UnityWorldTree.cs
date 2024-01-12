
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

		private void Start()
		{
			Core = new WorldTreeCore();
			Core.View = Core.PoolGetUnit<TreeNodeView>();
			Core.View.Draw(Core);
			Core.Log = Debug.Log;
			Core.LogWarning = Debug.LogWarning;
			Core.LogError = Debug.LogError;
			Core.Awake();

			Core.Root.AddComponent(out UnityWorldHeart _, 0).Run();//添加Unity世界心跳，间隔毫秒为0

			Core.Root.AddComponent(out InitialDomain _);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return)) Debug.Log(Core.ToStringDrawTree());
		}

		private void OnDestroy()
		{
			Core.Dispose();
			Core = null;
		}

	}
}
