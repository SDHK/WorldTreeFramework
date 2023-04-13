/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/13 19:57

* 描述： 二维矩阵

*/

namespace WorldTree
{
    /// <summary>
    /// 二维矩阵
    /// </summary>
    public class TreeMatrix2<T> : Node, IAwake<int, int>, ChildOf<INode>
    {
        /// <summary>
        /// x轴长度
        /// </summary>
        public int xLength;
        /// <summary>
        /// y轴长度
        /// </summary>
        public int yLength;

        public TreeArray<INode> m_Array;



        public T this[int x, int y]
        {
            get
            {
                if (x < xLength && y < yLength)
                {
                    m_Array[x] ??= this.AddChild(out TreeArray<T> _, yLength);
                    return ((TreeArray<T>)m_Array[x])[y];
                }
                else
                {
                    World.LogError("下标溢出");
                    return default(T);
                }
            }
            set
            {
                if (x < xLength && y < yLength)
                {
                    m_Array[x] ??= this.AddChild(out TreeArray<T> _, yLength);
                    ((TreeArray<T>)m_Array[x])[y] = value;
                }
                else
                {
                    World.LogError("下标溢出");
                }
            }
        }
    }
    public class TreeMatrix2AwakeRule<T> : AwakeRule<TreeMatrix2<T>, int, int>
    {
        public override void OnEvent(TreeMatrix2<T> self, int xLength, int yLength)
        {
            self.xLength = xLength;
            self.yLength = yLength;
            self.AddChild(out self.m_Array, xLength);
        }
    }

    class TreeMatrix2RemoveRule<T> : RemoveRule<TreeMatrix2<T>>
    {
        public override void OnEvent(TreeMatrix2<T> self)
        {
            self.xLength = 0;
            self.yLength = 0;
            self.m_Array = null;
        }
    }

    public class TreeMatrix2Rule
    {


    }
}
