
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述：世界树框架驱动器，一切从这里开始

*/

using UnityEngine;
using UnityEngine.Profiling;

namespace WorldTree
{

    public class UnityWorldTree : MonoBehaviour
    {
        public WorldTreeCore core;

        RuleActuator enable;
        RuleActuator disable;
        RuleActuator update;
        RuleActuator lateUpdate;
        RuleActuator fixedUpdate;
        //RuleActuator onGUI;


        private void Start()
        {
            World.Log = Debug.Log;
            World.LogWarning = Debug.LogWarning;
            World.LogError = Debug.LogError;

            core = new WorldTreeCore();

            enable = core.GetGlobalNodeRuleActuator<IEnableRule>();
            update = core.GetGlobalNodeRuleActuator<IUpdateRule>();
            disable = core.GetGlobalNodeRuleActuator<IDisableRule>();

            lateUpdate = core.GetGlobalNodeRuleActuator<ILateUpdateRule>();
            fixedUpdate = core.GetGlobalNodeRuleActuator<IFixedUpdateRule>();
            //onGUI = core.GetGlobalNodeRuleActuator<IGuiUpdateRule>();

            core.Root.AddComponent(out InitialDomain _);
        }

        private void Update()
        {
            Profiler.BeginSample("SDHK");

            enable?.Send();
            update?.Send(Time.deltaTime);
            disable?.Send();

            Profiler.EndSample();

            if (Input.GetKeyDown(KeyCode.Return)) Debug.Log(core.ToStringDrawTree());
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
            core?.Dispose();
            core = null;

            enable = null;
            update = null;
            disable = null;
            lateUpdate = null;
            fixedUpdate = null;
            //onGUI = null;
        }
        
    }
}
