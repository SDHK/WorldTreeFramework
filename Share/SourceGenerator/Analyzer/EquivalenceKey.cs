/****************************************

* 作者：闪电黑客
* 日期：2024/6/19 14:45

* 描述：

*/

namespace WorldTree.Analyzer
{
	public enum EquivalenceKey
	{
		/// <summary>
		/// 字段命名修复
		/// </summary>
		FieldNamingFix,

		/// <summary>
		/// 公共字段命名修复
		/// </summary>
		PublicFieldNamingFix,

		/// <summary>
		/// 私有字段命名修复
		/// </summary>
		PrivateFieldNamingFix,

		/// <summary>
		/// 保护字段命名修复
		/// </summary>
		ProtectedFieldNamingFix,

		/// <summary>
		/// 公共属性命名修复
		/// </summary>
		PublicPropertyNamingFix,

		/// <summary>
		/// 私有属性命名修复
		/// </summary>
		PrivatePropertyNamingFix,

		/// <summary>
		/// 保护属性命名修复
		/// </summary>
		ProtectedPropertyNamingFix,

		/// <summary>
		/// 方法命名修复
		/// </summary>
		MethodNamingFix,
	}
}