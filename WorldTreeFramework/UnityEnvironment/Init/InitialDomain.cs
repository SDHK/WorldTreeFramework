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
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace WorldTree
{

    ////抽象基类
    //public abstract class Panel : Unit//IAdd,IUpdate,IRemove
    //{
    //    public Transform UIContent;
    //    //抽象生命周期
    //}


    ////这个是C : 带有生命周期，绑定事件和Model全写这里面
    //public class MainUI : Panel
    //{
    //    public MainUIView view;
    //    public void OnAdd()
    //    {
    //        view = new MainUIView();
    //    }
    //}

    ////这个是V:一般由编辑器工具自动生成，主要是获取到这个预制体身上需要的组件引用
    //public class MainUIView
    //{
    //    public Panel panel;

    //    public Button button;
    //    public Image image;
    //    public void Init()
    //    {
    //        button =  panel.UIContent.Find("Button").GetComponent<Button>();
    //        image =  panel.UIContent.Find("Image").GetComponent<Image>();
    //    }

    //}

    ////管理器
    //class UIManager
    //{
    //    Dictionary<Type, Panel> Panels = new Dictionary<Type, Panel>();
    //    public T Show<T>()
    //        where T : Panel, new()
    //    {
    //        T panel = new T();//这里应该是对象池

    //        GameObject gameObject = Addressables.LoadAsset<GameObject>(typeof(T).Name);
    //        panel.UIContent = GameObject.Instantiate(gameObject);//这里应该是对象池
    //        return panel;
    //    }

    //}


    /// <summary>
    /// 初始域
    /// </summary>
    public class InitialDomain : Entity
    {
        public int index = 0;
    }
    //内联函数，缩短函数调用时间
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    class _InitialDomain : AddSystem<InitialDomain>
    {
        public override void OnAdd(InitialDomain self)
        {
            //Dictionary<string,int> dic = self.AddComponent<EntityDictionary<string,int>>().Value;

            World.Log("初始域启动！");

            //GameObject gameObject = await Addressables.InstantiateAsync("MainWindow").Task;

            //GameObject gameObject = await self.AddressablesInstantiateAsync("MainWindow");

            //self.Root.AddComponent<WindowManager>().Show<MainWindow>().Coroutine();


            //GetGroundPoint(Vector3(1,1), )

        }


        /// <summary>
        /// 获取地面的点
        /// </summary>
        /// <param name="origin">射线点</param>
        /// <param name="vector">向量</param>
        /// <param name="groundY">地面Y轴坐标</param>
        public Vector3 GetGroundPoint(Vector3 origin, Vector3 vector, float groundY = 0)
        {
            Vector3 point = origin + vector;

            if (point.y != groundY)
            {
                Vector3 pointA = new Vector3(point.x, groundY, point.z);
                float angle = Mathf.Asin(Mathf.Abs(vector.y) / vector.magnitude) * Mathf.Rad2Deg;
                float edge = (point - pointA).magnitude / Mathf.Sin(angle * Mathf.Deg2Rad);
                point = ((vector.magnitude + ((point.y < groundY) ? -edge : edge)) * vector) + origin;
            }
            return point;
        }


    }

    //静态监听 ：指定 InitialDomain 监听 全局 Node1 的添加 事件
    class InitialDomainListenerNode1AddSystem : ListenerAddSystem<InitialDomain, Node1, ISystem>
    {
        public override void OnAdd(InitialDomain self, Node1 entity)
        {
            Debug.Log($"Add Node1:{entity.id}");
        }
    }

    //静态监听 ：指定 InitialDomain 监听 全局拥有IEnbleSystem  实体 的添加 事件
    class InitialDomainListenerAddSystem : ListenerAddSystem<InitialDomain, Entity, IEnableSystem>
    {
        public override void OnAdd(InitialDomain self, Entity entity)
        {
            Debug.Log($"Add Node:{entity.id}");
        }
    }


    //动态监听 ：指定 InitialDomain 默认监听 全局Entity的添加 事件 。可切换变更监听目标
    class InitialDomainListenerEntityAddSystem : ListenerAddSystem<InitialDomain>
    {
        public override void OnAdd(InitialDomain self, Entity entity)
        {
            Debug.Log($"Add Entity:{entity.id}{entity}");
        }
    }


    class InitialDomainListenerRemoveSystem : ListenerRemoveSystem<InitialDomain, Entity, IEnableSystem>
    {
        public override void OnRemove(InitialDomain self, Entity entity)
        {
            Debug.Log($"Remove Node:{entity.id}");
        }
    }

    class InitialDomainUpdateSystem : UpdateSystem<InitialDomain>
    {
        public override void Update(InitialDomain self, float deltaTime)
        {
            World.Log("初始域Update！");


            if (Input.GetKeyDown(KeyCode.Q))
            {
                self.AddComponent<Node>()
                    .Test().Coroutine()
                    ;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                self.RemoveComponent<Node>();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                self.ListenerSwitchesTarget(typeof(Node), ListenerState.Entity);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                self.ListenerSwitchesTarget(typeof(IUpdateSystem), ListenerState.System);
            }


        }
    }
}
