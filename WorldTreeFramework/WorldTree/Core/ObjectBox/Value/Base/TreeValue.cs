/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

namespace WorldTree
{
    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public abstract class TreeValue : Node, ChildOf<INode>
    {
        ///// <summary>
        ///// 全局法则类型 暂不考虑
        ///// </summary>
        //public Type RuleType;
     

        public RuleActuator m_RuleActuator;

    }




    public static class TreeValueRule
    {

        /// <summary>
        /// 单向绑定
        /// </summary>
        public static void Bind<TV, TV1>(this TV self, TV1 treeValue)
            where TV : TreeValue
            where TV1 : TV
        {
            self.Referenced(treeValue);

        }

        /// <summary>
        /// 双向绑定
        /// </summary>
        public static void BindTwoWay<TV, TV1>(this TV self, TV1 treeValue)
            where TV : TreeValue
            where TV1 : TV
        {

            self.Referenced(treeValue);
            treeValue.Referenced(self);

        }

    }

}
