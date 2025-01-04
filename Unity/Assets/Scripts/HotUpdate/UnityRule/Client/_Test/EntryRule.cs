/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 15:06

* 描述：

*/
namespace WorldTree
{
	public static partial class EntryRule
	{
		class Add : AddRule<Entry>
		{
			protected override void Execute(Entry self)
			{
				self.Log("入口！！！");
				self.AddComponent(out InitialDomain _);
			}
		}
	}
}
