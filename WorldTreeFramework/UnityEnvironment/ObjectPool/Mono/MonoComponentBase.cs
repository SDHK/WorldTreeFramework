
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 20:39

* 描述： Mono组件基类
* 
* 思考：entity绑定Mono 事件，是否由mono反向调用组件

*/

using UnityEngine;
namespace WorldTree
{
    public abstract class MonoComponentBase : MonoBehaviour
    {
        /// <summary>
        /// 绑定的实体
        /// </summary>
        public Entity entity;
    }
}
