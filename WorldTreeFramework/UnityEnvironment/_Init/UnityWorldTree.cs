
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
        public System.Diagnostics. Stopwatch sw = new System.Diagnostics.Stopwatch();

        public WorldTreeCore Core;

        IRuleActuator<IEnableRule> enable;
        IRuleActuator<IDisableRule> disable;
        IRuleActuator<IUpdateRule> update;
        IRuleActuator<ILateUpdateRule> lateUpdate;
        IRuleActuator<IFixedUpdateRule> fixedUpdate;
        //RuleActuator onGUI;


        private void Start()
        {
            World.Log = Debug.Log;
            World.LogWarning = Debug.LogWarning;
            World.LogError = Debug.LogError;

            Core = new WorldTreeCore();

            enable = Core.GetGlobalNodeRuleActuator<IEnableRule>();
            update = Core.GetGlobalNodeRuleActuator<IUpdateRule>();
            disable = Core.GetGlobalNodeRuleActuator<IDisableRule>();

            lateUpdate = Core.GetGlobalNodeRuleActuator<ILateUpdateRule>();
            fixedUpdate = Core.GetGlobalNodeRuleActuator<IFixedUpdateRule>();
            //onGUI = Core.GetGlobalNodeRuleActuator<IGuiUpdateRule>();
            

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
