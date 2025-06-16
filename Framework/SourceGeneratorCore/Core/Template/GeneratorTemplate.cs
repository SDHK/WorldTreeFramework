/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 11:12

* 描述：

*/
using System.Collections.Generic;
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 生成器模板
	/// </summary>
	public static partial class GeneratorTemplate
	{
		private static Dictionary<int, string> genericsTypes;
		private static Dictionary<int, string> genericsTypesAfter;
		private static Dictionary<int, string> genericsTypesAngle;
		private static Dictionary<int, string> genericsParameter;
		private static Dictionary<int, string> genericsTypeParameter;
		private static Dictionary<int, string> genericsRefParameter;
		private static Dictionary<int, string> genericsRefTypeParameter;


		private static Dictionary<int, string> genericsInTypes;
		private static Dictionary<int, string> genericsInTypesAngle;



		/// <summary>
		/// 泛型类型模板: , in T1, in T2, in T3
		/// </summary>
		public static Dictionary<int, string> GenericsInTypes
		{
			get
			{
				if (genericsInTypes == null)
				{
					genericsInTypes = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							generics.Append($", in T{i + 1}");
						}
						genericsInTypes.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsInTypes;
			}
		}



		/// <summary>
		/// 泛型类型角括号模板: &lt;in T1, in T2, in T3 &gt;
		/// </summary>
		public static Dictionary<int, string> GenericsInTypesAngle
		{
			get
			{
				if (genericsInTypesAngle == null)
				{
					genericsInTypesAngle = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							if (i == 0) generics.Append("<");
							generics.Append($"in T{i + 1}");
							if (i == index - 1)
							{
								generics.Append(">");
							}
							else
							{
								generics.Append(", ");
							}
						}
						genericsInTypesAngle.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsInTypesAngle;
			}
		}




		/// <summary>
		/// 泛型类型模板: , T1, T2, T3
		/// </summary>
		public static Dictionary<int, string> GenericsTypes
		{
			get
			{
				if (genericsTypes == null)
				{
					genericsTypes = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							generics.Append($", T{i + 1}");
						}
						genericsTypes.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsTypes;
			}
		}



		/// <summary>
		/// 泛型类型模板: T1, T2, T3,
		/// </summary>
		public static Dictionary<int, string> GenericsTypesAfter
		{
			get
			{
				if (genericsTypesAfter == null)
				{
					genericsTypesAfter = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();

					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							generics.Append($"T{i + 1}, ");
						}
						genericsTypesAfter.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsTypesAfter;
			}
		}

		/// <summary>
		/// 泛型类型角括号模板: &lt; T1, T2, T3 &gt;
		/// </summary>
		public static Dictionary<int, string> GenericsTypesAngle
		{
			get
			{
				if (genericsTypesAngle == null)
				{
					genericsTypesAngle = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							if (i == 0) generics.Append("<");
							generics.Append($"T{i + 1}");
							if (i == index - 1)
							{
								generics.Append(">");
							}
							else
							{
								generics.Append(", ");
							}
						}
						genericsTypesAngle.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsTypesAngle;
			}
		}

		/// <summary>
		/// 泛型参数模板: , arg1, arg2, arg3
		/// </summary>
		public static Dictionary<int, string> GenericsParameter
		{
			get
			{
				if (genericsParameter == null)
				{
					genericsParameter = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							generics.Append($", arg{i + 1}");
						}
						genericsParameter.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsParameter;
			}
		}
		/// <summary>
		/// 泛型类型参数模板: , T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		public static Dictionary<int, string> GenericsTypeParameter
		{
			get
			{
				if (genericsTypeParameter == null)
				{
					genericsTypeParameter = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							generics.Append($", T{i + 1} arg{i + 1}");
						}
						genericsTypeParameter.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsTypeParameter;
			}
		}

		/// <summary>
		/// 泛型参数模板: , ref arg1, ref arg2, ref arg3
		/// </summary>
		public static Dictionary<int, string> GenericsRefParameter
		{
			get
			{
				if (genericsRefParameter == null)
				{
					genericsRefParameter = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							generics.Append($", ref arg{i + 1}");
						}
						genericsRefParameter.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsRefParameter;
			}
		}

		/// <summary>
		/// 泛型类型参数模板: , ref T1 arg1, ref T2 arg2, ref T3 arg3
		/// </summary>
		public static Dictionary<int, string> GenericsRefTypeParameter
		{
			get
			{
				if (genericsRefTypeParameter == null)
				{
					genericsRefTypeParameter = new Dictionary<int, string>();
					StringBuilder generics = new StringBuilder();
					for (int index = 0; index <= ProjectGeneratorSetting.ArgumentCount; index++)
					{
						for (int i = 0; i < index; i++)
						{
							generics.Append($", ref T{i + 1} arg{i + 1}");
						}
						genericsRefTypeParameter.Add(index, generics.ToString());
						generics.Clear();
					}
				}
				return genericsRefTypeParameter;
			}
		}


	}
}

