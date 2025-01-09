/****************************************

* 作者：闪电黑客
* 日期：2024/12/21 18:09

* 描述：

*/
namespace WorldTree
{

	/// <summary>
	/// 输入设备信息
	/// </summary>
	public struct InputInfo
	{
		/// <summary>
		/// 设备类型
		/// </summary>
		public InputDeviceType InputDeviceType;

		/// <summary>
		/// 设备索引号
		/// </summary>
		public byte InputDeviceId;

		/// <summary>
		/// 控件类型
		/// </summary>
		public InputType InputType;

		/// <summary>
		/// 控件码
		/// </summary>
		public byte InputCode;
	}
}