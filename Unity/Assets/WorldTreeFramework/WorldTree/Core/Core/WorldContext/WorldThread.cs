/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:56:05

* 描述： 世界线

*/

using System;
using System.Collections.Concurrent;
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

	/// <summary>
	/// 世界线X
	/// </summary>
	public interface IWorldLine : INode//包裹执行器
	{
		/// <summary>
		/// 单帧时间 (毫秒)
		/// </summary>
		public int FrameTime { get; set; }

		/// <summary>
		/// 暂停
		/// </summary>
		void Pause();

		/// <summary>
		/// 单帧运行
		/// </summary>
		void OneFrame();

		/// <summary>
		/// 添加世界
		/// </summary>
		bool TryAddWorld(IWorldTreeCore world);
		/// <summary>
		/// 移除世界
		/// </summary>
		void RemoveWorld(IWorldTreeCore world);
	}



	//世界,执行器需要单帧控制

	/// <summary>
	/// 世界线程接口
	/// </summary>
	public interface IWorldThread : INode
	{
		/// <summary>
		/// 单帧时间 (毫秒)
		/// </summary>
		public int FrameTime { get; set; }

		/// <summary>
		/// 运行
		/// </summary>
		void Run();

		/// <summary>
		/// 暂停
		/// </summary>
		void Pause();

		/// <summary>
		/// 单帧运行
		/// </summary>
		void OneFrame();

		/// <summary>
		/// 添加世界
		/// </summary>
		bool TryAddWorld(IWorldLine world);
		/// <summary>
		/// 移除世界
		/// </summary>
		void RemoveWorld(IWorldLine world);
	}

	/// <summary>
	/// 世界线程
	/// </summary>
	public class WorldThread : Node, IWorldThread
		, ComponentOf<WorldTreeCore>
		, AsRule<IAwakeRule>
	{
		public ConcurrentDictionary<long, IWorldLine> m_WorldLines = new();

		public int FrameTime { get; set; }

		public void Run()
		{
		}

		public void Pause()
		{
		}

		public void OneFrame()
		{
		}


		public bool TryAddWorld(IWorldLine worldLine)
		{
			return m_WorldLines.TryAdd(worldLine.Id, worldLine);
		}

		public void RemoveWorld(IWorldLine worldLine)
		{
			m_WorldLines.TryRemove(worldLine.Id, out _);
		}


	}

	public static class WorldThreadRule
	{
		class AddRule : AddRule<WorldThread>
		{
			protected override void OnEvent(WorldThread self)
			{
				self.Core.worldThread?.RemoveWorld(self.Core);
				self.TryAddWorld(self.Core);
			}
		}
	}
}
