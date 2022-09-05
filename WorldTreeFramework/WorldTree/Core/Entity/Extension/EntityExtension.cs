using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
    public static class EntityExtension
    {
        /// <summary>
        /// 返回用字符串绘制的树
        /// </summary>
        public static string ToStringDrawTree(this Entity entity, string t = "\t")
        {
            string t1 = "\t" + t;
            string str = "";

            str += t1 + $"[{entity.id}] " + entity.ToString() + "\n";

            if (entity.children != null)
            {
                if (entity.children.Count > 0)
                {
                    str += t1 + "   Children:\n";
                    foreach (var item in entity.Children.Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }

            if (entity.components != null)
            {
                if (entity.components.Count > 0)
                {
                    str += t1 + "   Components:\n";
                    foreach (var item in entity.Components.Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }

            return str;
        }


        #region Send

        public static void SendSystem(this Entity self)
        {
            var sendSystems = self.Root.SystemManager.GetSystems<ISendSystem>(self.GetType());

            foreach (ISendSystem sendSystem in sendSystems)
            {
                sendSystem.Invoke(self);
            }
        }
        public static void SendSystem<T1>(this Entity self, T1 arg1)
        {
            var sendSystems = self.Root.SystemManager.GetSystems<ISendSystem<T1>>(self.Type);

            foreach (ISendSystem<T1> sendSystem in sendSystems)
            {
                sendSystem.Invoke(self, arg1);
            }
        }


        #endregion

        #region Call

        public static OutT CallSystem<OutT>(this Entity self)
        {
            var sendSystems = self.Root.SystemManager.GetSystems<ICallSystem<OutT>>(self.Type);

            OutT outT = default(OutT);
            foreach (ICallSystem<OutT> sendSystem in sendSystems)
            {
                outT = sendSystem.Invoke(self);
            }

            return outT;
        }

        public static OutT CallSystem<T1, OutT>(this Entity self, T1 arg1)
        {
            var sendSystems = self.Root.SystemManager.GetSystems<ICallSystem<T1, OutT>>(self.Type);

            OutT outT = default(OutT);
            foreach (ICallSystem<T1, OutT> sendSystem in sendSystems)
            {
                outT = sendSystem.Invoke(self, arg1);
            }

            return outT;
        }

        #endregion


    }
}
