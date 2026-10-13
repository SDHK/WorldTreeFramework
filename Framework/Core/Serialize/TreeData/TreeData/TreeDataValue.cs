/****************************************

* 作者：闪电黑客
* 日期：2024/9/3 12:08

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 树数据数值
	/// </summary>
	public class TreeDataValue : TreeData
	{
		/// <summary>
		/// 数值
		/// </summary>
		public object Value;

		public override string ToString()
		{
			return $"[TreeValue:{this.TypeName}] : {Value}";
		}
	}

}
