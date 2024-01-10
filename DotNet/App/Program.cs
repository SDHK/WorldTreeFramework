using System;
using System.Threading;

namespace WorldTree
{
	internal class Program
    {

        static void Main(string[] args)
        {

            WorldTreeCore Core = new WorldTreeCore();

			Core.Log = Console.WriteLine;
			Core.LogWarning = Console.WriteLine;
			Core.LogError = Console.Error.WriteLine;
			Core.Awake();

			Core.Root.AddComponent(out WorldHeart _, 1000).Run();//启动世界心跳 设定间隔为1000ms

			Core.Root.AddComponent(out DotNetTestNode _);

          
        }
    }
}