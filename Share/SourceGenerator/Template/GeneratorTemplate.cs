/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 11:12

* 描述：

*/
namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 生成器模板
	/// </summary>
	public static partial class GeneratorTemplate
	{
		private static GenericsTypesTemplate genericsTypes = new();
		private static GenericsTypesAfterTemplate genericsTypesAfter = new();
		private static GenericsParameterTemplate genericsParameter = new();
		private static GenericsTypeParameterTemplate genericsTypeParameter = new();
		private static GenericsTypesAngleTemplate genericsTypesAngle = new();

		/// <summary>
		/// 泛型类型模板: , T1, T2, T3
		/// </summary>
		public static GenericsTypesTemplate GenericsTypes => genericsTypes ??= new();
		/// <summary>
		/// 泛型类型模板: T1, T2, T3,
		/// </summary>
		public static GenericsTypesAfterTemplate GenericsTypesAfter => genericsTypesAfter ??= new();
		/// <summary>
		/// 泛型参数模板: , arg1, arg2, arg3
		/// </summary>
		public static GenericsParameterTemplate GenericsParameter => genericsParameter ??= new();
		/// <summary>
		/// 泛型类型参数模板: , T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		public static GenericsTypeParameterTemplate GenericsTypeParameter => genericsTypeParameter ??= new();
		/// <summary>
		/// 泛型类型角括号模板: &lt; T1, T2, T3 &gt;
		/// </summary>
		public static GenericsTypesAngleTemplate GenericsTypesAngle => genericsTypesAngle ??= new();
	}
}

