/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 泛型树值类型

*/

namespace WorldTree
{

    /// <summary>
    /// 数值监听法则接口
    /// </summary>
    public interface ITreeValueRule<T1> : ISendRule<T1> { }
    /// <summary>
    /// 数值监听法则
    /// </summary>
    public abstract class TreeValueRule<N, T1> : SendRuleBase<ITreeValueRule<T1>, N, T1> where N : class, INode { }


    /// <summary>
    /// 泛型树值类型
    /// </summary>
    public class TreeValue<T> : ITreeValue
        where T : struct
    {
        private T value;

        public IRuleActuator<ISendRule<T>> actuator;

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

                    if (m_ReferencedsBy != null)
                        foreach (var node in m_ReferencedsBy)
                        {
                            ((TreeValue<T>)node.Value).Value = value;
                        }
                }
            }
        }
    }

    class TreeValueTreeValueRule<T> : TreeValueRule<TreeValue<T>, T>
        where T : struct
    {
        public override void OnEvent(TreeValue<T> self, T arg1)
        {

        }
    }
}
