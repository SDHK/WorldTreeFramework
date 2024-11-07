namespace WorldTree
{
	/// <summary>
	/// 序列化法则
	/// </summary>
	public interface Serialize : ISendRule, ILifeCycleRule
	{ }

	/// <summary>
	/// 反序列化法则
	/// </summary>
	public interface Deserialize : ISendRule, ILifeCycleRule
	{ }
}