using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 步骤执行器 
	/// </summary>
	public partial class StepMachine : Node
		, AsComponentBranch
		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 参数栈 
		/// </summary>
		public UnitStack<VarValue> ParamStack;

		/// <summary>
		/// 步骤列表
		/// </summary>
		public List<StepData> StepList = new();

		/// <summary>
		/// 步骤指针
		/// </summary>
		public int Pointer = -1;

		/// <summary>
		/// 运行标记
		/// </summary>
		public bool isRun = false;

		/// <summary>
		/// 弹出参数
		/// </summary>
		public VarValue Pop() => ParamStack.Count > 0 ? ParamStack.Pop() : new VarValue();

		/// <summary>
		/// 压入参数 
		/// </summary>
		public void Push(VarValue value) => ParamStack.Push(value);

		/// <summary>
		/// 添加步骤
		/// </summary>
		public void AddStep(StepData stepData)
		{
			StepList.Add(stepData);
		}

		/// <summary>
		/// 执行步骤
		/// </summary>
		public void Update()
		{
			if (Pointer != -1 && Pointer < StepList.Count && isRun == true)
			{
				Pointer = StepList[Pointer].Run(Pointer);
			}
			else if (Pointer >= StepList.Count)
			{
				Pointer = -1;
				isRun = false;
			}
		}
	}
}
