
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述：世界树框架驱动器，一切从这里开始

*/

using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace WorldTree
{

    public class UnityWorldTree : MonoBehaviour
    {
        public System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public WorldTreeCore Core;

        GlobalRuleActuator<IEnableRule> enable;
        GlobalRuleActuator<IDisableRule> disable;
        GlobalRuleActuator<IUpdateRule> update;
        GlobalRuleActuator<ILateUpdateRule> lateUpdate;
        GlobalRuleActuator<IFixedUpdateRule> fixedUpdate;
        //GlobalRuleActuator<IGuiUpdateRule> onGUI;


        private void Start()
        {
            World.Log = Debug.Log;
            World.LogWarning = Debug.LogWarning;
            World.LogError = Debug.LogError;

            Core = new WorldTreeCore();

            Core.TryGetGlobalRuleActuator(out enable);
            Core.TryGetGlobalRuleActuator(out update);
            Core.TryGetGlobalRuleActuator(out disable);

            Core.TryGetGlobalRuleActuator(out lateUpdate);
            Core.TryGetGlobalRuleActuator(out fixedUpdate);
            //Core.TryGetGlobalRuleActuator(out onGUI);


            Core.Root.AddComponent(out InitialDomain _);
        }

        private void Update()
        {

            /* 代码执行过程 */

            Profiler.BeginSample("SDHK");

            //sw.Restart();
            enable?.Send();
            update?.Send(Time.deltaTime);
            disable?.Send();

            //sw.Stop();
            //World.Log($"毫秒: {sw.ElapsedMilliseconds}");

            Profiler.EndSample();

            if (Input.GetKeyDown(KeyCode.Return)) Debug.Log(Core.ToStringDrawTree());
        }

        private void LateUpdate()
        {
            lateUpdate?.Send(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            fixedUpdate?.Send(Time.fixedDeltaTime);
        }

        //private void OnGuiUpdate()
        //{
        //    onGUI.Send(0.02f);
        //}

        private void OnDestroy()
        {
            Core?.Dispose();
            Core = null;

            enable = null;
            update = null;
            disable = null;
            lateUpdate = null;
            fixedUpdate = null;
            //onGUI = null;
        }

    }
}
