/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件错误码。

*/

namespace WorldTree
{
	/// <summary>
	/// 文件错误码
	/// </summary>
	public enum FileError
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,

		/// <summary>
		/// 不存在
		/// </summary>
		NotFound,

		/// <summary>
		/// 不支持
		/// </summary>
		NotSupported,

		/// <summary>
		/// 拒绝访问
		/// </summary>
		AccessDenied,

		/// <summary>
		/// 已存在
		/// </summary>
		AlreadyExists,

		/// <summary>
		/// 路径不合法
		/// </summary>
		InvalidPath,

		/// <summary>
		/// IO 错误
		/// </summary>
		IOError,

		/// <summary>
		/// 超时
		/// </summary>
		Timeout,
	}
}
