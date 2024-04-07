using System;
using System.Reflection;
using System.Runtime.Loader;

namespace WorldTree
{
	/// <summary>
	/// 测试节点
	/// </summary>
	public class DotNetTestNode : Node, ComponentOf<INode>
		, AsRule<IAwakeRule>
	{
		public int TestValue;
	}

	public static partial class DotNetTestNodeRule
	{
		private class AddRule : AddRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				// 1. 创建自定义AssemblyLoadContext
				//var alc = new AssemblyLoadContext("NewALC", true);

				//// 3. 加载程序集
				//alc.LoadFromAssemblyPath("A.dll");

				//// 4. 创建对象并调用
				//Type t = alc.LoadFromAssemblyName(new AssemblyName("A.MyClass")).GetType("A.MyClass");
				//dynamic obj = Activator.CreateInstance(t);
				//obj.MyMethod();

				////卸载程序集
				//alc.Unload();

				self.Log(" 初始化！！！!!");

				//self.AddComponent(out RefeshCsProjFileCompileInclude _);
				//self.Log(self.Core.ToStringDrawTree());
			}
		}

		private class UpdateTimeRule : UpdateTimeRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self, TimeSpan timeSpan)
			{
				self.Log($"初始更新！！{timeSpan.TotalSeconds}");
			}
		}

		private class RemoveRule : RemoveRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log($"初始关闭！！");
			}
		}
	}
}