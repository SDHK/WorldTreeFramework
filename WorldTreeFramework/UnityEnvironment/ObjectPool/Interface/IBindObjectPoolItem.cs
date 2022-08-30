
/******************************

 * Author: 闪电黑客

 * 日期: 2021/12/14 03:56:08

 * 最后日期: 2021/12/15 18:09:10

 * 最后修改: 闪电黑客

 * 描述:  
    
    泛型绑定对象接口，继承 IObjectPoolItem

    实现类持有控制一个GameObject

******************************/



using UnityEngine;
namespace WorldTree
{
    /// <summary>
    /// 泛型绑定对象接口
    /// </summary>
    public interface IBindObjectPoolItem : IUnitPoolItem
    {
        /// <summary>
        /// 绑定的GameObject
        /// </summary>
        GameObject bindGameObject { get; set; }
    }
}

