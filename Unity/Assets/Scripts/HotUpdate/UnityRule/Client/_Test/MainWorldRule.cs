/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 15:06

* 描述：

*/
namespace WorldTree
{
	public static partial class MainWorldRule
	{
		static OnAdd<MainWorld> Add = self => self.AddComponent(out InitialDomain c_);

		class MainWorldAddCurveManager : NodeAddComponentRule<MainWorld, CurveManager> { }
		class MainWorldAddTreeTweenManager : NodeAddComponentRule<MainWorld, TreeTweenManager> { }
		class MainWorldAddTreeTaskQueueLockManager : NodeAddComponentRule<MainWorld, TreeTaskQueueLockManager> { }

		class MainWorldAddInitialDomain : NodeAddComponentRule<MainWorld, InitialDomain> { }

	}
}
