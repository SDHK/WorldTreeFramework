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
	public interface ISerialize : IRule { }

	/// <summary>
	/// 反序列化法则接口：用于反序列化未知泛型，解除AsRule的法则限制
	/// </summary>
	public interface IDeserialize : IRule { }

	/// <summary>
	/// 序列化法则
	/// </summary>
	public interface Serialize<T> : ISendRefRule<T>, ISerialize { }

	/// <summary>
	/// 反序列化法则
	/// </summary>
	public interface Deserialize<T> :ISendRefRule<T>, IDeserialize { }
}
