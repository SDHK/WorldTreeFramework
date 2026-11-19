/****************************************

* 作者： 闪电黑客
* 日期： 2026/11/19

* 描述： 视图字段挂载。生成器把挂上、写入字段、打开收成一次调用。

*/

namespace WorldTree
{
	/// <summary>
	/// 视图字段挂载。
	/// </summary>
	public static class ViewFieldHelp
	{
		/// <summary>
		/// 按名字挂上子视图，写进字段，返回打开任务。
		/// </summary>
		/// <remarks>
		/// 不能写成 async。async 方法不能有 out，字段要在返回任务之前写好，调用处再 await。
		/// 名字用字段名，交给字符串唤醒写成 ViewName。
		/// </remarks>
		public static TreeTask AddView<V>(View self, out NodeRef<V> field, string name)
			where V : View
		{
			self.AddView(out V view, name);
			field = view;
			return view.OnOpen();
		}
	}
}
