using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 代码执行器 
	/// </summary>
	public class CodeMachine : Node
		, AsComponentBranch
		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 指令列表
		/// </summary>
		public List<CodeData> CodeDataList = new();

		/// <summary>
		/// 指令指针
		/// </summary>
		public int Pointer = -1;

		/// <summary>
		/// 运行标记
		/// </summary>
		public bool isRun = false;

		/// <summary>
		/// 添加指令 
		/// </summary>
		public void AddCode(CodeData codeData)
		{
			CodeDataList.Add(codeData);
		}

		/// <summary>
		/// 任务解析器列表 
		/// </summary>
		public void Update()
		{
			if (Pointer != -1 && Pointer < CodeDataList.Count && isRun == true)
			{
				Pointer = CodeDataList[Pointer].Execute(Pointer);
			}
			else if (Pointer >= CodeDataList.Count)
			{
				Pointer = -1;
				isRun = false;
			}
		}
	}
}
