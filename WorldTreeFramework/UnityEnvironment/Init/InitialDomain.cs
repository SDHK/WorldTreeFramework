/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件

*/
namespace WorldTree
{
    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Entity { }


    class _InitialDomain : AddSystem<InitialDomain>
    {
        public override void OnAdd(InitialDomain self)
        {
            self.Domain = self;
            World.Log("初始域启动！");
        }
    }

}
