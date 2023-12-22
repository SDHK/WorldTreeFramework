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

			Core.Root.AddComponent(out DotNetTestNode _);

            while (true)
            {
                Thread.Sleep(100);
				Core.Update(100);
			}
        }
    }
}