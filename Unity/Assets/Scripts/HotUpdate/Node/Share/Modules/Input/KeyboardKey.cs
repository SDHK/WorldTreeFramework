namespace WorldTree
{
	/// <summary>
	/// 键盘
	/// </summary>
	public enum KeyboardKey : byte
	{
		#region 功能键
		/// <summary> Ecs </summary>
		Escape,
		/// <summary> F1 </summary>
		F1,
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
		/// <summary> Tab </summary>
		Tab,
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
		/// <summary> Enter </summary>
		Enter,
		/// <summary> Space </summary>
		Space,
		/// <summary> Backspace </summary>
		Backspace,
		#endregion

		#region 小键盘的数字与功能键
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

		/// <summary> 数字0 </summary>
		NumPad0,
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

		#endregion

		#region 主键盘数字
		/// <summary> 0 </summary>
		Num0,
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

		#region 方向键
		/// <summary> 上 </summary>
		Up,
		/// <summary> 下 </summary>
		Down,
		/// <summary> 左 </summary>
		Left,
		/// <summary> 右 </summary>
		Right,
		#endregion

		#region 符号
		/// <summary> `/ ~ </summary>
		BackQuote,
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

		#region 字母
		/// <summary> A </summary>
		A,
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

		#region 系统键
		/// <summary> Win </summary>
		SystemKeyLeft,
		/// <summary> Win </summary>
		SystemKeyRight,
		/// <summary> CapsLock </summary>
		CapsLock,
		/// <summary> 上下文菜单键 </summary>
		MenuKey,
		/// <summary> 打印屏幕/系统请求 </summary>
		Print,
		/// <summary> 滚动锁定 </summary>
		ScrollLock,
		/// <summary> 暂停/中断 </summary>
		Pause,

		#endregion

		#region 小功能键
		/// <summary> Insert </summary>
		Insert,
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
	}

}
