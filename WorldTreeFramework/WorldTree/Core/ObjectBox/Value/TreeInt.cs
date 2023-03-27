/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 树节点Int类型

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 树节点Int类型变化绑定规则接口
    /// </summary>
    public interface ITreeIntChangeBindRule : ISendRule<int> { }

    /// <summary>
    /// 树节点Int类型变化绑定规则
    /// </summary>
    public abstract class TreeIntChangeBindRule<N> : SendRuleBase<ITreeIntChangeBindRule, N, int> where N : class, INode { }

    /// <summary>
    /// 树节点int类型
    /// </summary>
    public class TreeInt : TreeValue
    {
        /// <summary>
        /// 值
        /// </summary>
        private int value;


        public int Value
        {
            get => value;

            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    ruleActuator.SendRule(default(ITreeIntChangeBindRule), value);
                }
            }
        }
    }

    public static class TreeIntStaticRule
    {
        class TreeIntTreeIntChangeBindRule : TreeIntChangeBindRule<TreeInt>
        {
            public override void OnEvent(TreeInt self, int arg1)
            {
                self.Value = arg1;
            }
        }


        //public static void SetRule(this TreeInt self)
        //{

        //}




    }


}
