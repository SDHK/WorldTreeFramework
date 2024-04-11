/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 16:01

* 描述：法则生成器帮助类

*/

using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 法则生成器帮助类
	/// </summary>
	internal static class RuleGeneratorHelper
	{
		/// <summary>
		/// 获取泛型类型带尖括号:《T1, T2, T3》
		/// </summary>
		public static string GetGenericsAngle(int index)
		{
			StringBuilder genericsAngle = new StringBuilder();
			for (int i = 0; i < index; i++)
			{
				if (i == 0) genericsAngle.Append("<");
				genericsAngle.Append($"T{i + 1}");
				if (i == index - 1)
				{
					genericsAngle.Append(">");
				}
				else
				{
					genericsAngle.Append(", ");
				}
			}
			return genericsAngle.ToString();
		}

		/// <summary>
		/// 获取泛型类型: , T1, T2, T3
		/// </summary>
		public static string GetGenerics(int index, bool isAfter = false)
		{
			StringBuilder generics = new StringBuilder();
			for (int i = 0; i < index; i++)
			{
				if (isAfter)
				{
					generics.Append($"T{i + 1}, ");
				}
				else
				{
					generics.Append($", T{i + 1}");
				}
			}
			return generics.ToString();
		}

		/// <summary>
		/// 获取泛型类型加参数: , T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		public static string GetGenericTypeParameter(int index)
		{
			StringBuilder GenericTypeParameter = new StringBuilder();
			for (int i = 0; i < index; i++)
			{
				GenericTypeParameter.Append($", T{i + 1} arg{i + 1}");
			}
			return GenericTypeParameter.ToString();
		}

		/// <summary>
		/// 获取泛型参数: , arg1, arg2, arg3
		/// </summary>
		public static string GetGenericParameter(int index)
		{
			StringBuilder GenericParameter = new StringBuilder();
			for (int i = 0; i < index; i++)
			{
				GenericParameter.Append($", arg{i + 1}");
			}
			return GenericParameter.ToString();
		}
	}
}