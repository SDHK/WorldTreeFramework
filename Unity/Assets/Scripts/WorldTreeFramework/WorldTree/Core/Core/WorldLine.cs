/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 世界线程
*

*/

using System;
using System.Collections.Concurrent;

namespace WorldTree
{

	/// <summary>
	/// 世界线管理器
	/// </summary>
	public class WorldLineManager
	{
		/// <summary>
		/// 世界线集合
		/// </summary>
		private ConcurrentDictionary<int, IWorldCore> lineDict;

		/// <summary>
		/// 创建世界线
		/// </summary>
		public void Create(int id, Type type, int frameTime)
		{

		}
	}


	/// <summary>
	/// 世界线
	/// </summary>
	public class WorldLine : Node, IWorldCore, IListenerIgnorer
	{

		#region 基础生命周期

		/// <summary>
		/// 添加法则组
		/// </summary>
		public IRuleGroup<Add> AddRuleGroup;
		/// <summary>
		/// 移除前法则组
		/// </summary>
		public IRuleGroup<BeforeRemove> BeforeRemoveRuleGroup;
		/// <summary>
		/// 移除法则组
		/// </summary>
		public IRuleGroup<Remove> RemoveRuleGroup;

		/// <summary>
		/// 嫁接法则组
		/// </summary>
		public IRuleGroup<Graft> GraftRuleGroup;

		/// <summary>
		/// 裁剪法则组
		/// </summary>
		public IRuleGroup<Cut> CutRuleGroup;

		/// <summary>
		/// 激活法则组
		/// </summary>
		public IRuleGroup<Enable> EnableRuleGroup;
		/// <summary>
		/// 禁用法则组
		/// </summary>
		public IRuleGroup<Disable> DisableRuleGroup;

		/// <summary>
		/// 序列化法则组
		/// </summary>
		public IRuleGroup<Serialize> SerializeRuleGroup;

		/// <summary>
		/// 反序列化法则组
		/// </summary>
		public IRuleGroup<Deserialize> DeserializeRuleGroup;
		#endregion

		/// <summary>
		/// Id管理器
		/// </summary>
		public IdManager IdManager;

		/// <summary>
		/// 类型信息
		/// </summary>
		public TypeInfo TypeInfo;

		/// <summary>
		/// 真实时间管理器
		/// </summary>
		public RealTimeManager RealTimeManager;

		/// <summary>
		/// 游戏时间管理器???
		/// </summary>
		public GameTimeManager GameTimeManager;

		/// <summary>
		/// 法则管理器
		/// </summary>
		public RuleManager RuleManager;

		/// <summary>
		/// 单位对象池管理器
		/// </summary>
		public UnitPoolManager UnitPoolManager;

		/// <summary>
		/// 节点对象池管理器
		/// </summary>
		public NodePoolManager NodePoolManager;

		/// <summary>
		/// 数组对象池管理器
		/// </summary>
		public ArrayPoolManager ArrayPoolManager;



		/// <summary>
		/// 节点引用池管理器
		/// </summary>
		public ReferencedPoolManager ReferencedPoolManager;

		/// <summary>
		/// 全局法则执行器管理器
		/// </summary>
		public GlobalRuleExecutorManager GlobalRuleExecutorManager;



		/// <summary>
		/// 世界之心
		/// </summary>
		public WorldHeartBase WorldHeart;

		/// <summary>
		/// 世界线：线程上下文
		/// </summary>
		public WorldContext WorldContext;

		public void Init(Type heartType, int frameTime)
		{

		}
	}
}