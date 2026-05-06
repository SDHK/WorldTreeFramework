namespace WorldTree
{
	/// <summary>
	/// 按钮
	/// </summary>
	public class Button : View<ButtonBind>
	{
		/// <summary>
		/// 点击事件
		/// </summary>
		public RuleUnicast<ISendRule> OnClick;
	}

	/// <summary>
	/// 按钮绑定
	/// </summary>
	public class ButtonBind : ViewBind { }


}
