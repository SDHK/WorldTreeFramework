using Avalonia.Controls;
using System.Diagnostics;

namespace WorldTree
{
	public static partial class MainWorldRule
	{
		[NodeRule(nameof(AwakeRule<MainWorld, Window>))]
		private static void OnAwake(this MainWorld self, Window window)
		{
			//self.Log("Avalonia入口！！");
			Debug.WriteLine("Avalonia入口！！");
		}

		[NodeRule(nameof(UpdateRule<MainWorld>))]
		private static void OnUpdate(this MainWorld self)
		{
			//self.Log("Avalonia更新！！");
			Debug.WriteLine("Avalonia更新！！");
		}
	}
}
