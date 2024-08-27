/****************************************

* 作者：闪电黑客
* 日期：2024/2/21 11:39

* 描述：游戏启动入口

*/

namespace WorldTree.AOT
{
	/// <summary>
	/// 宏定义
	/// </summary>
	public static class Define
	{
		/// <summary>
		/// 是否编辑器模式
		/// </summary>
#if UNITY_EDITOR
		public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif

	}
}