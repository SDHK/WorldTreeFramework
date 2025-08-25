/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 泛型全局广播法则执行器

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 法则全局广播
	/// </summary>
	public interface RuleBroadcast<in R> : IRuleExecutor<R> where R : IGlobalRule { }

	/// <summary>
	/// 全局广播法则执行器：请使用 RuleBroadcast<R>
	/// </summary>
	public partial class RuleBroadcaster<R> : RuleBroadcaster, INodeListener, RuleBroadcast<IGlobalRule>
		, GenericOf<long, RuleBroadcastManager>
		, AsRule<Awake>
		, AsRule<ListenerAdd>
		where R : IGlobalRule
	{ }

	/// <summary>
	/// 全局广播法则执行器基类
	/// </summary>
	public abstract class RuleBroadcaster : RuleExecutorBase, IRuleExecutorOperate, IRuleExecutorEnumerable
		, AsChildBranch
	{
		/// <summary>
		/// 法则类型
		/// </summary>
		public long RuleType => ruleGroupDict.RuleType;

		/// <summary>
		/// 单法则集合
		/// </summary>
		[Protected] public RuleGroup ruleGroupDict;

		public override string ToString()
		{
			return GetType().ToString();
		}

		/// <summary>
		/// 尝试添加节点
		/// </summary>
		public bool TryAdd(INode node)
		{
			if (node == null) return false;
			if (ruleGroupDict == null || !ruleGroupDict.TryGetValue(node.Type, out RuleList rule)) return false;
			return this.TryAdd(node, rule);
		}

		public override void Remove(long id)
		{
			//全局事件的对象是释放移除，不能手动移除
		}

		public override void Remove(INode node)
		{
			//全局事件的对象是释放移除，不能手动移除
		}
	}

	//序列化处理
	public partial class RuleBroadcaster<R>
	{
		class TreeDataSerialize : TreeDataSerializeRule<RuleBroadcaster<R>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out RuleBroadcaster<R> obj, false, writeType: typeof(RuleBroadcastData))) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.TypeToCode<R>());
			}
		}
		class TreeDataDeserialize : TreeDataDeserializeRule<RuleBroadcaster<R>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(RuleBroadcastData), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				long ruleTypeCode = self.ReadValue<long>();
				value = self.Core.GetRuleBroadcast(ruleTypeCode);
			}
		}
	}


	public static class RuleBroadcasterRule
	{
		class ListenerAddRule<R> : ListenerRuleAddRule<RuleBroadcaster<R>, R>
			where R : IGlobalRule
		{
			protected override void Execute(RuleBroadcaster<R> self, INode node)
			{
				self.TryAdd(node);
			}
		}

		class Add<R> : AddRule<RuleBroadcaster<R>>
			where R : IGlobalRule
		{
			protected override void Execute(RuleBroadcaster<R> self)
			{
				self.GetBaseRule<RuleBroadcaster, RuleExecutorBase, Add>().Send(self);
				self.ruleGroupDict = self.Core.RuleManager.GetOrNewRuleGroup<R>();
				self.LoadGlobalNode();
			}
		}

		/// <summary>
		/// 填装全局节点
		/// </summary>
		public static void LoadGlobalNode<R>(this RuleBroadcaster<R> self)
			where R : IGlobalRule
		{
			foreach (var item in self.ruleGroupDict)
			{
				bool isListenerIgnorer = false;
				foreach (Type typeItem in self.CodeToType(item.Key).GetInterfaces())
				{
					if (typeItem == typeof(IListenerIgnorer))
					{
						isListenerIgnorer = true;
						break;
					}
				}

				if (!isListenerIgnorer)
				{
					if (self.Core.ReferencedPoolManager.TryGetPool(item.Key, out ReferencedPool pool))
					{
						foreach (var node in pool.NodeDict) self.TryAdd(node.Value);
					}
				}
			}
		}
	}




}
