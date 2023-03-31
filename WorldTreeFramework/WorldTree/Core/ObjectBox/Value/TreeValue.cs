/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 泛型树值类型

*/

namespace WorldTree
{
    /// <summary>
    /// 泛型树值类型
    /// </summary>
    public class TreeValue<T> : TreeValue
        where T : struct
    {
        private T value;

        /// <summary>
        /// 值
        /// </summary>
        public virtual T Value
        {
            get => value;

            set
            {
                if (!this.value.Equals(value))
                {
                    this.value = value;

                    if (m_ListenerDictionary != null)
                        foreach (var node in m_ListenerDictionary)
                        {
                            ((TreeValue<T>)node.Value).Value = value;
                        }
                }
            }
        }
    }
}
