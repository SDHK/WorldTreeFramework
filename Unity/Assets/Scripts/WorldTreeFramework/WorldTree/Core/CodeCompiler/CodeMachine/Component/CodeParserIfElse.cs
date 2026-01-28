using System;

namespace WorldTree
{
	/// <summary>
	/// 代码解析器：条件分支 
	/// </summary>
	public class CodeParserIfElse : Node
		, ComponentOf<CodeMachine>
		, AsRule<Awake>
	{
		/// <summary>
		/// 条件结构体 
		/// </summary>
		public struct IfElseData
		{
			/// <summary>
			/// 条件判断事件 
			/// </summary>
			public Func<bool> Check;
			/// <summary>
			/// Else地址 
			/// </summary>
			public int Else;
			/// <summary>
			/// End地址 
			/// </summary>
			public int End;
		}

		/// <summary>
		/// 所属代码执行机 
		/// </summary>
		public CodeMachine machine;

		/// <summary>
		/// 条件数据列表
		/// </summary>
		public UnitList<IfElseData> dataList;

		/// <summary>
		/// IF地址栈 
		/// </summary>
		public UnitStack<int> AddressStack;

		/// <summary>
		/// 解析If指令 
		/// </summary>
		private int ParserIf(int pointer, int address)
		{
			IfElseData data = dataList[address];
			if (data.Check.Invoke())
				return pointer + 1;
			else if (data.Else != 0)
				return data.Else + 1;
			else if (data.End != 0)
				return data.End;
			else
				this.LogError("If指令解析错误，缺少Else或End地址");
			return pointer + 1;
		}

		/// <summary>
		/// 解析Else指令 
		/// </summary>
		private int ParserElse(int pointer, int address) => dataList[address].End;

		/// <summary>
		/// 解析End指令 
		/// </summary>
		private int ParserEnd(int pointer, int address) => pointer + 1;

		/// <summary>
		/// 获取IF代码数据 
		/// </summary>
		public CodeData GetIF(Func<bool> check)
		{
			IfElseData data = new()
			{
				Check = check,
				Else = 0,
				End = 0,
			};
			dataList.Add(data);
			AddressStack.Push(dataList.Count - 1);
			return new CodeData()
			{
				Parser = ParserIf,
				Address = dataList.Count - 1,
			};
		}
		/// <summary>
		/// 获取Else代码数据 
		/// </summary>
		public CodeData GetElse()
		{
			if (AddressStack.Count == 0)
				this.LogError("Else指令获取错误，缺少对应的If指令");
			// 获取If地址
			int ifAddress = AddressStack.Peek();
			// 设置If的Else地址
			IfElseData data = dataList[ifAddress];
			data.Else = machine.CodeDataList.Count - 1;
			dataList[ifAddress] = data;
			return new CodeData()
			{
				Parser = ParserElse,
				Address = ifAddress,
			};
		}

		/// <summary>
		/// 获取End代码数据 
		/// </summary>
		public CodeData GetEnd()
		{
			if (AddressStack.Count == 0)
				this.LogError("End指令获取错误，缺少对应的If指令");
			// 获取If地址
			int ifAddress = AddressStack.Pop();
			// 设置If的End地址
			IfElseData data = dataList[ifAddress];
			data.End = machine.CodeDataList.Count;
			dataList[ifAddress] = data;
			return new CodeData()
			{
				Parser = ParserEnd,
				Address = ifAddress,
			};
		}
	}

	public static class CodeParserIfElseRule
	{
		class Awake : AwakeRule<CodeParserIfElse>
		{
			protected override void Execute(CodeParserIfElse self)
			{
				self.GetParent(out self.machine);
				self.Core.PoolGetUnit(out self.dataList);
				self.Core.PoolGetUnit(out self.AddressStack);
			}
		}
		class Remove : RemoveRule<CodeParserIfElse>
		{
			protected override void Execute(CodeParserIfElse self)
			{
				self.dataList.Dispose();
				self.dataList = null;
				self.AddressStack.Dispose();
				self.AddressStack = null;
				self.machine = null;
			}
		}
	}
}
