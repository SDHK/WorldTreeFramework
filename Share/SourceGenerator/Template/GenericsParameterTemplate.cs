/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 11:13

* 描述：

*/
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 泛型参数模板: , arg1, arg2, arg3
	/// </summary>
	public class GenericsParameterTemplate : Dictionary<int, string>
	{
		public GenericsParameterTemplate()
		{
			for (int i = 0; i <= RuleGeneratorSetting.argumentCount; i++)Add(i, GetGenericsParameter(i));
		}

		private string GetGenericsParameter(int index)
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

