using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	public class UtcTimeTest:Node
		,ComponentOf<InitialDomain>
		,AsAwake
		,AsGuiUpdateTime
	{
		public DateTime StartTime;

		/// <summary>
		/// 每分钟漂移更新一次
		/// </summary>
		public TimeSpan offset;

		/// <summary>
		/// 每分钟时间戳
		/// </summary>
		public DateTime OneTime;

		/// <summary>
		/// 偏移后的累计时间
		/// </summary>
		public TimeSpan difference;
	}
}
