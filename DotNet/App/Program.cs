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
            WorldLine mainLine = new WorldLine();

            mainLine.LogType = typeof(WorldLog);
            mainLine.LogLevel = LogLevel.All;
            mainLine.Init(typeof(WorldHeart), 1000);

            //启动世界心跳 设定间隔为1000ms
            mainLine.World.AddComponent(out Entry _);

            //防止程序集被优化掉
            Type ruleType = typeof(EntryRule);
            Type nodeType = typeof(DotNetInit);
        }
    }
}