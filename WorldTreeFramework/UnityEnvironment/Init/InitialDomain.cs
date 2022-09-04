/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件

*/
using System;

namespace WorldTree
{
    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Entity {

       public InitialDomain()
        {
            //Type = typeof(新方案);//实体改标签为子类新方案

            //Type = typeof(Entity);//实体改标签为父类可复用父类系统
        }

    }

    //原系统
    class _InitialDomain : AddSystem<InitialDomain>
    {
        public override void OnAdd(InitialDomain self)
        {

            self.Domain = self;
            World.Log("初始域启动！");
        }
    }


    public class 新方案 : InitialDomain{ }
    class 新方案的加入系统 : AddSystem<InitialDomain>
    {
        public override Type EntityType => typeof(新方案); 
        public override void OnAdd(InitialDomain self)
        {

            self.Domain = self;
            World.Log("初始域启动 新方案！！！！！");
        }
    }



    class 父类的加入系统 : AddSystem<Entity>
    {
   
        public override void OnAdd(Entity self)
        {

            self.Domain = self;
            World.Log("初始域启动 父类系统！！！！！");
        }
    }


}
