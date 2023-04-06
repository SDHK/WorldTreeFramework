/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/6 11:45

* 描述： 向量

*/

namespace WorldTree
{
    public struct Vector3Float
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator Vector3Float(float i)
        {
            return new Vector3Float { x = i };
        }

    }

}
