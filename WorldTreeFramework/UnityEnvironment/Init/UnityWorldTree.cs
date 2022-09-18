
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述：世界树框架驱动器，一切从这里开始

*/

using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{

    public class UnityWorldTree : MonoBehaviour
    {
        public EntityManager root;

        SystemBroadcast enable;
        SystemBroadcast disable;
        SystemBroadcast update;
        SystemBroadcast lateUpdate;
        SystemBroadcast fixedUpdate;
        SystemBroadcast onGUI;


        private void Start()
        {
            World.Log = Debug.Log;
            World.LogWarning = Debug.LogWarning;
            World.LogError = Debug.LogError;

            root = new EntityManager();

            enable = root.GetSystemBroadcast<IEnableSystem>();
            update = root.GetSystemBroadcast<IUpdateSystem>();
            disable = root.GetSystemBroadcast<IDisableSystem>();

            lateUpdate = root.GetSystemBroadcast<ILateUpdateSystem>();
            fixedUpdate = root.GetSystemBroadcast<IFixedUpdateSystem>();
            onGUI = root.GetSystemBroadcast<IOnGUISystem>();

            root.AddComponent<InitialDomain>();
        }

        private void Update()
        {
            enable?.Send();
            update?.Send(Time.deltaTime);
            disable?.Send();
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

        private void OnGUI()
        {
            onGUI?.Send(0.02f);
        }

        private void OnDestroy()
        {
            enable = null;
            update = null;
            disable = null;
            lateUpdate = null;
            fixedUpdate = null;
            onGUI = null;
            root.Dispose();
        }

    }
}
