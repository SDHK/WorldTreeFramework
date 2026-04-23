using CommandLine;
using System;

namespace WorldTree.Server
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

            WorldTreeCore worldTree = new();
            worldTree.SetOptions(options);
            worldTree.SetLog<WorldLog>();

            var line = worldTree.Create(0, typeof(MainWorld), typeof(WorldHeart), 1000);

            //防止程序集被优化掉
            Type ruleType = typeof(MainWorldRule);
            Type nodeType = typeof(MainWorld);
            Type dotNetInit = typeof(DotNetInit);
        }
    }
}