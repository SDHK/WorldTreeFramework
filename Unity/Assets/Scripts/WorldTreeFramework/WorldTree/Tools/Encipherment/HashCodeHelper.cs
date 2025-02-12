/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

* 描述：

*/
using System;
using System.Security.Cryptography;
using System.Text;

namespace WorldTree
{
	/// <summary>
	/// 哈希码计算
	/// </summary>
	public static class HashCodeHelper
	{

		/// <summary>
		/// SHA256
		/// </summary>
		private readonly static SHA256CryptoServiceProvider sha256 = new();

		/// <summary>
		/// 获取64位的哈希码
		/// </summary>
		public static long GetHash64(this string str)
		{
			return BitConverter.ToInt64(sha256.ComputeHash(Encoding.UTF8.GetBytes(str)), 0);
		}


		/// <summary>
		/// 快速获取32位的哈希码
		/// </summary>
		public static int GetFNV1aHash32(this string str)
		{
			const int fnvPrime = 0x01000193;
			const int fnvOffsetBasis = unchecked((int)0x811C9DC5);
			int hash = fnvOffsetBasis;
			foreach (char c in str)
			{
				hash ^= c;
				hash *= fnvPrime;
			}
			return hash;
		}
	}
}
