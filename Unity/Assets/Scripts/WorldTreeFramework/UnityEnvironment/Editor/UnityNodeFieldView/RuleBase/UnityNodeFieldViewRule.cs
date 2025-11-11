namespace WorldTree
{

	public static partial class UnityNodeFieldViewRule
	{
		/// <summary>
		/// 世界树节点字段信息Unity视图泛型法则
		/// </summary>
		/// <typeparam name="T">字段类型</typeparam>
		public abstract class GenericsViewRule<T> : NodeFieldViewRule<UnityNodeFieldView<T>>
		{ }
	}

}
