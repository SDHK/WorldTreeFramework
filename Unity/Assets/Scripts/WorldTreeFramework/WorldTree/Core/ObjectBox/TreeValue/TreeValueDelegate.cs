/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/6 10:59

* 描述： 绑定委托式泛型树值类型

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 绑定委托式泛型树值类型
	/// </summary>
	public partial class TreeValueDelegate<T> : TreeValueBase<T>
		, AsAwake<object, Func<object, T>, Action<object, T>>
		, ChildOf<INode>
	where T : IEquatable<T>
	{
		/// <summary>
		/// 绑定的对象
		/// </summary>
		public object bindObject;
		/// <summary>
		/// 从绑定对象上获取值
		/// </summary>
		public Func<object, T> getCallBack;
		/// <summary>
		/// 将值设置到绑定对象上
		/// </summary>
		public Action<object, T> setCallBack;
		public override T Value
		{
			get => getCallBack(bindObject);

			set
			{
				if (this.getCallBack(bindObject) is null)
				{
					this.setCallBack(bindObject, value);
				}
				else if (!getCallBack(bindObject).Equals(value))
				{
					setCallBack(bindObject, value);
					valueChange?.Send(value);
					globalValueChange?.Send(value);
				}
			}
		}
	}


	public static class TreeValueDelegateRule
	{
		class AwakeRuleGenerics<T> : AwakeRule<TreeValueDelegate<T>, object, Func<object, T>, Action<object, T>>
			where T : struct, IEquatable<T>
		{
			protected override void Execute(TreeValueDelegate<T> self, object arg1, Func<object, T> arg2, Action<object, T> arg3)
			{
				self.bindObject = arg1;
				self.getCallBack = arg2;
				self.setCallBack = arg3;
			}
		}
	}



}
