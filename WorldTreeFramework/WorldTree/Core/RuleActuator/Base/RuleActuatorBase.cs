﻿/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则执行器基类
*
* 执行拥有指定法则的节点
* 用于代替委托事件

 */

using System;

namespace WorldTree
{
    /// <summary>
    /// 法则执行器 接口基类
    /// </summary>
    public interface IRuleActuator : INode { }

    /// <summary>
    /// 法则执行器 逆变泛型接口
    /// </summary>
    /// <typeparam name="T">法则类型</typeparam>
    /// <remarks>
    /// <para>主要通过法则类型逆变提示可填写参数</para>
    /// </remarks>
    public interface IRuleActuator<in T> : IRuleActuator where T : IRule { }




    /// <summary>
    /// 法则执行器基类
    /// </summary>
    public abstract class RuleActuatorBase : Node, ChildOf<INode>, IRuleActuator
    {
        /// <summary>
        /// 单法则集合
        /// </summary>
        public RuleGroup ruleGroup;


        /// <summary>
        /// 节点id队列
        /// </summary>
        public TreeQueue<long> idQueue;

        /// <summary>
        /// 节点字典
        /// </summary>
        public TreeDictionary<long, INode> nodeDictionary;

        /// <summary>
        /// 节点id被移除的次数
        /// </summary>
        public TreeDictionary<long, int> removeIdDictionary;

        /// <summary>
        /// 法则集合字典
        /// </summary>
        public TreeDictionary<long, RuleGroup> ruleGroupDictionary;

        /// <summary>
        /// 动态的遍历数量
        /// </summary>
        /// <remarks>当遍历时移除后，减少数量</remarks>
        public int traversalCount;

        public override string ToString()
        {
            return $"RuleActuator : {ruleGroup?.RuleType}";
        }

        /// <summary>
        /// 刷新动态遍历数量
        /// </summary>
        public int RefreshTraversalCount()
        {
            return traversalCount = idQueue is null ? 0 : idQueue.Count;
        }

        /// <summary>
        /// 尝试出列
        /// </summary>
        public bool TryGetNext(out INode node, out RuleGroup ruleGroup)
        {
            //尝试获取一个id
            if (idQueue != null && idQueue.TryDequeue(out long id))
            {
                //假如id被回收了
                while (removeIdDictionary != null && removeIdDictionary.TryGetValue(id, out int count))
                {
                    //回收次数抵消
                    removeIdDictionary[id] = --count;
                    //遍历数抵消
                    if (traversalCount != 0) traversalCount--;

                    //次数为0时删除id
                    if (count == 0) removeIdDictionary.Remove(id);
                    if (removeIdDictionary.Count == 0)
                    {
                        removeIdDictionary.Dispose();
                        removeIdDictionary = null;
                    }

                    //获取下一个id
                    if (!idQueue.TryDequeue(out id))
                    {
                        //假如队列空了,则直接返回退出
                        ruleGroup = this.ruleGroup;
                        node = null;
                        return false;
                    }
                }

                //此时的id是正常id
                if (ruleGroupDictionary == null || !ruleGroupDictionary.TryGetValue(id, out ruleGroup))
                {
                    ruleGroup = this.ruleGroup;
                }
                //id压回队列
                idQueue.Enqueue(id);
                return nodeDictionary.TryGetValue(id, out node);
            }
            ruleGroup = this.ruleGroup;
            node = null;
            return false;
        }

        #region 添加

        /// <summary>
        /// 尝试添加节点
        /// </summary>
        public bool TryAdd(INode node)
        {
            nodeDictionary ??= this.AddChild(out nodeDictionary);
            if (nodeDictionary.ContainsKey(node.Id))
            {
                return false;
            }

            idQueue ??= this.AddChild(out idQueue);
            nodeDictionary ??= this.AddChild(out nodeDictionary);

            idQueue.Enqueue(node.Id);
            nodeDictionary.Add(node.Id, node);

            return true;
        }

        /// <summary>
        /// 尝试添加节点，并建立引用关系
        /// </summary>
        public bool TryAddReferenced(INode node)
        {
            if (TryAdd(node))
            {
                this.Referenced(node);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 尝试添加节点与对应法则
        /// </summary>
        public bool TryAdd(INode node, RuleGroup ruleGroup)
        {

            if (ruleGroup != null)
            {
                if (TryAdd(node))
                {
                    ruleGroupDictionary ??= this.AddChild(out ruleGroupDictionary);
                    ruleGroupDictionary.Add(node.Id, ruleGroup);
                    return true;
                }
                else
                {
                    if (ruleGroupDictionary.TryGetValue(node.Id, out RuleGroup OldRuleGroup))
                    {
                        if (OldRuleGroup.RuleType != ruleGroup.RuleType) World.LogError("执行器中已存在这个节点，但注册的法则不同。");
                    }
                    return false;
                }
            }
            else
            {
                World.LogError("法则为空");
                return false;
            }
        }

        /// <summary>
        /// 尝试添加节点与对应法则，并建立引用关系
        /// </summary>
        public bool TryAddReferenced(INode node, RuleGroup ruleGroup)
        {
            if (TryAdd(node, ruleGroup))
            {
                this.Referenced(node);
                return true;
            }
            return false;
        }



        #endregion

        #region 移除

        /// <summary>
        /// 移除节点
        /// </summary>
        public void Remove(long id)
        {
            if (nodeDictionary != null && nodeDictionary.TryGetValue(id, out INode node))
            {
                nodeDictionary.Remove(id);
                ruleGroupDictionary?.Remove(id);

                this.DeReferenced(node);
                //累计强制移除的节点id
                removeIdDictionary ??= this.AddChild(out removeIdDictionary);
                if (removeIdDictionary.TryGetValue(id, out var count))
                {
                    removeIdDictionary[id] = count + 1;
                }
                else
                {
                    removeIdDictionary.Add(id, 1);
                }

                if (nodeDictionary.Count == 0)
                {
                    Clear();
                }
            }

        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public void Remove(INode node)
        {
            Remove(node.Id);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            if (nodeDictionary != null)
            {
                foreach (INode node in nodeDictionary.Values)
                {
                    this.DeReferenced(node);
                }
                nodeDictionary.Dispose();
                nodeDictionary = null;
            }

            idQueue?.Dispose();
            ruleGroupDictionary?.Dispose();
            removeIdDictionary?.Dispose();

            idQueue = null;
            ruleGroupDictionary = null;
            removeIdDictionary = null;

            traversalCount = 0;
        }




        #endregion

    }

    public static class RuleActuatorBaseRule
    {
        class RemoveRule : RemoveRule<RuleActuatorBase>
        {
            public override void OnEvent(RuleActuatorBase self)
            {
                self.ruleGroup = null;
                self.Clear();
            }
        }
    }


}
