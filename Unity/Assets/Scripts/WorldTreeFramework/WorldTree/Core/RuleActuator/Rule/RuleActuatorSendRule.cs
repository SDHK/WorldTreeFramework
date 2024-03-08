namespace WorldTree
{
	public static class RuleActuatorSendRule
	{
		public static void Send<R>(this IRuleActuator<R> Self)
			where R : ISendRuleBase
		{
			if (Self.IsActive)
			{
				IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
				foreach ((INode, RuleList) nodeRuleTuple in self)
				{
					((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1);
				}
			}
		}
		public static void Send<R, T1>(this IRuleActuator<R> Self, T1 arg1)
			where R : ISendRuleBase<T1>
		{
			if (Self.IsActive)
			{
				IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
				foreach ((INode, RuleList) nodeRuleTuple in self)
				{
					((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1, arg1);
				}
			}
		}
		public static void Send<R, T1, T2>(this IRuleActuator<R> Self, T1 arg1, T2 arg2)
			where R : ISendRuleBase<T1, T2>
		{
			if (Self.IsActive)
			{
				IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
				foreach ((INode, RuleList) nodeRuleTuple in self)
				{
					((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1, arg1, arg2);
				}
			}
		}
		public static void Send<R, T1, T2, T3>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3)
			where R : ISendRuleBase<T1, T2, T3>
		{
			if (Self.IsActive)
			{
				IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
				foreach ((INode, RuleList) nodeRuleTuple in self)
				{
					((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1, arg1, arg2, arg3);
				}
			}
		}
		public static void Send<R, T1, T2, T3, T4>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			where R : ISendRuleBase<T1, T2, T3, T4>
		{
			if (Self.IsActive)
			{
				IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
				foreach ((INode, RuleList) nodeRuleTuple in self)
				{
					((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1, arg1, arg2, arg3, arg4);
				}
			}
		}
		public static void Send<R, T1, T2, T3, T4, T5>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			where R : ISendRuleBase<T1, T2, T3, T4, T5>
		{
			if (Self.IsActive)
			{
				IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
				foreach ((INode, RuleList) nodeRuleTuple in self)
				{
					((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1, arg1, arg2, arg3, arg4, arg5);
				}
			}
		}
	}

}
