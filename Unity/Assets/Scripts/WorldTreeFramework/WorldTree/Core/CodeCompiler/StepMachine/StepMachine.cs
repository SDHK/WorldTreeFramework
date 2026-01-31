using System;
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
		public int PopParam() => ParamAddress--;

		/// <summary>
		/// 获取参数 
		/// </summary>
		public VarValue GetParam(int address) => ParamList[address];

		/// <summary>
		/// 设置参数 
		/// </summary>
		public void SetParam(int address, VarValue value) => ParamList[address] = value;



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
