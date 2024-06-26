﻿/******************************

 * Author: 闪电黑客

 * 日期: 2021/12/13 13:40:18

 * 最后日期: 2021/12/15 18:32:23

 * 最后修改: 闪电黑客

 * 描述:  
  
    泛型对象接口
    让IUnit类获得对象池管理

******************************/

namespace WorldTree
{

	/// <summary>
	/// 单位池对象接口
	/// </summary>
	public interface IUnitPoolItem : IUnit
    {
        /// <summary>
        /// 世界树核心
        /// </summary>
        /// <remarks>框架的核心</remarks>
        public WorldTreeCore Core { get; set; }

        /// <summary>
        /// 是否从池获取
        /// </summary>
        public bool IsFromPool { get; set; }
      
    }
}