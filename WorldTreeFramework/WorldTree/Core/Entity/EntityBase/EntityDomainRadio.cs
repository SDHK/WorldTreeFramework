
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/8 18:11

* 描述： 实体收音机

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public abstract partial class Entity
    {
       
        public SystemBroadcast<T> GetSystemDomainBroadcast<T>()
        where  T : ISystem
        {
            return this.AddComponent<SystemBroadcast<T>>();
        }

    }
}
