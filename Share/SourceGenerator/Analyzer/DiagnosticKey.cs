/****************************************

* 作者：闪电黑客
* 日期：2024/6/19 11:39

* 描述：

*/

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 诊断枚举
	/// </summary>
	[Flags]
	public enum DiagnosticKey
	{
		/// <summary>
		/// 类型命名规范诊断
		/// </summary>
		ClassNaming = 1,

		/// <summary>
		/// 公共字段命名规范诊断
		/// </summary>
		PublicFieldNaming = 1 << 1,

		/// <summary>
		/// 私有字段命名规范诊断
		/// </summary>
		PrivateFieldNaming = 1 << 2,

		/// <summary>
		/// 保护字段命名规范诊断
		/// </summary>
		ProtectedFieldNaming = 1 << 3,

		/// <summary>
		/// 公共属性命名规范诊断
		/// </summary>
		PublicPropertyNaming = 1 << 4,

		/// <summary>
		/// 私有属性命名规范诊断
		/// </summary>
		PrivatePropertyNaming = 1 << 5,

		/// <summary>
		/// 保护属性命名规范诊断
		/// </summary>
		ProtectedPropertyNaming = 1 << 6,

		/// <summary>
		/// 方法命名规范诊断
		/// </summary>
		MethodNaming = 1 << 7,

		/// <summary>
		/// 类型字段命名规范诊断
		/// </summary>
		ClassFieldNaming = 1 << 8,
	}
}