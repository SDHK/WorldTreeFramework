namespace WorldTree
{
	/// <summary>
	/// 输入键
	/// </summary>
	public enum InputKey : ushort//0~65535
	{
		#region 0~1000开始为特别保留

		/// <summary>
		/// 无
		/// </summary>
		None = 0,

		#endregion

		#region 1000开始为鼠标,100以内为系统级别

		/// <summary> 鼠标左键 </summary>
		MouseLeft = 1000,
		/// <summary> 鼠标右键 </summary>
		MouseRight,
		/// <summary> 鼠标中键 </summary>
		MouseMiddle,
		/// <summary> 鼠标按键0 </summary>
		Mouse0 = 1100,
		/// <summary> 鼠标按键1 </summary>
		Mouse1,
		/// <summary> 鼠标按键2 </summary>
		Mouse2,
		/// <summary> 鼠标按键3 </summary>
		Mouse3,
		/// <summary> 鼠标按键4 </summary>
		Mouse4,
		/// <summary> 鼠标按键5 </summary>
		Mouse5,
		/// <summary> 鼠标按键6 </summary>
		Mouse6,

		#endregion


		#region 2000开始为键盘

		#region 系统键:一般为键盘上的特殊键
		/// <summary> Win </summary>
		SystemKeyLeft = 2000,
		/// <summary> Win </summary>
		SystemKeyRight,
		/// <summary> 上下文菜单键 </summary>
		MenuKey,

		/// <summary> 打印屏幕/系统请求 </summary>
		Print,
		/// <summary> 滚动锁定 </summary>
		ScrollLock,
		/// <summary> 暂停/中断 </summary>
		Pause,

		#endregion

		#region 主要功能键
		/// <summary> Ecs </summary>
		Escape = 2100,
		/// <summary> Enter </summary>
		Enter,
		/// <summary> Tab </summary>
		Tab,
		/// <summary> CapsLock </summary>
		CapsLock,
		/// <summary> Shift </summary>
		ShiftLeft,
		/// <summary> Shift </summary>
		ShiftRight,
		/// <summary> Ctrl </summary>
		CtrlLeft,
		/// <summary> Ctrl </summary>
		CtrlRight,
		/// <summary> Alt </summary>
		AltLeft,
		/// <summary> Alt </summary>
		AltRight,
		/// <summary> Backspace </summary>
		Backspace,
		/// <summary> Space </summary>
		Space,

		#endregion

		#region F1 ~ F12

		/// <summary> F1 </summary>
		F1 = 2201,
		/// <summary> F2 </summary>
		F2,
		/// <summary> F3 </summary>
		F3,
		/// <summary> F4 </summary>
		F4,
		/// <summary> F5 </summary>
		F5,
		/// <summary> F6 </summary>
		F6,
		/// <summary> F7 </summary>
		F7,
		/// <summary> F8 </summary>
		F8,
		/// <summary> F9 </summary>
		F9,
		/// <summary> F10 </summary>
		F10,
		/// <summary> F11 </summary>
		F11,
		/// <summary> F12 </summary>
		F12,

		#endregion

		#region 字母		
		/// <summary> A </summary>
		A = 2300,
		/// <summary> B </summary>
		B,
		/// <summary> C </summary>
		C,
		/// <summary> D </summary>
		D,
		/// <summary> E </summary>
		E,
		/// <summary> F </summary>
		F,
		/// <summary> G </summary>
		G,
		/// <summary> H </summary>
		H,
		/// <summary> I </summary>
		I,
		/// <summary> J </summary>
		J,
		/// <summary> K </summary>
		K,
		/// <summary> L </summary>
		L,
		/// <summary> M </summary>
		M,
		/// <summary> N </summary>
		N,
		/// <summary> O </summary>
		O,
		/// <summary> P </summary>
		P,
		/// <summary> Q </summary>
		Q,
		/// <summary> R </summary>
		R,
		/// <summary> S </summary>
		S,
		/// <summary> T </summary>
		T,
		/// <summary> U </summary>
		U,
		/// <summary> V </summary>
		V,
		/// <summary> W </summary>
		W,
		/// <summary> X </summary>
		X,
		/// <summary> Y </summary>
		Y,
		/// <summary> Z </summary>
		Z,
		#endregion

		#region 符号

		/// <summary> `/ ~ </summary>
		BackQuote = 2400,
		/// <summary> - _ </summary>
		Minus,
		/// <summary> = + </summary>
		Equal,
		/// <summary> [ { </summary>
		LeftBracket,
		/// <summary> ] } </summary>
		RightBracket,
		/// <summary> \ | </summary>
		Backslash,
		/// <summary> ; : </summary>
		Semicolon,
		/// <summary> ' " </summary>
		Quote,
		/// <summary> , &lt; </summary>
		Comma,
		/// <summary> . &gt; </summary>
		Period,
		/// <summary> / ? </summary>
		Slash,

		#endregion

		#region 主键盘数字
		/// <summary> 0 </summary>
		Num0 = 2500,
		/// <summary> 1 </summary>
		Num1,
		/// <summary> 2 </summary>
		Num2,
		/// <summary> 3 </summary>
		Num3,
		/// <summary> 4 </summary>
		Num4,
		/// <summary> 5 </summary>
		Num5,
		/// <summary> 6 </summary>
		Num6,
		/// <summary> 7 </summary>
		Num7,
		/// <summary> 8 </summary>
		Num8,
		/// <summary> 9 </summary>
		Num9,
		#endregion

		#region 小键盘的数字与功能键
		/// <summary> 数字0 </summary>
		NumPad0 = 2600,
		/// <summary> 数字1 </summary>
		NumPad1,
		/// <summary> 数字2 </summary>
		NumPad2,
		/// <summary> 数字3 </summary>
		NumPad3,
		/// <summary> 数字4 </summary>
		NumPad4,
		/// <summary> 数字5 </summary>
		NumPad5,
		/// <summary> 数字6 </summary>
		NumPad6,
		/// <summary> 数字7 </summary>
		NumPad7,
		/// <summary> 数字8 </summary>
		NumPad8,
		/// <summary> 数字9 </summary>
		NumPad9,
		/// <summary> 小键盘回车 </summary>
		NumpadEnter,
		/// <summary> 小键盘+ </summary>
		Add,
		/// <summary> 小键盘- </summary>
		Subtract,
		/// <summary> 小键盘* </summary>
		Multiply,
		/// <summary> 小键盘/ </summary>
		Divide,
		/// <summary> 小键盘. </summary>
		Del,
		/// <summary> 小键盘NumLock </summary>
		NumLock,

		#endregion

		#region 小键盘功能键
		/// <summary> Insert </summary>
		Insert = 2700,
		/// <summary> Delete </summary>
		Delete,
		/// <summary> Home </summary>
		Home,
		/// <summary> End </summary>
		End,
		/// <summary> PageUp </summary>
		PageUp,
		/// <summary> PageDown </summary>
		PageDown,
		#endregion

		#region 方向键

		/// <summary> 上 </summary>
		Up = 2800,
		/// <summary> 下 </summary>
		Down,
		/// <summary> 左 </summary>
		Left,
		/// <summary> 右 </summary>
		Right,

		#endregion

		#endregion


		#region 3000开始为手柄,100以内为系统级别



		#endregion



	}
}
