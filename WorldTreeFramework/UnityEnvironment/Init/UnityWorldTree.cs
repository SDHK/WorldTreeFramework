
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
        public WorldTreeRoot root;

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

            root = new WorldTreeRoot();

            enable = root.GetGlobalNodeRuleActuator<IEnableRule>();
            update = root.GetGlobalNodeRuleActuator<IUpdateRule>();
            disable = root.GetGlobalNodeRuleActuator<IDisableRule>();

            lateUpdate = root.GetGlobalNodeRuleActuator<ILateUpdateRule>();
            fixedUpdate = root.GetGlobalNodeRuleActuator<IFixedUpdateRule>();
            //onGUI = root.GetGlobalNodeRuleActuator<IGuiUpdateRule>();

            root.AddComponent<InitialDomain>();
        }

        private void Update()
        {
            Profiler.BeginSample("SDHK");

            enable?.Send();
            update?.Send(Time.deltaTime);
            disable?.Send();

            Profiler.EndSample();

            if (Input.GetKeyDown(KeyCode.Return)) Debug.Log(root.ToStringDrawTree());
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
            root?.Dispose();
            root = null;

            enable = null;
            update = null;
            disable = null;
            lateUpdate = null;
            fixedUpdate = null;
            //onGUI = null;
        }
        
    }
}
