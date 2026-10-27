/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： Tag 到 Source 的注册项信息。

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// Tag 到 Source 的注册项信息
	/// </summary>
	public readonly struct FileSourceInfo
	{
		/// <summary>
		/// 路由 Tag；空字符串表示默认 Tag
		/// </summary>
		public string Tag { get; }

		/// <summary>
		/// 文件源
		/// </summary>
		public IFileSource Source { get; }

		/// <summary>
		/// 兜底链中的优先级，越小越优先（0 最高）
		/// </summary>
		public int Priority { get; }

		public FileSourceInfo(string tag, IFileSource source, int priority = 0)
		{
			Tag = tag ?? string.Empty;
			Source = source ?? throw new ArgumentNullException(nameof(source));
			Priority = priority;
		}

		public override string ToString()
		{
			return $"{Tag} -> {Source.Name} ({Priority})";
		}
	}
}
