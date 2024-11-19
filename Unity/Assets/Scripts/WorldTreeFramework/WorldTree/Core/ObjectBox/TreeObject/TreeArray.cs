/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/13 10:17

* 描述： 一维数组

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 一维数组
    /// </summary>
    public class TreeArray<T> : Node
		, ChildOf<INode>
        , ComponentOf<INode>
        , AsAwake<int>
    {
        /// <summary>
        /// 数组
        /// </summary>
        public T[] arrays;

        /// <summary>
        /// 数组长度
        /// </summary>
        public int Length { get { return arrays.Length; } }

        public T this[int index]
        {
            get { return arrays[index]; }
            set { arrays[index] = value; }
        }


        public static implicit operator Array(TreeArray<T> treeArray)
        {
            return treeArray.arrays;
        }
        public static implicit operator T[](TreeArray<T> treeArray)
        {
            return treeArray.arrays;
        }
    }

    class TreeArrayAwakeRule<T> : AwakeRule<TreeArray<T>, int>
    {
        protected override void Execute(TreeArray<T> self, int length)
        {
            self.arrays = self.Core.PoolGetArray<T>(length);
        }
    }


    class TreeArrayRemoveRule<T> : RemoveRule<TreeArray<T>>
    {
        protected override void Execute(TreeArray<T> self)
        {
            self.Core.PoolRecycle(self.arrays);
            self.arrays = null;
        }
    }


}
