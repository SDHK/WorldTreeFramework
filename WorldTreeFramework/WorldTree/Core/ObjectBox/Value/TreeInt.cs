/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 树节点Int类型

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public abstract class TreeValue : Node, ChildOfNode
    {

    }

    /// <summary>
    /// 树节点int类型
    /// </summary>
    public class TreeInt : TreeValue
    {
        /// <summary>
        /// 值
        /// </summary>
        private int value;

        /// <summary>
        /// 法则执行器
        /// </summary>
        public RuleActuator ruleActuator;

        /// <summary>
        /// 全局法则类型
        /// </summary>
        public Type RuleType;


        public int Value
        {
            get => value;

            set
            {

                if (this.value != value)
                {
                    this.value = value;
                }
            }
        }
    }

    public static class TreeIntStaticRule
    {
        //public static void SetRule(this TreeInt self)
        //{

        //}
    }


}
