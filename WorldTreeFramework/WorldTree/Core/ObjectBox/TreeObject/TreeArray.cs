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
        , IAwake<int>
        , ChildOf<INode>
        , ComponentOf<INode>
    {
        public T[] array;
        public int Length { get { return array.Length; } }

        public T this[int index]
        {
            get { return array[index]; }
            set { array[index] = value; }
        }


        public static implicit operator Array(TreeArray<T> treeArray)
        {
            return treeArray.array;
        }
        public static implicit operator T[](TreeArray<T> treeArray)
        {
            return treeArray.array;
        }
    }

    class TreeArrayAwakeRule<T> : AwakeRule<TreeArray<T>, int>
    {
        public override void OnEvent(TreeArray<T> self, int length)
        {
            self.array = self.Core.ArrayPoolManager.Get<T>(length);
        }
    }


    class TreeArrayRemoveRule<T> : RemoveRule<TreeArray<T>>
    {
        public override void OnEvent(TreeArray<T> self)
        {
            self.Core.ArrayPoolManager.Recycle(self.array);
            self.array = null;
        }
    }

  
}
