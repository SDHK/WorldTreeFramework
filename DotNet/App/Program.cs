using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Microsoft.VisualBasic;

namespace WorldTree
{
    internal class Program
    {

        static void Main(string[] args)
        {
            World.Log = Console.WriteLine;
            World.LogWarning = Console.WriteLine;
            World.LogError = Console.Error.WriteLine;
            WorldTreeCore Core = new WorldTreeCore();

            GlobalRuleActuator<IEnableRule> enable;
            GlobalRuleActuator<IDisableRule> disable;
            GlobalRuleActuator<IUpdateRule> update;

            Core.GetOrNewGlobalRuleActuator(out enable);
            Core.GetOrNewGlobalRuleActuator(out update);
            Core.GetOrNewGlobalRuleActuator(out disable);

            Core.Root.AddComponent(out DotNetTestNode _);


            while (true)
            {
                Thread.Sleep(100);
                enable?.Send();
                update?.Send(0.1f);
                disable?.Send();
            }
        }
    }
}