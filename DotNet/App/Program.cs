using CommandLine;
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
            Options options = new();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Console.WriteLine(e.ExceptionObject.ToString());
            // 命令行参数接收
            Parser.Default.ParseArguments<Options>(System.Environment.GetCommandLineArgs())
                       .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                       .WithParsed((o) => options = o);

            WorldLineManager lineManager = new();
            lineManager.Options = options;
            lineManager.LogType = typeof(WorldLog);

            lineManager.Create(0, typeof(WorldHeart), 1000, typeof(MainWorld));

            //防止程序集被优化掉
            Type ruleType = typeof(MainWorldRule);
            Type nodeType = typeof(DotNetInit);
        }
    }
}