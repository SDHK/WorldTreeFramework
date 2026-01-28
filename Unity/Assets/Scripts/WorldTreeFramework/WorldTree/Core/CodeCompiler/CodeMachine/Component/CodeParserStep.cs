using System;

namespace WorldTree
{
	/// <summary>
	/// 代码解析器：步骤执行
	/// </summary>
	public class CodeParserStep : Node
		, ComponentOf<CodeMachine>
		, AsRule<Awake>
	{

		/// <summary>
		/// 步骤列表 
		/// </summary>
		public UnitList<Action> stepList;

		/// <summary>
		/// 解析步骤 
		/// </summary>
		private int ParserStep(int address, int pointer)
		{
			stepList[address].Invoke();
			return pointer + 1;
		}

		/// <summary>
		/// 添加步骤 
		/// </summary>
		public CodeData GetStep(Action step)
		{
			stepList.Add(step);
			return new()
			{
				Parser = ParserStep,
				Address = stepList.Count - 1,
			};
		}
	}

	public static class CodeParserStepRule
	{
		class Awake : AwakeRule<CodeParserStep>
		{
			protected override void Execute(CodeParserStep self)
			{
				self.Core.PoolGetUnit(out self.stepList);
			}
		}

		class Remove : RemoveRule<CodeParserStep>
		{
			protected override void Execute(CodeParserStep self)
			{
				self.stepList.Dispose();
				self.stepList = null;
			}
		}
	}
}
