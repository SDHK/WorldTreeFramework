using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 代码执行器 
	/// </summary>
	public class CodeExecutor : Node
		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 指令列表
		/// </summary>
		public List<InstructionData> InstructionList = new();

		/// <summary>
		/// 指令指针
		/// </summary>
		public int Pointer = -1;
	}

	/// <summary>
	/// 指令数据
	/// </summary>
	public struct InstructionData
	{
		/// <summary>
		/// 解析器
		/// </summary>
		public NodeRef<INode> Node;

		/// <summary>
		/// 地址 
		/// </summary>
		public int Address;
	}

}
