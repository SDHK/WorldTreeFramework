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
    public interface  ITreeValue : INode, ChildOf<INode>
    {

    }


    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public interface ITreeValue<T> : ITreeValue
    {

    }

    public static class TreeValueRule
    {

        /// <summary>
        /// 单向绑定
        /// </summary>
        public static void Bind<TV, TV1>(this TV self, TV1 treeValue)
            where TV : ITreeValue
            where TV1 : TV
        {
            self.Referenced(treeValue);

        }

        /// <summary>
        /// 双向绑定
        /// </summary>
        public static void BindTwoWay<TV, TV1>(this TV self, TV1 treeValue)
            where TV : ITreeValue
            where TV1 : TV
        {

            self.Referenced(treeValue);
            treeValue.Referenced(self);

        }

    }

}
