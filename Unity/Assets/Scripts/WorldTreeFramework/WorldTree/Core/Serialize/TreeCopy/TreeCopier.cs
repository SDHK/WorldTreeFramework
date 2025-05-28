/****************************************

* 作者：闪电黑客
* 日期：2025/5/20 20:14

* 描述：

*/


using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	public static partial class TreeCopierRule
	{
		class AddRule : AddRule<TreeCopier>
		{
			protected override void Execute(TreeCopier self)
			{
				// 获取节点的法则集
				self.Core.RuleManager.TryGetRuleGroup<TreeCopyStruct>(out self.copyStructRuleDict);
				self.Core.PoolGetUnit(out self.ObjectToObjectDict);
			}
		}

		class RemoveRule : RemoveRule<TreeCopier>
		{
			protected override void Execute(TreeCopier self)
			{
				self.ObjectToObjectDict.Dispose();
			}
		}
	}

	/// <summary>
	/// 树深拷贝执行器
	/// </summary>
	public class TreeCopier : Node
		, TempOf<INode>
		, AsRule<ITreeCopy>
		, AsAwake
	{
		/// <summary>
		/// 对象对应对象字典
		/// </summary>
		public UnitDictionary<object, object> ObjectToObjectDict;

		/// <summary>
		/// 不同类型序列化法则列表集合
		/// </summary>
		public RuleGroup copyStructRuleDict;

		/// <summary>
		/// 拷贝对象
		/// </summary>
		public T CopyTo<T>(T source, ref T target) => InternalCopy(source, ref target, true);

		/// <summary>
		/// 拷贝对象
		/// </summary>
		public T Copy<T>(T source)
		{
			T target = default;
			return InternalCopy(source, ref target);
		}

		/// <summary>
		/// 内部使用拷贝对象
		/// </summary>
		public T InternalCopy<T>(T source, ref T target, bool isClear = false)
		{
			if (isClear) ObjectToObjectDict.Clear();

			if (EqualityComparer<T>.Default.Equals(source, default))
			{
				// 如果是默认值，直接赋值并返回
				target = source;
				return target;
			}

			// 如果是纯值类型（不包含引用字段），直接赋值并返回
			if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				target = source;
				return target;
			}

			Type type = source.GetType();
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			// 前面 纯值类型挡住了，这里是判断 包含引用的结构体
			if (type.IsValueType)
			{
				if (copyStructRuleDict.TryGetValue(typeCode, out RuleList ruleList))
				{
					for (int i = 0; i < ruleList.Count; i++)
					{
						Unsafe.As<TreeCopyStructRule<T>>(ruleList[i]).Invoke(this, ref source, ref target);
					}
				}
			}
			// 不是结构体，就是类型
			else if (this.Core.RuleManager.TryGetRuleList<TreeCopy>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				object sourceObj = source;
				object targetObj = null;

				// 尝试获取目标对象
				if (!ObjectToObjectDict.TryGetValue(sourceObj, out targetObj))
				{
					// 不存在则拷贝
					targetObj = target;
					((IRuleList<TreeCopy>)ruleList).SendRef(this, ref sourceObj, ref targetObj);
				}

				target = (T)targetObj;

				// 记录引用类型
				if (sourceObj != null && targetObj != null)
					ObjectToObjectDict.TryAdd(sourceObj, targetObj);
			}
			return target;
		}

		/// <summary>
		/// 内部使用危险指定类型拷贝对象
		/// </summary>
		public void InternalTypeCopy(Type type, object source, ref object target)
		{
			long typeCode = this.TypeToCode(type);
			if (Core.RuleManager.TryGetRuleList<TreeCopy>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
				((IRuleList<TreeCopy>)ruleList).SendRef(this, ref source, ref target);
		}
	}
}