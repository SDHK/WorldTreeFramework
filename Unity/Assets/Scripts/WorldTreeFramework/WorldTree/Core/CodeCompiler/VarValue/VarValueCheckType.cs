namespace WorldTree
{
	public partial struct VarValue
	{
		/// <summary>
		/// 判断是否为数值类型
		/// </summary>
		public bool IsNumeric()
		{
			return Type == VarType.Long || Type == VarType.Double || Type == VarType.Bool;
		}

		/// <summary>
		/// 判断是否为整数类型
		/// </summary>
		public bool IsInteger()
		{
			return Type == VarType.Long || Type == VarType.Bool;
		}

		/// <summary>
		/// 判断是否为浮点数类型
		/// </summary>
		public bool IsFloat()
		{
			return Type == VarType.Double;
		}
	}
}
