/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 11:14

* 描述：

*/
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 泛型类型角括号模板: &lt; T1, T2, T3 &gt;
	/// </summary>
	public class GenericsTypesAngleTemplate : Dictionary<int, string>
	{
		public GenericsTypesAngleTemplate()
		{
			for (int i = 0; i <= RuleGeneratorSetting.argumentCount; i++) Add(i, GetGenericsTypesAngle(i));
		}

		private string GetGenericsTypesAngle(int index)
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
	}
}

