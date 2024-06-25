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
	public enum DiagnosticKey
	{
		/// <summary>
		/// 类型命名规范诊断
		/// </summary>
		ClassNaming,

		/// <summary>
		/// 结构体命名规范诊断
		/// </summary>
		StructNaming,

		/// <summary>
		/// 接口命名规范诊断
		/// </summary>
		InterfaceNaming,

		/// <summary>
		/// 委托命名规范诊断
		/// </summary>
		DelegateNaming,

		/// <summary>
		/// 枚举命名规范诊断
		/// </summary>
		EnumNaming,


		/// <summary>
		/// 枚举成员命名规范诊断
		/// </summary>
		EnumMemberNaming,

		/// <summary>
		/// 常量命名规范诊断
		/// </summary>
		ConstNaming,

		/// <summary>
		/// 公共字段命名规范诊断
		/// </summary>
		PublicFieldNaming,

		/// <summary>
		/// 私有字段命名规范诊断
		/// </summary>
		PrivateFieldNaming,

		/// <summary>
		/// 保护字段命名规范诊断
		/// </summary>
		ProtectedFieldNaming,

		/// <summary>
		/// 公共属性命名规范诊断
		/// </summary>
		PublicPropertyNaming,

		/// <summary>
		/// 私有属性命名规范诊断
		/// </summary>
		PrivatePropertyNaming,

		/// <summary>
		/// 保护属性命名规范诊断
		/// </summary>
		ProtectedPropertyNaming,

		/// <summary>
		/// 方法命名规范诊断
		/// </summary>
		MethodNaming,

		/// <summary>
		/// 参数命名规范诊断
		/// </summary>
		ParameterNaming,

		/// <summary>
		/// 泛型参数命名规范诊断
		/// </summary>
		TypeParameterNaming,

		/// <summary>
		/// 局部变量命名规范诊断
		/// </summary>
		LocalVariableNaming,

		/// <summary>
		/// 局部方法命名规范诊断
		/// </summary>
		LocalMethodNaming,



		/// <summary>
		/// 类型字段命名规范诊断
		/// </summary>
		ClassFieldNaming,

		/// <summary>
		/// 类型属性命名规范诊断
		/// </summary>
		ClassPropertyNaming,


		/// <summary>
		/// 实例字段访问诊断
		/// </summary>
		InstanceFieldAccess,

	}
}