/****************************************

* 作者： 闪电黑客
* 日期： 2024/3/19 20:30

* 描述：

*/

using System.IO;

namespace WorldTree.AOT
{
	/// <summary>
	/// 资源文件解密流
	/// </summary>
	public class BundleStream : FileStream
	{
		public const byte KEY = 64;

		public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
		{
		}

		public BundleStream(string path, FileMode mode) : base(path, mode)
		{
		}

		public override int Read(byte[] array, int offset, int count)
		{
			var index = base.Read(array, offset, count);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] ^= KEY;
			}
			return index;
		}
	}
}