/****************************************

* 作者：闪电黑客
* 日期：2024/12/23 16:41

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
		/// 0~1区间型：键盘按键、扳机、踏板
		/// </summary>
		Press = 0,

		/// <summary>
		/// X轴向量型：方向盘、单轴摇杆
		/// </summary>
		Vector = 10,

		/// <summary>
		/// XY向量型：摇杆
		/// </summary>
		Vector2,

		/// <summary>
		/// XYZ向量型
		/// </summary>
		Vector3,


		/// <summary>
		/// X轴坐标型
		/// </summary>
		Position = 20,
		/// <summary>
		/// XY轴坐标型：鼠标坐标
		/// </summary>
		Position2,

		/// <summary>
		/// XYZ轴坐标型：动捕追踪器
		/// </summary>
		Position3,


		/// <summary>
		/// X轴差值型：鼠标滚轮、旋钮
		/// </summary>
		Delta = 30,

		/// <summary>
		/// XY轴差值型：特别的鼠标滚轮
		/// </summary>
		Delta2,

		/// <summary>
		/// XYZ轴差值型：陀螺仪
		/// </summary>
		Delta3,
	}
}