/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/

//using Ionic.Crc;
using System;
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// CRC32校验工具类
	/// </summary>
	public static partial class CRC32Helper
	{
		/// <summary>
		/// 获取CRC32
		/// </summary>
		public static uint StreamCRC32(Stream stream)
		{
			CRC32Algorithm hash = new CRC32Algorithm();
			byte[] hashBytes = hash.ComputeHash(stream);
			return BitConverter.ToUInt32(hashBytes, 0);
		}
	}
}