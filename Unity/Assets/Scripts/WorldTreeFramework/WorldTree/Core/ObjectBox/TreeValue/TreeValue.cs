/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 泛型树值类型

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 泛型树值类型
	/// </summary>
	public partial class TreeValue<T> : TreeValueBase<T>
		, ChildOf<INode>
		, AsRule<Awake>
		, AsRule<Awake<T>>
		where T : IEquatable<T>
	{
		/// <summary>
		/// 值
		/// </summary>
		public T value;

		public override T Value
		{
			get => value;

			set
			{
				if (this.value is null)
				{
					this.value = value;
				}
				else if (!this.value.Equals(value))
				{
					this.value = value;
					valueChange?.Send(value);
					//globalValueChange?.Send(value);
				}
			}
		}
	}
}