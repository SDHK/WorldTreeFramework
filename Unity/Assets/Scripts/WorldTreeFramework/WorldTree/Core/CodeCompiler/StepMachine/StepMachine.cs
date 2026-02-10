using System;

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
		/// 参数寄存器
		/// </summary>
		public UnitList<VarValue> ParamList;

		/// <summary>
		/// 参数地址
		/// </summary>
		public int ParamAddress;

		/// <summary>
		/// 参数最大容量记录
		/// </summary>
		public int MaxCapacity;

		/// <summary>
		/// 步骤列表
		/// </summary>
		public UnitList<StepExecuteData> StepList;

		/// <summary>
		/// 步骤指针
		/// </summary>
		public int Pointer = -1;

		/// <summary>
		/// 运行标记
		/// </summary>
		public bool isRun = false;

		/// <summary>
		/// 压入参数地址 
		/// </summary>
		public int PushParam()
		{
			MaxCapacity = Math.Max(MaxCapacity, ParamAddress + 1);
			return ParamAddress++;
		}

		/// <summary>
		/// 弹出参数地址 
		/// </summary>
		public int PopParam() => --ParamAddress;

		/// <summary>
		/// 查看栈顶参数地址
		/// </summary> 
		public int PeekParam() => ParamAddress - 1;

		/// <summary>
		/// 获取参数 
		/// </summary>
		public VarValue GetParam(int address) => ParamList[address];

		/// <summary>
		/// 设置参数 
		/// </summary>
		public void SetParam(int address, VarValue value)
		{
			ParamList[address] = value;
		}


		/// <summary>
		/// 添加步骤
		/// </summary>
		public void AddStep(StepExecuteData stepData)
		{
			StepList.Add(stepData);
		}

		/// <summary>
		/// 启动
		/// </summary>
		public void Run()
		{
			//List扩容到最大参数容量
			for (int i = ParamList.Count; i < MaxCapacity; i++)
			{
				ParamList.Add(new VarValue());
			}
			Pointer = 0;
			isRun = true;
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

	public static class StepMachineRule
	{
		class Awake : AwakeRule<StepMachine>
		{
			protected override void Execute(StepMachine self)
			{
				self.Core.PoolGetUnit(out self.ParamList);
				self.Core.PoolGetUnit(out self.StepList);
				self.ParamAddress = 0;
				self.MaxCapacity = 0;
			}
		}
	}
}
