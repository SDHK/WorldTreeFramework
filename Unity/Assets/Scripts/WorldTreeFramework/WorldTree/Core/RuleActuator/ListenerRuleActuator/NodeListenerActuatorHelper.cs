/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 20:34

* 描述： 节点监听执行器帮助类

*/

namespace WorldTree
{
	/// <summary>
	/// 节点监听执行器帮助类
	/// </summary>
	public static class NodeListenerActuatorHelper
	{
		/// <summary>
		/// 获取节点监听执行器
		/// </summary>
		/// <remarks>获取监听这个节点的执行器</remarks>
		public static IRuleActuator<R> GetListenerActuator<R>(INode node)
		   where R : IListenerRule
		{
			if (node.Core.ReferencedPoolManager != null)
			{
				if (node.Core.ReferencedPoolManager.TryGetPool(node.Type, out ReferencedPool nodePool))
				{
					if (nodePool.AddComponent(out HybridListenerRuleActuatorGroup _, isPool: false).TryAddRuleActuator(node.Type, out IRuleActuator<R> actuator))
					{
						return actuator;
					}
				}
			}
			return null;
		}
	}



}
