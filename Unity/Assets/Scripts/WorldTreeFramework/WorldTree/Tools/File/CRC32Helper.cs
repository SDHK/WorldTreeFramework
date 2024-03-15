/****************************************

* 作者： 闪电黑客
* 日期： 2024/03/11 20:52:18

* 描述： CRC32校验工具类
*
*/
using Ionic.Crc;
using System.IO;

namespace WorldTree
{
	public static partial class CRC32Helper
	{
		/// <summary>
		/// 获取CRC32
		/// </summary>
		public static int StreamCRC32(Stream stream)
		{
			CRC32 crc32 = new CRC32();
			return crc32.GetCrc32(stream);
		}

	}
}