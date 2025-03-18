/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 泛型全局单法则执行器

*/

using System;

namespace WorldTree
{

	/// <summary>
	/// 全局法则执行器
	/// </summary>
	public partial class GlobalRuleExecutor<R> : RuleGroupExecutorBase, INodeListener, IRuleExecutor<IRule>
		, TypeNodeOf<GlobalRuleExecutorManager>
		, AsAwake
		where R : IGlobalRule
	{
		public override string ToString()
		{
			return GetType().ToString();
		}
	}

	public static class GlobalRuleExecutorRule
	{
		class ListenerAddRule<R> : ListenerAddRule.Rule<GlobalRuleExecutor<R>, R>
			where R : IGlobalRule
		{
			protected override void Execute(GlobalRuleExecutor<R> self, INode node)
			{
				self.TryAdd(node);
			}
		}

		class Add<R> : AddRule<GlobalRuleExecutor<R>>
			where R : IGlobalRule
		{
			protected override void Execute(GlobalRuleExecutor<R> self)
			{
				self.GetBaseRule<GlobalRuleExecutor<R>, RuleGroupExecutorBase, Add>().Send(self);
				self.ruleGroupDict = self.Core.RuleManager.GetOrNewRuleGroup<R>();
				self.LoadGlobalNode();
			}
		}

		/// <summary>
		/// 填装全局节点
		/// </summary>
		public static void LoadGlobalNode<R>(this GlobalRuleExecutor<R> self)
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


	public partial class GlobalRuleExecutor<R>
	{
		class TreeDataSerialize : TreeDataSerializeRule<GlobalRuleExecutor<R>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out GlobalRuleExecutor<R> obj, false, writeType: typeof(GlobalRuleExecutorData))) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.TypeToCode<R>());
			}
		}
		class TreeDataDeserialize : TreeDataDeserializeRule<GlobalRuleExecutor<R>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(GlobalRuleExecutorData), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				long ruleTypeCode = self.ReadValue<long>();
				value = self.Core.GetGlobalRuleExecutor(ruleTypeCode);
			}
		}

	}

}
