/****************************************

* 作者： 闪电黑客
* 日期： 2026/11/15

* 描述： 视图绘制壳基类。

*/

namespace WorldTree
{
	/// <summary>
	/// 视图绘制壳。继承加载壳，放在 <see cref="View.Draw"/> 上，不是节点，没有法则。
	/// 进加载池的由 LoadPool 借还。不进池的路径留空，用 NewUnit 创建，释放后直接作废。
	/// 平台壳以 Draw 开头，继承这个类。
	/// </summary>
	public abstract class ViewDraw : LoadUnit, IViewDraw
	{
		/// <summary>
		/// 是否激活显示
		/// </summary>
		public abstract bool IsActive { get; }

		/// <summary>
		/// 设置激活显示
		/// </summary>
		public abstract void SetActive(bool value);
	}

	/// <summary>
	/// 带平台对象类型的绘制壳。T 是这只壳拿着的对象，用 <see cref="Coat"/> 读写，存在 <see cref="LoadUnit.Target"/> 上。
	/// </summary>
	public abstract class ViewDraw<T> : ViewDraw
		where T : class
	{
		/// <summary>
		/// 这只壳拿着的平台对象。
		/// </summary>
		public T Coat
		{
			get => Target as T;
			set => Target = value;
		}
	}
}
