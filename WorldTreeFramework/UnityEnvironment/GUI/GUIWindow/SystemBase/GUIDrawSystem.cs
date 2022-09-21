/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 13:08

* 描述： GUI绘制系统

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{

    public interface IGUIDrawSystem : ISendSystem { }

    /// <summary>
    /// GUI绘制系统
    /// </summary>
    public abstract class GUIDrawSystem<T> : SystemBase<T, IGUIDrawSystem>, IGUIDrawSystem
       where T : Entity
    {
        public void Invoke(Entity self) => DrawGUI(self as T);

        public abstract void DrawGUI(T self);

    }
}
