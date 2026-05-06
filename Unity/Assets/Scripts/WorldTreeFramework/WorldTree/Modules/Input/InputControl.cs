/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 输入控件
	/// </summary>
	public class InputControl : Node
		, AsRule<Awake>
	{
		/// <summary>
		/// 输入类型
		/// </summary>
		public InputType InputType;

		/// <summary>
		/// 输入检测器
		/// </summary>
		public InputBind InputAdapter;

		/// <summary>
		/// 行为事件
		/// </summary>
		public long InputRuleType;
	}

}