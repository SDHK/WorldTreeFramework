/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 11:13

* 描述：

*/
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 泛型类型参数模板: , T1 arg1, T2 arg2, T3 arg3
	/// </summary>
	public class GenericsTypeParameterTemplate : Dictionary<int, string>
	{
		public GenericsTypeParameterTemplate()
		{
			for (int i = 0; i <= RuleGeneratorSetting.argumentCount; i++)Add(i, GetGenericsTypeParameter(i));
		}

		private string GetGenericsTypeParameter(int index)
		{
			StringBuilder GenericTypeParameter = new StringBuilder();
			for (int i = 0; i < index; i++)
			{
				GenericTypeParameter.Append($", T{i + 1} arg{i + 1}");
			}
			return GenericTypeParameter.ToString();
		}
	}

}

