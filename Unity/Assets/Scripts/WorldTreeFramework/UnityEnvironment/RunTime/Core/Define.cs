using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 宏定义
	/// </summary>
	public static class Define
	{
		/// <summary>
		/// 是否编辑器模式
		/// </summary>
#if UNITY_EDITOR
		public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif

	}
}
