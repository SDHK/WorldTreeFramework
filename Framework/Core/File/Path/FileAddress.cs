/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件地址：channel/group[/tag]:key。

*/

namespace WorldTree
{
	/// <summary>
	/// 文件地址
	/// </summary>
	/// <remarks>
	/// channel/group[/tag]:key。
	/// group 不能为空；tag 可空；key 不能为空，可包含 '/' 和 ':'。
	/// </remarks>
	public readonly struct FileAddress
	{
		/// <summary>
		/// 文件通道
		/// </summary>
		public FileChannel Channel { get; }

		/// <summary>
		/// 内容分组
		/// </summary>
		public string Group { get; }

		/// <summary>
		/// 逻辑标签（版本/别名/备用路径）
		/// </summary>
		public string Tag { get; }

		/// <summary>
		/// 文件键/路径
		/// </summary>
		public string Key { get; }

		public FileAddress(FileChannel channel, string group, string tag, string key)
		{
			if (string.IsNullOrEmpty(group))
				throw new FileException(FileError.InvalidPath, group, "Group 不能为空");
			if (string.IsNullOrEmpty(key))
				throw new FileException(FileError.InvalidPath, key, "Key 不能为空");

			Channel = channel;
			Group = group;
			Tag = tag ?? string.Empty;
			Key = key;
		}

		/// <summary>
		/// 解析 channel/group[/tag]:key
		/// </summary>
		public static FileAddress Parse(string address)
		{
			if (!TryParse(address, out FileAddress result))
				throw new FileException(FileError.InvalidPath, address, "地址格式错误");

			return result;
		}

		/// <summary>
		/// 尝试解析 channel/group[/tag]:key
		/// </summary>
		public static bool TryParse(string address, out FileAddress result)
		{
			result = default;
			if (string.IsNullOrEmpty(address)) return false;

			int colon = address.IndexOf(':');
			if (colon <= 0 || colon == address.Length - 1) return false;

			string prefix = address.Substring(0, colon);
			string key = address.Substring(colon + 1);

			string[] parts = prefix.Split('/');
			if (parts.Length < 2 || parts.Length > 3) return false;

			string channelText = parts[0];
			string group = parts[1];
			string tag = parts.Length == 3 ? parts[2] : string.Empty;

			if (string.IsNullOrEmpty(channelText) || string.IsNullOrEmpty(group)) return false;
			if (!TryParseChannel(channelText, out FileChannel channel)) return false;

			result = new FileAddress(channel, group, tag, key);
			return true;
		}

		/// <summary>
		/// 解析通道操作码
		/// </summary>
		public static bool TryParseChannel(string value, out FileChannel channel)
		{
			switch (value?.ToLowerInvariant())
			{
				case "pack":
					channel = FileChannel.Pack;
					return true;
				case "user":
					channel = FileChannel.User;
					return true;
				case "temp":
					channel = FileChannel.Temp;
					return true;
				default:
					channel = default;
					return false;
			}
		}

		public override string ToString()
		{
			string channel = Channel.ToString().ToLowerInvariant();
			if (Tag.Length == 0) return $"{channel}/{Group}:{Key}";
			return $"{channel}/{Group}/{Tag}:{Key}";
		}
	}
}
