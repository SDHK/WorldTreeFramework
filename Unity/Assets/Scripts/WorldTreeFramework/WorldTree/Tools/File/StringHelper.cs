/****************************************

* 作者： 闪电黑客
* 日期： 2024/03/11 19:48:50

* 描述： 字符串工具类
*
*/

namespace WorldTree
{
	public static partial class StringHelper
	{
		/// <summary>
		/// 截取字符串
		/// 获取匹配到的后面内容
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="key">截取关键字</param>
		/// <param name="includeKey">分割的结果里是否包含关键字</param>
		/// <param name="firstMatch">是否使用初始匹配的位置，否则使用末尾匹配的位置</param>
		public static string Intercept(string content, string key, bool includeKey, bool firstMatch = true)
		{
			if (string.IsNullOrEmpty(key))
				return content;

			int startIndex = -1;
			if (firstMatch)
				startIndex = content.IndexOf(key); //返回子字符串第一次出现位置
			else
				startIndex = content.LastIndexOf(key); //返回子字符串最后出现的位置

			// 如果没有找到匹配的关键字
			if (startIndex == -1)
				return content;

			if (includeKey)
				return content.Substring(startIndex);
			else
				return content.Substring(startIndex + key.Length);
		}
	}
}