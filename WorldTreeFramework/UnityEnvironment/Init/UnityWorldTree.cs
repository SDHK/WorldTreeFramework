
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

            update = root.GetSystemBroadcast<IUpdateSystem>();
            lateUpdate = root.GetSystemBroadcast<ILateUpdateSystem>();
            fixedUpdate = root.GetSystemBroadcast<IFixedUpdateSystem>();
            onGUI = root.GetSystemBroadcast<IOnGUISystem>();

            root.AddComponent<InitialDomain>();
        }

        private void Update()
        {
            update?.Send(Time.deltaTime);

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
            update = null;
            lateUpdate = null;
            fixedUpdate = null;
            onGUI = null;
            root.Dispose();
        }

    }
}
