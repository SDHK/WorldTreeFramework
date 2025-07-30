/****************************************

* 作者： 闪电黑客
* 日期： 2025/7/17 20:48

* 描述： 

*/
namespace WorldTree
{
	public static class WindowManagerRule
	{
		/// <summary>
		/// 打开窗口
		/// </summary>
		public static void Open<VM>(this WindowManager windowManager, VM vm)
			where VM : class, IView
		{
			//vm.ViewType


		}
	}
}
