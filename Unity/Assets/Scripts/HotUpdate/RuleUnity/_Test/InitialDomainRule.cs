using System;
using UnityEngine;
using WorldTree.Internal;

namespace WorldTree
{

	public class T1<T> : Node
	{

	}


	public delegate T Call<N, T>(N self, TimeSpan timeSpan = default) where N : class, INode, AsRule<UpdateTime>;

	public delegate TreeTask<T> CallAsync<N, T>(N self, TimeSpan timeSpan = default) where N : class, INode, AsRule<UpdateTime>;

	/// <summary>
	/// 异步调用法则基类接口
	/// </summary>
	public interface ICallRuleAsync1<OutT> : IRule
	{
		TreeTask<OutT> Invoke(INode self);
	}

	/// <summary>
	/// 代码生成兄弟类型调用委托
	/// </summary>
	public static partial class T1Rule
	{
		class UpdateTimeRule : UpdateTimeRule<T1<float>>
		{
			public override void Invoke(INode self, TimeSpan arg1) => UpdateTime(self as T1<float>, arg1);

			protected override void Execute(T1<float> self, TimeSpan timeSpan)
			{
				UpdateTime(self, timeSpan);
			}
		}
	}

	/// <summary>
	/// 业务写的代码
	/// </summary>
	public static partial class T1Rule
	{
		static OnUpdateTime<T1<float>> UpdateTime = (self, timeSpan) =>
		{
			self.Log($"T1<float>更新！！{timeSpan.TotalSeconds}");
		};


	}



	public static partial class InitialDomainRule
	{
		//测试框架功能
		private class AddRule : AddRule<InitialDomain>
		{
			protected override void Execute(InitialDomain self)
			{
				self.Log($"初始域热更部分！！!");



				//self.Core.AddWorld(out WorldTreeCore core);//添加子世界
				self.AddComponent(out TaskTest _);

				//添加宏测试成功，应该在编辑器下才能添加
				//string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
				//if (!currentDefines.Contains("MY_NEW_DEFINE")) currentDefines += ";MY_NEW_DEFINE";
				//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentDefines);
			}
		}

		private class Update : UpdateRule<InitialDomain>
		{
			protected override void Execute(InitialDomain self)
			{
			}
		}

		private class EnableRule : EnableRule<InitialDomain>
		{
			protected override void Execute(InitialDomain self)
			{
				self.Log($"初始域激活！！");
			}
		}

		private class DisableRule : DisableRule<InitialDomain>
		{
			protected override void Execute(InitialDomain self)
			{
				self.Log($"初始域禁用！！");
			}
		}

		private class UpdateTimeRule : UpdateTimeRule<InitialDomain>
		{
			protected override void Execute(InitialDomain self, TimeSpan timeSpan)
			{
				//self.Log($"初始域更新！！{timeSpan.TotalSeconds}");
			}
		}

		private class GuiUpdateTimeRule : GuiUpdateTimeRule<InitialDomain>
		{
			private GUIStyle textStyle = new GUIStyle() { fontSize = 60 };

			protected override void Execute(InitialDomain self, TimeSpan timeSpan)
			{
				textStyle.normal.textColor = Color.green;
				GUILayout.Label($"            !!!!{timeSpan.TotalMilliseconds}", textStyle);
			}
		}

		private class RemoveRule : RemoveRule<InitialDomain>
		{
			protected override void Execute(InitialDomain self)
			{
				self.Log($"初始域关闭！！");
			}
		}
	}
}