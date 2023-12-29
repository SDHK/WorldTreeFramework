/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:56:05

* 描述： 世界线

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	public class WorldTreeCoreBase : Node
	{

		/// <summary>
		/// 法则管理器
		/// </summary>
		public RuleManager RuleManager;



		public GlobalRuleActuator<IEnableRule> enable;
		public GlobalRuleActuator<IDisableRule> disable;
		public GlobalRuleActuator<IUpdateRule> update;
		public GlobalRuleActuator<IUpdateTimeRule> updateTime;
	}


	public interface IWorldThread
	{
		INode WorldCore { get; }
		INode WorldContext { get; }


	}

	/// <summary>
	/// 世界线
	/// </summary>
	public class WorldThread : Node
	{



	}
}
