/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/24 06:17:03

* 描述：可视化类型管理器

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 可视化类型管理器
	/// </summary>
	public class ViewTypeManager : Node
		, AsAwake
		, ComponentOf<WorldTreeRoot>
	{
		/// <summary>
		/// 字段类型，可视化节点类型
		/// </summary>
		public Dictionary<Type, Type> types = new();
	}
	public static class ViewTypeManagerRule
	{
		class AddRule : AddRule<ViewTypeManager>
		{
			protected override void Execute(ViewTypeManager self)
			{
				//收集所有实现了INodeFieldViewRule的法则的节点 
				if (self.Core.RuleManager.TryGetRuleGroup(typeof(INodeFieldViewRule).TypeToCode(), out RuleGroup ruleGroup))
				{
					foreach (var NodeTypeRuleType in ruleGroup)
					{
						Type nodeType = NodeTypeRuleType.Key.CodeToType();

						if (nodeType.IsGenericType)
						{
							Type paramType = nodeType.GetGenericArguments()[0];
							self.types.TryAdd(paramType, nodeType);
						}
					}
				}
			}
		}
	}
}
