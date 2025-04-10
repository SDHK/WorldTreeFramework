/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 15:39

* 描述：

*/
namespace WorldTree
{
	public static class MainWorldRule
	{
		class Add : AddRule<MainWorld>
		{
			protected override void Execute(MainWorld self)
			{
				self.Log("入口！！！");
				self.AddComponent(out DotNetInit _);
			}
		}
	}
}