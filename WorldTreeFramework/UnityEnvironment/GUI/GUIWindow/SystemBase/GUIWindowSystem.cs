/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 13:08

* 描述： 

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{

    public interface IGUIWindowSystem : ISendSystem { }

    /// <summary>
    /// 窗体绘制系统
    /// </summary>
    public abstract class GUIWindowSystem<T> : SystemBase<T, IGUIWindowSystem>, IGUIWindowSystem
       where T : Entity
    {
        public void Invoke(Entity self) => DrawWindow(self as T);

        public abstract void DrawWindow(T self);

    }
}
