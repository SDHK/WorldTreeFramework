/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 球体管理器测试
	/// </summary>
	public partial class SphereManagerTest : Node
		, ComponentOf<InitialDomain>
		, AsGenericBranch<long>
		, AsRule<Awake>
	{
		/// <summary>
		/// 重力
		/// </summary>
		public int G = 10;
		/// <summary>
		/// 球体半径
		/// </summary>
		public int BottomY = 0;
		/// <summary>
		/// 顶部Y坐标
		/// </summary>
		public int TopY = 10;
		/// <summary>
		/// 生成数量
		/// </summary>
		public int SpawnCount = 1000;
		/// <summary>
		/// 球体
		/// </summary>
		public GameObject balls;


		[NodeRule(nameof(UpdateRule<SphereManagerTest>))]
		private static void OnUpdateRule(SphereManagerTest self)
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				self.Log($"SphereManagerTestRule！！!");
				self.Run();
			}
		}

		/// <summary>
		/// 运行
		/// </summary>
		private void Run()
		{
			this.balls = GameObject.Find("Sphere");
			GameObject rootGo = new GameObject("SphereManagerTest");
			rootGo.transform.position = Vector3.zero;

			for (int i = 0; i < this.SpawnCount; ++i)
			{
				var go = UnityEngine.Object.Instantiate(this.balls);
				go.name = "Drop_" + i;

				this.TryGetGeneric((long)i, out SphereTest dropComponent);
				dropComponent.Manager = this;
				dropComponent.GameObject = go;

				dropComponent.Delay = 0.02f * i;
				dropComponent.Mass = UnityEngine.Random.Range(0.5f, 3f);

				Vector3 pos = UnityEngine.Random.insideUnitSphere * 40;
				go.transform.parent = rootGo.transform;
				pos.y = this.TopY;
				go.transform.position = pos;
			}
		}
	}
}