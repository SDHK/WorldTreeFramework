/****************************************

* 作者：闪电黑客
* 日期：2024/7/23 15:57

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 序列化法则接口：用于序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreePackSerialize : IRule { }

	/// <summary>
	/// 反序列化法则接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface ITreePackDeserialize : IRule { }

	/// <summary>
	/// 序列化法则
	/// </summary>
	public interface TreePackSerialize<T> : ISendRefRule<T>, ITreePackSerialize { }

	/// <summary>
	/// 反序列化法则
	/// </summary>
	public interface TreePackDeserialize<T> :ISendRefRule<T>, ITreePackDeserialize { }
}
