/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 树节点对象池
*
* 管理类型： INode
*   
*   调用 节点 生命周期法则， 生成， 获取， 回收， 销毁，
*   
*   同时对 节点 赋予 根节点 和 Id 分发。
*   
*/
using System;

namespace WorldTree
{

	/// <summary>
	/// 实体对象池
	/// </summary>
	public class NodePool : GenericPool<INode>
		, ChildOf<PoolManagerBase<NodePool>>
	{
		/// <summary>
		/// 对象新建规则列表
		/// </summary>
		public IRuleList<New> newRule;
		/// <summary>
		/// 对象获取规则列表
		/// </summary>
		public IRuleList<Get> getRule;
		/// <summary>
		/// 对象回收规则列表
		/// </summary>
		public IRuleList<Recycle> recycleRule;
		/// <summary>
		/// 对象销毁规则列表
		/// </summary>
		public IRuleList<Destroy> destroyRule;


		public NodePool() : base()
		{
			NewObject = ObjectNew;
			DestroyObject = ObjectDestroy;

			objectOnNew = ObjectOnNew;
			objectOnGet = ObjectOnGet;
			objectOnRecycle = ObjectOnRecycle;
			objectOnDestroy = ObjectOnDestroy;
		}

		public override string ToString()
		{
			return $"[NodePool<{ObjectType}>] : {Count} ";
		}

		/// <summary>
		/// 获取对象并转为指定类型
		/// </summary>
		public T Get<T>()
			where T : class
		{
			return Get() as T;
		}

		public override void Recycle(object obj) => Recycle(obj as INode);
		/// <summary>
		/// 回收对象
		/// </summary>
		public void Recycle(INode obj)
		{
			lock (objectPoolQueue)
			{
				if (obj != null)
				{
					if (maxLimit == -1 || objectPoolQueue.Count < maxLimit)
					{
						if (obj.IsRecycle) return;

						objectOnRecycle.Invoke(obj);
						objectPoolQueue.Enqueue(obj);
					}
					else
					{
						objectOnRecycle.Invoke(obj);
						objectOnDestroy.Invoke(obj);
						DestroyObject.Invoke(obj);
					}
				}
			}
		}
		/// <summary>
		/// 对象新建方法
		/// </summary>
		private INode ObjectNew(IPool pool)
		{
			INode obj = Activator.CreateInstance(ObjectType, true) as INode;
			obj.IsFromPool = true;
			obj.Core = Core;
			obj.Root = Core.Root;
			obj.Type = ObjectTypeCode;
			return obj;
		}
		/// <summary>
		/// 对象销毁方法
		/// </summary>
		private void ObjectDestroy(INode obj)
		{
		}
		/// <summary>
		/// 对象新建处理事件
		/// </summary>
		private void ObjectOnNew(INode obj)
		{
			newRule?.Send(obj);
		}
		/// <summary>
		/// 对象获取处理事件
		/// </summary>
		private void ObjectOnGet(INode obj)
		{
			obj.IsRecycle = false;
			getRule?.Send(obj);
		}
		/// <summary>
		/// 对象回收处理事件
		/// </summary>
		public void ObjectOnRecycle(INode obj)
		{
			obj.IsRecycle = true;
			recycleRule?.Send(obj);
			obj.Id = 0;
		}
		/// <summary>
		/// 对象销毁处理事件
		/// </summary>
		private void ObjectOnDestroy(INode obj)
		{
			obj.IsDisposed = true;
			destroyRule?.Send(obj);
		}

	}


	public static partial class NodePoolRule
	{
		class GraftRule : GraftRule<NodePool>
		{
			protected override void Execute(NodePool self)
			{
				self.Core.RuleManager.SupportNodeRule(self.ObjectTypeCode);

				//生命周期法则
				self.Core.RuleManager.TryGetRuleList(self.ObjectTypeCode, out self.newRule);
				self.Core.RuleManager.TryGetRuleList(self.ObjectTypeCode, out self.getRule);
				self.Core.RuleManager.TryGetRuleList(self.ObjectTypeCode, out self.recycleRule);
				self.Core.RuleManager.TryGetRuleList(self.ObjectTypeCode, out self.destroyRule);
			}
		}
		class DestroyRule : DestroyRule<NodePool>
		{
			protected override void Execute(NodePool self)
			{
				self.DisposeAll();
				self.NewObject = null;
				self.DestroyObject = null;
				self.objectOnNew = null;
				self.objectOnGet = null;
				self.objectOnRecycle = null;
				self.objectOnDestroy = null;

				self.newRule = default;
				self.getRule = default;
				self.recycleRule = default;
				self.destroyRule = default;
			}
		}
	}
}
