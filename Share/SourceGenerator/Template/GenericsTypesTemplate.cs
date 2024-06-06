/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 11:13

* 描述：

*/
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 泛型类型模板: , T1, T2, T3
	/// </summary>
	public class GenericsTypesTemplate : Dictionary<int, string>
	{
		public GenericsTypesTemplate()
		{
			for (int i = 0; i <= RuleGeneratorSetting.argumentCount; i++) Add(i, GetGenericsTypes(i));
		}
		private string GetGenericsTypes(int index)
		{
			StringBuilder generics = new StringBuilder();
			for (int i = 0; i < index; i++)
			{
				generics.Append($", T{i + 1}");
			}
			return generics.ToString();
		}
	}

}

