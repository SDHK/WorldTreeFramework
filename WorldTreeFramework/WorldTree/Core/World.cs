using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    /// <summary>
    /// 世界静态类：当前环境的基础设置
    /// </summary>
    public static class World
    {
        /// <summary>
        /// 打印日志
        /// </summary>
        public static Action<object> Log;
        /// <summary>
        /// 打印警告日志
        /// </summary>
        public static Action<object> LogWarning;
        /// <summary>
        /// 打印错误日志
        /// </summary>
        public static Action<object> LogError;
    }
}
