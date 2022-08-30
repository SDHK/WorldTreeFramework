using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{

    public class ConsoleManager : Entity
    {
        public UnitDictionary<long, bool> ViewShows;
        public SystemGroup systems;

    }

    class ConsoleManagerAddSystem : AddSystem<ConsoleManager>
    {
        public override void OnAdd(ConsoleManager self)
        {
            self.ViewShows = self.Root.ObjectPoolManager.Get<UnitDictionary<long, bool>>();
            foreach (var item in self.Root.allEntities)
            {
                self.ViewShows.TryAdd(item.Key, true);
            }
        }
    }

    class ConsoleManagerRemoveSystem : RemoveSystem<ConsoleManager>
    {
        public override void OnRemove(ConsoleManager self)
        {
            self.ViewShows.Recycle();
        }
    }



    class ConsoleManagerEntitySystem : EntitySystem<ConsoleManager>
    {
        public override void OnAddEntity(ConsoleManager self, Entity entity)
        {
            self.ViewShows.TryAdd(entity.id, false);
        }

        public override void OnRemoveEntity(ConsoleManager self, Entity entity)
        {
            self.ViewShows.Remove(entity.id);
        }
    }

    //主控窗口Log消息：大小窗，FPS显示

    //全局式树状图：多开，焦点

    //跟踪挂载式信息面板


    //窗口属于组件绘制父物体UI代码
    //每个代码独立窗口
    public class ConsoleWindow : Entity
    {
        public SystemGroup systems;

        public Rect rect = new Rect(0, 0, 1000, 1000);
        public void GUIWindowMax(int windowId)
        {
            GUI.DragWindow();

            //子物体绘制
        }
    }

    //class ConsoleWindowOnGUISystem : OnGUISystem<ConsoleWindow>
    //{
    //    public override void OnGUI(ConsoleWindow self, float deltaTime)
    //    {
    //        if (Event.current.type != EventType.Repaint)
    //        {
    //            self.rect = GUI.Window((int)self.id, self.rect, self.GUIWindowMax, "世界树控制台");
    //        }

    //    }

    //}

    class ConsoleWindowEvent : EventDelegate { }

    public class WorldTreeConsole : Entity
    {
        public void GUITreeView()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(this.Parent.ToString());

            GUILayout.BeginHorizontal();


            GUILayout.Space(40);


            GUILayout.BeginVertical();

            UnitList<Type> TypeKeys = this.Root.ObjectPoolManager.Get<UnitList<Type>>();
            foreach (var item in this.Parent.Components)
            {
                TypeKeys.Add(item.Key);
            }

            foreach (Type item in TypeKeys)
            {
                if (this.Parent.TryGetComponent(item, out Entity component))
                {
                    if (component.Type != this.Type)
                    {
                        component.AddComponent<WorldTreeConsole>().GUITreeView();
                    }
                }
            }
            TypeKeys.Recycle();

            UnitList<long> Keys = this.Root.ObjectPoolManager.Get<UnitList<long>>();

            foreach (var item in this.Parent.Children)
            {
                Keys.Add(item.Key);
            }

            foreach (long item in Keys)
            {
                if (this.Parent.TryGetChildren(item, out Entity entity))
                {
                    entity.AddComponent<WorldTreeConsole>().GUITreeView();
                }
            }
            Keys.Recycle();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
