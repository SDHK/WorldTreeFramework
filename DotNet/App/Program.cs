using System;

namespace WorldTree
{
    /// <summary>
    /// 世界树框架驱动器，一切从这里开始
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 启动
        /// </summary>
        private static void Main(string[] args)
        {
            WorldLineManager lineManager = new WorldLineManager();
            lineManager.LogType = typeof(WorldLog);
            lineManager.LogLevel = LogLevel.All;

            lineManager.Create(0, typeof(WorldHeart), 1000, typeof(MainWorld));

            //防止程序集被优化掉
            Type ruleType = typeof(MainWorldRule);
            Type nodeType = typeof(DotNetInit);
        }
    }
}