/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 整数编码
	/// </summary>
	public static class ValueTypeCode
	{
		/// <summary>
		/// 最大单字节值
		/// </summary>
		public const byte MAX_SINGLE_VALUE = 127;
		/// <summary>
		/// 最小单字节值
		/// </summary>
		public const sbyte MIN_SINGLE_VALUE = -120;

		/// <summary>
		/// 字节
		/// </summary>
		public const sbyte BYTE = -121;
		/// <summary>
		/// 有符号字节
		/// </summary>
		public const sbyte SBYTE = -122;
		/// <summary>
		/// 无符号短整数
		/// </summary>
		public const sbyte UINT16 = -123;
		/// <summary>
		/// 有符号短整数
		/// </summary>
		public const sbyte INT16 = -124;
		/// <summary>
		/// 无符号整数
		/// </summary>
		public const sbyte UINT32 = -125;
		/// <summary>
		/// 有符号整数
		/// </summary>
		public const sbyte INT32 = -126;
		/// <summary>
		/// 无符号长整数
		/// </summary>
		public const sbyte UINT64 = -127;
		/// <summary>
		/// 有符号长整数
		/// </summary>
		public const sbyte INT64 = -128;
	}
}
