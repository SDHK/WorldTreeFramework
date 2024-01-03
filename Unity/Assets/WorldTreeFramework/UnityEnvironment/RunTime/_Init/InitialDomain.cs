/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件
* 
* 然而框架还没完成，目前用于功能测试

*/

namespace WorldTree
{
	/// <summary>
	/// 初始域
	/// </summary>
	public class InitialDomain : DynamicNodeListener, ComponentOf<INode>
		, AsRule<IAwakeRule>
		, AsRule<IFixedUpdateTimeRule>
		, AsRule<ILateUpdateTimeRule>
	{ }

	public static class InitialDomainRule
	{

		//测试框架功能
		class AddRule : AddRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
			{
				self.Log($"初始域启动！！");
				//WorldTreeCore core = new WorldTreeCore();
				//core.Log = self.Core.Log;
				//core.LogWarning = self.Core.LogWarning;
				//core.LogError = self.Core.LogError;
				//core.Awake();
				//self.GraftComponent(core);
				self.Core.AddWorld(out WorldTreeCore core, isPool: false);
				core.Log = self.Core.Log;
				core.LogWarning = self.Core.LogWarning;
				core.LogError = self.Core.LogError;
				core.Awake();
			}
		}
	}
}
