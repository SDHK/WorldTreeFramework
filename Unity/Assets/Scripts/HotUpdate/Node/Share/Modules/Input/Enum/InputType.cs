/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/23 16:41

* 描述：

*/

namespace WorldTree
{
	/// <summary>
	/// 控件类型
	/// </summary>
	public enum InputType : byte
	{
		/// <summary>
		/// 0~1区间型，如键盘按键、扳机、踏板
		/// </summary>
		Press,

		/// <summary>
		/// -1~1区间型，如方向盘、单轴摇杆
		/// </summary>
		Axis,

		/// <summary>
		/// XY轴区间型，如摇杆
		/// </summary>
		Axis2,

		/// <summary>
		/// XYZ轴区间型
		/// </summary>
		Axis3,

		/// <summary>
		/// 差值型，如鼠标滚轮、旋钮
		/// </summary>
		Delta,

		/// <summary>
		/// XY轴差值型，如鼠标移动
		/// </summary>
		Delta2,

		/// <summary>
		/// XYZ轴差值型
		/// </summary>
		Delta3,
	}
}