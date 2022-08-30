using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WorldTree
{

    class InitialDomainAddSystem : AddSystem<InitialDomain>
    {
        public override void OnAdd(InitialDomain self)
        {
            self.Root.AddChildren<ConsoleTreeView>();
            self.Root.AddChildren<GUIWindow>();
        }
    }
    class ConsoleTreeViewAddSystem : AddSystem<ConsoleTreeView>
    {
        public override void OnAdd(ConsoleTreeView self)
        {

            self.beginVertical1 = self.AddChildren<GUIBeginVertical>();
            self.beginVertical1.Texture = self.GetColorTexture(new Color(0.1f, 0.1f, 0.1f, 1));
            self.beginVertical1.Padding = new RectOffset(5, 5, 5, 5);

            self.beginVertical2 = self.AddChildren<GUIBeginVertical>();
            self.beginVertical2.Texture = self.GetColorTexture(new Color(0.2f, 0.2f, 0.2f, 1));
            self.beginVertical2.Padding = new RectOffset(5, 5, 5, 5);


            self.beginHorizontal1 = self.AddChildren<GUIBeginHorizontal>();
            self.beginHorizontal1.Texture = self.GetColorTexture(new Color(0.1f, 0.1f, 0.1f, 1));
            self.beginHorizontal1.Padding = new RectOffset(5, 5, 5, 5);

            self.beginHorizontal2 = self.AddChildren<GUIBeginHorizontal>();
            self.beginHorizontal2.Texture = self.GetColorTexture(new Color(0.2f, 0.2f, 0.2f, 1));
            self.beginHorizontal2.Padding = new RectOffset(5, 5, 5, 5);


            self.lineHorizontal = self.AddChildren<GUIBox>();
            self.lineHorizontal.Texture = self.GetColorTexture(new Color(0.1f, 0.1f, 0.1f, 1));
            self.lineHorizontal.Height = 2;

            self.lineVertical = self.AddChildren<GUIBox>();
            self.lineVertical.Texture = self.GetColorTexture(new Color(0.1f, 0.1f, 0.1f, 1));
            self.lineVertical.Width = 2;

            self.title = self.AddChildren<GUIBox>();
            self.title.text = "世界树可视化";
            self.title.FontAnchor = TextAnchor.MiddleLeft;


            self.systems = self.RootGetSystemGroup<IConsoleTreeViewItemSystem>();

            self.componentShowSwitchs = self.Root.ObjectPoolManager.Get<UnitDictionary<long, bool>>();
            self.childrenShowSwitchs = self.Root.ObjectPoolManager.Get<UnitDictionary<long, bool>>();

            self.componentShowSwitchs.TryAdd(self.Root.id, true);
            self.childrenShowSwitchs.TryAdd(self.Root.id, true);
            foreach (var item in self.Root.allEntities)
            {
                self.componentShowSwitchs.TryAdd(item.Value.id, true);
                self.childrenShowSwitchs.TryAdd(item.Value.id, true);
            }
            self.currentNode = self.Root;



        }
    }

    class ConsoleTreeViewRemoveSystem : RemoveSystem<ConsoleTreeView>
    {
        public override void OnRemove(ConsoleTreeView self)
        {
            self.componentShowSwitchs.Recycle();
            self.childrenShowSwitchs.Recycle();
        }
    }
    class ConsoleTreeViewEntitySystem : EntitySystem<ConsoleTreeView>
    {
        public override void OnAddEntity(ConsoleTreeView self, Entity entity)
        {
            self.componentShowSwitchs.TryAdd(entity.id, true);
            self.childrenShowSwitchs.TryAdd(entity.id, true);
        }

        public override void OnRemoveEntity(ConsoleTreeView self, Entity entity)
        {
            if (self.currentNode == entity)
            {
                self.currentNode = entity.Parent;
            }
            self.componentShowSwitchs.Remove(entity.id);
            self.componentShowSwitchs.Remove(entity.id);
        }
    }

    class ConsoleTreeViewOnGUISystem : OnGUISystem<ConsoleTreeView>
    {
        public override void OnGUI(ConsoleTreeView self, float deltaTime)
        {
            Rect rect = new Rect(self.rect.x, self.rect.y, self.rect.width * GUIDefault.size, self.rect.height * GUIDefault.size);
            rect = GUILayout.Window(self.GetHashCode(), rect, self.GUIWindowMax, default(string));
            self.rect.x = rect.x;
            self.rect.y = rect.y;

        }
    }
    public class ConsoleTreeView : Entity
    {
        public UnitDictionary<long, bool> componentShowSwitchs;
        public UnitDictionary<long, bool> childrenShowSwitchs;
        public Rect rect = new Rect(0, 0, 800, 600);

        public SystemGroup systems;
        public Entity currentNode;
        public Entity selectNode;
        private Vector2 scrollLogView = Vector2.zero;

        //public GUIStyle Box1;
        //public GUIStyle Box2;
        public GUIBox title;
        public GUIBox lineHorizontal;
        public GUIBox lineVertical;
        public GUIBeginHorizontal beginHorizontal1;
        public GUIBeginHorizontal beginHorizontal2;
        public GUIBeginVertical beginVertical1;
        public GUIBeginVertical beginVertical2;

        public void GUIWindowMax(int windowId)
        {
            
            beginVertical1.Draw();

            beginHorizontal2.Draw();

            title.Draw();

            GUILayout.Space(20);

            lineVertical.Draw();

            GUIDefault.LeftButton(() => { GUIDefault.size--; });
            GUILayout.Label(GUIDefault.size.ToString(), GUIDefault.StyleBlack1, GUIDefault.OptionWidth60);
            GUIDefault.RightButton(() => { GUIDefault.size++; });

            lineVertical.Draw();

            GUILayout.Space(20);

            lineVertical.Draw();

            GUIDefault.LeftButton(() => { rect.width = rect.width - 100; });
            GUILayout.Label(rect.width.ToString(), GUIDefault.StyleBlack1, GUIDefault.OptionWidth60);
            GUIDefault.RightButton(() => { rect.width = rect.width + 100; });
            lineVertical.Draw();

            GUILayout.Space(20);

            lineVertical.Draw();
            GUIDefault.LeftButton(() => { rect.height = rect.height - 100; });
            GUILayout.Label(rect.height.ToString(), GUIDefault.StyleBlack1, GUIDefault.OptionWidth60);
            GUIDefault.RightButton(() => { rect.height = rect.height + 100; });
            lineVertical.Draw();

            GUILayout.FlexibleSpace();

            GUIDefault.CloseButton(() => { this.RemoveSelf(); });

            GUILayout.EndHorizontal();



            lineHorizontal.Draw();

            GUIWindowContent();

            GUILayout.EndVertical();
            GUI.DragWindow();

        }





        public void GUIWindowContent()
        {

            beginHorizontal1.Draw();

            PathNodeView(currentNode);

            GUILayout.EndHorizontal();

            lineHorizontal.Draw();
            //GUIDefault.LineHorizontal();


            beginHorizontal2.Draw();

            GUILayout.FlexibleSpace();
            GUILayout.Label("聚焦", GUIDefault.StyleBlack3);
            GUILayout.Label("全展开", GUIDefault.StyleBlack3);
            GUILayout.Label("全折叠", GUIDefault.StyleBlack3);
            GUILayout.Label("结构/信息", GUIDefault.StyleBlack3);

            GUILayout.EndHorizontal();


            lineHorizontal.Draw();
            //GUIDefault.LineHorizontal();


            beginVertical1.Draw();

            scrollLogView = GUILayout.BeginScrollView(scrollLogView, GUIDefault.StyleBlack2);

            //TraversalRecursive(currentNode);
            ForeachShow(currentNode);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }


        public void TraversalRecursive(Entity node)
        {
            GUILayout.BeginVertical();

            ShowItem(node, this);


            ForeachShow(node);



            GUILayout.EndVertical();
        }


        public void PathNodeView(Entity entity)
        {
            if (entity != null)
            {
                PathNodeView(entity.Parent);
                if (GUILayout.Button(entity.Type.Name, GUIDefault.StyleBlack3, GUILayout.ExpandWidth(false)))
                {
                    currentNode = entity;
                }
            }
        }

        public void ShowItem(Entity self, ConsoleTreeView console)
        {

            bool componentSwitch = console.componentShowSwitchs[self.id];
            bool childrenSwitch = console.childrenShowSwitchs[self.id];

            //GUILayout.BeginHorizontal("Box", GUILayout.Width(600));
            GUILayout.BeginHorizontal(console.selectNode == self ? GUIDefault.StyleBlue : GUIDefault.StyleBlack3, GUILayout.Width(600));
            //EditorGUILayout

            if (self.components != null || self.children != null)
            {
                GUIDefault.FoldoutButton(componentSwitch || childrenSwitch, (bit) =>
                {
                    componentSwitch = !bit;
                    childrenSwitch = componentSwitch;
                });
            }
            else
            {
                GUILayout.Space(25);
            }


            GUILayout.BeginHorizontal();

            GUIDefault.Button(self.Type.Name, () =>
            {
                if (console.selectNode == self)
                {
                    console.currentNode = self;
                }
                console.selectNode = self;
            }, GUILayout.Width(300));


            if (self.components != null)
            {
                if (GUILayout.Button(componentSwitch ? "▼" : "▶", GUIDefault.StyleTransparent, GUILayout.Width(20)))
                {
                    componentSwitch = !componentSwitch;
                }
                GUILayout.Label(self.components.Count.ToString(), GUILayout.Width(80));
            }
            else
            {
                componentSwitch = false;
            }


            if (self.children != null)
            {
                if (GUILayout.Button(childrenSwitch ? "▼" : "▶", GUIDefault.StyleTransparent, GUILayout.Width(20)))
                {
                    childrenSwitch = !childrenSwitch;
                }
                GUILayout.Label(self.children.Count.ToString(), GUILayout.Width(80));
            }
            else
            {
                childrenSwitch = false;
            }

            console.componentShowSwitchs[self.id] = componentSwitch;
            console.childrenShowSwitchs[self.id] = childrenSwitch;

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        public void ForeachShow(Entity node)
        {
            GUILayout.BeginVertical();



            ForeachComponents(node);
            ForeachChildren(node);

            GUILayout.EndVertical();


        }

        public void ForeachComponents(Entity node)
        {

            componentShowSwitchs.TryGetValue(node.id, out bool value);

            if (node.components != null && value)
            {
                //GUIDefault.LineHorizontal();

                GUILayout.BeginHorizontal(GUIDefault.StyleBlack1);
                GUILayout.BeginHorizontal(GUIDefault.StyleBlack2);
                //GUILayout.Space(100);
                GUILayout.Label("Components:", GUIDefault.StyleTransparent, GUIDefault.OptionWidth80);

                GUIDefault.LineVertical();

                GUILayout.BeginVertical();


                foreach (var item in node.components)
                {
                    TraversalRecursive(item.Value);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

            }
        }

        public void ForeachChildren(Entity node)
        {
            childrenShowSwitchs.TryGetValue(node.id, out bool value);
            if (node.children != null && value)
            {
                GUILayout.BeginHorizontal(GUIDefault.StyleBlack1);
                GUILayout.BeginHorizontal(GUIDefault.StyleBlack2);

                GUILayout.Label("Children:", GUIDefault.StyleTransparent, GUIDefault.OptionWidth80);

                //GUILayout.Space(100);

                GUIDefault.LineVertical();

                GUILayout.BeginVertical();



                foreach (var item in node.children)
                {
                    TraversalRecursive(item.Value);
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

            }

        }


    }




    public class ConsoleTreeViewItem : ConsoleTreeViewItemSystem<Entity>
    {
        public override void OnDraw(Entity self, ConsoleTreeView console)
        {

            bool componentSwitch = console.componentShowSwitchs[self.id];
            bool childrenSwitch = console.childrenShowSwitchs[self.id];

            //GUILayout.BeginHorizontal("Box", GUILayout.Width(600));
            GUILayout.BeginHorizontal(console.selectNode == self ? GUIDefault.StyleBlue : GUIDefault.StyleBlack3, GUILayout.Width(600));
            //EditorGUILayout

            if (self.components != null || self.children != null)
            {
                GUIDefault.FoldoutButton(componentSwitch || childrenSwitch, (bit) =>
                {
                    componentSwitch = !bit;
                    childrenSwitch = componentSwitch;
                });
            }
            else
            {
                GUILayout.Space(25);
            }


            GUILayout.BeginHorizontal();

            GUIDefault.Button(self.Type.Name, () =>
            {
                if (console.selectNode == self)
                {
                    console.currentNode = self;
                }
                console.selectNode = self;
            }, GUILayout.Width(300));


            if (self.components != null)
            {
                if (GUILayout.Button(componentSwitch ? "▼" : "▶", GUIDefault.StyleTransparent, GUILayout.Width(20)))
                {
                    componentSwitch = !componentSwitch;
                }
                GUILayout.Label(self.components.Count.ToString(), GUILayout.Width(80));
            }
            else
            {
                componentSwitch = false;
            }


            if (self.children != null)
            {
                if (GUILayout.Button(childrenSwitch ? "▼" : "▶", GUIDefault.StyleTransparent, GUILayout.Width(20)))
                {
                    childrenSwitch = !childrenSwitch;
                }
                GUILayout.Label(self.children.Count.ToString(), GUILayout.Width(80));
            }
            else
            {
                childrenSwitch = false;
            }

            console.componentShowSwitchs[self.id] = componentSwitch;
            console.childrenShowSwitchs[self.id] = childrenSwitch;

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }
    }


}
