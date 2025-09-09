/****************************************

* 作者： 闪电黑客
* 日期： 2025/9/3 11:10

* 描述： 任务执行器
* 
* 作用是将一系列任务按设定执行

*/
using System;
using System.Collections.Generic;

namespace VM
{
	#region 核心

	/// <summary>
	/// 任务执行器 - 负责执行任务
	/// </summary>
	public partial class TaskExecutor : IDisposable
	{


		/// <summary>
		/// 解析地址储存器
		/// </summary>
		private List<Action<int>> ParserPaths = new List<Action<int>>();

		/// <summary>
		/// 任务地址储存器
		/// </summary>
		private List<int> TaskPaths = new List<int>();

		/// <summary>
		/// 指令指针
		/// </summary>
		public int Pointer = -1;

		/// <summary>
		/// 运行标记
		/// </summary>
		public bool isRun = false;

		/// <summary>
		/// 结束标记
		/// </summary>
		public bool isEnd = false;

		/// <summary>
		/// 执行器启动
		/// </summary>
		public void Run()
		{
			Pointer = 0;
			isRun = true;
		}

		/// <summary>
		/// 执行器任务终止
		/// </summary>
		public void Stop()
		{
			Pointer = -1;
			isRun = false;
		}

		/// <summary>
		/// [指令写入]                                  
		/// 注： 任务解析器地址 和 任务地址 合成为一条指令 写入到储存器
		/// </summary>
		/// <param name="ParserPath">任务解析器</param>
		/// <param name="TaskPath">任务地址</param>
		private void TaskAdd(Action<int> ParserPath, int TaskPath)
		{
			ParserPaths.Add(ParserPath);
			TaskPaths.Add(TaskPath);
		}

		/// <summary>
		/// 主要运行进程 [放在主进程Update里]
		/// </summary>
		public void Update()
		{
			if (Pointer != -1 && Pointer < ParserPaths.Count && isRun == true)
			{
				//通过 指令指针 提取 任务解析器 和 任务地址 让解析器运行任务
				ParserPaths[Pointer](TaskPaths[Pointer]);
			}
			else if (Pointer >= ParserPaths.Count)
			{
				Pointer = -1;
				isRun = false;
			}
		}


		public void Dispose()
		{
			Clear();
		}

		public void Clear()
		{
			Stop();
			ParserPaths.Clear();
			TaskPaths.Clear();
			TaskEvents.Clear();
			TaskTimes.Clear();
			Task_IFs.Clear();
			Task_Loops.Clear();
			Loop_Address.Clear();
			IF_Address.Clear();
			_methodCallTasks.Clear();
			_methodDefineEndTasks.Clear();
			_methodParameterNames.Clear();
			Method_ReturnAddress.Clear();
			Method_Address.Clear();
			Method_NameAddress.Clear();
			_parameterStack.Clear();
			_pushTasks.Clear();
			_PopPushVariableTasks.Clear();

			memory.Clear();
			TaskValues.Clear();
			_scopeStack.Clear();
			_scopeParent.Clear();
			_currentScopeId = 0;
			_nextScopeId = 1;
			Pointer = -1;
			isRun = false;
			isEnd = false;

		}
	}

	#endregion

	#region 作用域管理

	public partial class TaskExecutor
	{

		/// <summary>
		/// 作用域栈 - 管理作用域层次
		/// </summary>
		private Stack<int> _scopeStack = new Stack<int>();

		/// <summary>
		/// 当前作用域ID
		/// </summary>
		private int _currentScopeId = 0;

		/// <summary>
		/// 下一个作用域ID
		/// </summary>
		private int _nextScopeId = 1;

		/// <summary>
		/// 作用域父子关系
		/// </summary>
		private Dictionary<int, int> _scopeParent = new();

		/// <summary>
		/// 虚拟内存：作用域，变量名，变量值
		/// </summary>
		private Dictionary<int, Dictionary<string, VarValue>> memory = new();

		/// <summary>
		/// 设置变量值列表（调试用）
		/// </summary>
		private List<(string, VarValue)> TaskValues = new List<(string, VarValue)>();

		/// <summary>
		/// 进入新作用域
		/// </summary>
		public void EnterScope()
		{
			_scopeParent[_nextScopeId] = _currentScopeId;
			_scopeStack.Push(_currentScopeId);
			_currentScopeId = _nextScopeId++;
			memory[_currentScopeId] = new Dictionary<string, VarValue>();
		}

		/// <summary>
		/// 退出当前作用域
		/// </summary>
		public void ExitScope()
		{
			if (_scopeStack.Count > 0)
			{
				// 清理当前作用域的内存
				if (memory.ContainsKey(_currentScopeId))
				{
					memory.Remove(_currentScopeId);
				}
				_scopeParent.Remove(_currentScopeId);

				_currentScopeId = _scopeStack.Pop();
			}
		}

		/// <summary>
		/// 设置变量值（在当前作用域）
		/// </summary>
		public void SetVariable(string name, VarValue value)
		{
			if (!memory.ContainsKey(_currentScopeId))
			{
				memory[_currentScopeId] = new Dictionary<string, VarValue>();
			}
			memory[_currentScopeId][name] = value;
		}

		/// <summary>
		/// 获取变量值（从当前作用域向上查找）
		/// </summary>
		public VarValue GetVariable(string name)
		{
			int scopeId = _currentScopeId;

			// 从当前作用域向上查找
			while (scopeId >= 0)
			{
				if (memory.ContainsKey(scopeId) && memory[scopeId].ContainsKey(name))
				{
					return memory[scopeId][name];
				}

				// 查找父作用域
				if (_scopeParent.ContainsKey(scopeId))
				{
					scopeId = _scopeParent[scopeId];
				}
				else
				{
					break;
				}
			}

			// 变量未找到，返回默认值
			return new VarValue();
		}

		/// <summary>
		/// 检查变量是否存在
		/// </summary>
		public bool HasVariable(string name)
		{
			int scopeId = _currentScopeId;

			while (scopeId >= 0)
			{
				if (memory.ContainsKey(scopeId) && memory[scopeId].ContainsKey(name))
				{
					return true;
				}

				if (_scopeParent.ContainsKey(scopeId))
				{
					scopeId = _scopeParent[scopeId];
				}
				else
				{
					break;
				}
			}

			return false;
		}


		/// <summary>
		/// 解析器：[设置变量]
		/// </summary>
		/// <param name="TaskPath"></param>
		public void Parser_SetVariableEvent(int TaskPath)
		{
			var (name, value) = TaskValues[TaskPath];
			SetVariable(name, value);
			this.Pointer++;
		}

		/// <summary>
		/// 任务：[设置变量] 
		/// </summary>
		public TaskExecutor SetVariableEvent(string name, VarValue value)
		{
			TaskAdd(Parser_SetVariableEvent, TaskValues.Count);
			TaskValues.Add((name, value));
			SetVariable(name, value);
			return this;
		}

		/// <summary>
		/// 解析器：[移动变量] 
		/// </summary>
		public void Parser_MoveVariableEvent(int TaskPath)
		{
			var (fromName, toName) = TaskValues[TaskPath];
			if (HasVariable(fromName))
			{
				var value = GetVariable(fromName);
				SetVariable((string)toName, value);
			}
			this.Pointer++;
		}

		/// <summary>
		/// 任务：[移动变量] 
		/// </summary>
		public TaskExecutor MoveVariableEvent(string fromName, string toName)
		{
			TaskAdd(Parser_MoveVariableEvent, TaskValues.Count);
			TaskValues.Add((fromName, toName));
			if (HasVariable(fromName))
			{
				var value = GetVariable(fromName);
				SetVariable((string)toName, value);
			}
			return this;
		}
	}

	#endregion

	#region 执行

	public partial class TaskExecutor
	{
		/// <summary>
		/// 任务储存器：[委托事件]
		/// </summary>
		private List<Action> TaskEvents = new List<Action>();

		/// <summary>
		/// 任务解析器：[委托事件]                                  
		/// 注：任务执行的具体流程方法                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_Event(int TaskPath)
		{
			//通过 任务地址 来获取 任务 执行。
			TaskEvents[TaskPath]();
			//指令指针+1，跳转执行下一条指令。
			this.Pointer++;
		}

		/// <summary>
		/// 任务：[委托事件]                                  
		/// 注：执行委托任务                                  
		/// </summary>
		/// <param name="Event">委托事件</param>
		/// <returns>任务执行器</returns>
		public TaskExecutor Event(Action Event)
		{
			//指令添加：委托事件解析器地址 ，委托事件任务地址
			TaskAdd(Parser_Event, TaskEvents.Count);
			TaskEvents.Add(Event);//委托写入任务储存器
			return this;
		}
	}

	#endregion

	#region 延时

	public partial class TaskExecutor
	{
		/// <summary>
		/// 任务储存器：[时间]
		/// </summary>
		private List<int> TaskTimes = new List<int>();

		/// <summary>
		/// 时钟：因为是单线程，所以只需要一个时钟就行了
		/// </summary>
		private DateTime Clock = DateTime.MinValue;

		/// <summary>
		/// 任务解析器：[延时]                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_Delay(int TaskPath)
		{
			if (Clock == DateTime.MinValue)
			{
				Clock = DateTime.Now.AddMilliseconds(TaskTimes[TaskPath]);
			}
			else
			{
				if (DateTime.Now < Clock) return;
				Clock = DateTime.MinValue;
				this.Pointer++;
			}
		}

		/// <summary>
		/// 任务：[延时]                                  
		/// </summary>
		public TaskExecutor Delay(int ms)
		{
			TaskAdd(Parser_Delay, TaskTimes.Count);
			TaskTimes.Add(ms);//委托写入任务储存器
			return this;
		}
	}
	#endregion

	#region 分支

	public partial class TaskExecutor
	{
		/// <summary>
		/// IF判断类_结构
		/// </summary>
		private class IF_Struct
		{
			public Func<bool> IF;//IF事件
			public int IF_Else = -1;//Else地址
			public int IF_End = -1;//End地址
		}

		/// <summary>
		/// 任务储存器：[IF判断]
		/// </summary>
		private List<IF_Struct> Task_IFs = new List<IF_Struct>();

		/// <summary>
		/// 地址栈：用于指令写入时，对多层嵌套IF的标记地址匹配
		/// </summary>
		private Stack<int> IF_Address = new Stack<int>();

		/// <summary>
		/// 任务解析器：[IF判断]                                  
		/// 注：任务执行的具体流程方法                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_IF(int TaskPath)
		{
			if (Task_IFs[TaskPath].IF()) this.Pointer++;
			else
			if (Task_IFs[TaskPath].IF_Else != -1)
				this.Pointer = Task_IFs[TaskPath].IF_Else + 1;
			else
			if (Task_IFs[TaskPath].IF_End != -1)
				this.Pointer = Task_IFs[TaskPath].IF_End;
			else
				this.Pointer += 2;
		}

		/// <summary>
		/// 任务解析器：[IF_Eles]                                  
		/// 注：任务执行的具体流程方法                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_IF_Else(int TaskPath)
		{
			this.Pointer = Task_IFs[TaskPath].IF_End;
		}

		/// <summary>
		/// 任务解析器：[IF_End]                                  
		/// 注：任务执行的具体流程方法                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_IF_End(int TaskPath) => this.Pointer++;

		/// <summary>
		/// 任务：[IF判断]                                  
		/// 注：根据委托返回的bool进行标记跳转                                  
		/// </summary>
		/// <param name="IF_Event">委托的bool事件</param>
		public TaskExecutor IF_Event(Func<bool> IF_Event)
		{
			//地址栈添加当前任务地址
			IF_Address.Push(Task_IFs.Count);
			//指令添加：IF判断解析器地址 ，IF判断任务地址
			TaskAdd(Parser_IF, Task_IFs.Count);
			//新建IF类
			IF_Struct iF_Struct = new IF_Struct();
			iF_Struct.IF = IF_Event;//IF事件添加
			Task_IFs.Add(iF_Struct);//写入任务储存器

			return this;
		}

		/// <summary>
		/// 任务：[IF_Else标记]                                  
		/// 注：写入时会更具内部地址栈，匹配最近的IF判断                                  
		/// </summary>
		public TaskExecutor IF_Else()
		{
			//指令添加：IF_Else解析器地址 ，IF判断任务地址
			TaskAdd(Parser_IF_Else, IF_Address.Peek());
			//地址栈获取当前任务，匹配Else位置标记
			Task_IFs[IF_Address.Peek()].IF_Else = this.ParserPaths.Count - 1;
			return this;
		}

		/// <summary>
		/// 任务：[IF_End标记]                                  
		/// 注：写入时会更具内部地址栈，匹配最近的IF判断                                  
		/// </summary>
		/// <returns>任务执行器</returns>
		public TaskExecutor IF_End()
		{
			//地址栈取出当前任务，匹配End位置标记
			Task_IFs[IF_Address.Pop()].IF_End = this.ParserPaths.Count;
			return this;
		}
	}
	#endregion

	#region 循环

	/// <summary>
	/// 循环任务                           
	/// </summary>
	public partial class TaskExecutor
	{
		/// <summary>
		/// Loop循环类_结构
		/// </summary>
		private class Loop_Struct
		{
			public Func<bool> Loop;//Loop事件
			public int Loop_Enter = -1;//Enter地址
			public int Loop_End = -1;//End地址
		}

		/// <summary>
		/// 任务储存器：[Loop循环]
		/// </summary>
		private List<Loop_Struct> Task_Loops = new List<Loop_Struct>();

		/// <summary>
		/// 地址栈：用于指令写入时，对多层嵌套Loop的标记地址匹配
		/// </summary>
		private Stack<int> Loop_Address = new Stack<int>();

		/// <summary>
		/// 任务解析器：[LoopEnter]                                  
		/// 注：任务执行的具体流程方法                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_LoopEnter(int TaskPath)
		{
			if (Task_Loops[TaskPath].Loop())
				this.Pointer++;
			else
				this.Pointer = Task_Loops[TaskPath].Loop_End + 1;
		}

		/// <summary>
		/// 任务解析器：[LoopEnd]                                  
		/// 注：任务执行的具体流程方法                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_LoopEnd(int TaskPath)
		{
			if (Task_Loops[TaskPath].Loop())
				this.Pointer = Task_Loops[TaskPath].Loop_Enter;
			else
				this.Pointer++;
		}

		/// <summary>
		/// 任务解析器：[Loop_End]                                  
		/// 注：任务执行的具体流程方法                                  
		/// </summary>
		/// <param name="TaskPath">任务地址</param>
		private void Parser_Loop_End(int TaskPath)
		{
			this.Pointer = Task_Loops[TaskPath].Loop_Enter;
		}

		/// <summary>
		/// 任务：[Loop_Enter标记]                                  
		/// 注：写入时会更具内部地址栈，匹配最近的Loop_End循环                                  
		/// </summary>
		/// <returns>任务执行器</returns>
		public TaskExecutor Loop_Enter()
		{
			//地址栈添加当前任务地址
			Loop_Address.Push(Task_Loops.Count);
			//新建Loop循环类结构
			Loop_Struct loop_Struct = new Loop_Struct();
			loop_Struct.Loop_Enter = this.ParserPaths.Count;//匹配循环开始位置标记
			Task_Loops.Add(loop_Struct);//写入任务储存器

			return this;
		}

		/// <summary>
		/// 任务：[Loop_End标记]                                  
		/// 注：写入时会更具内部地址栈，匹配最近的Loop_Enter循环                                  
		/// </summary>
		/// <returns>任务执行器</returns>
		public TaskExecutor Loop_End()
		{
			//地址栈取出任务地址，匹配循环退出位置标记
			int Address = Loop_Address.Pop();
			Task_Loops[Address].Loop_End = this.ParserPaths.Count;
			//指令添加：Loop循环退出 解析器地址 ，任务地址
			TaskAdd(Parser_Loop_End, Address);

			return this;
		}

		/// <summary>
		/// 任务：[Loop_Enter循环]                                  
		/// 注：写入时会更具内部地址栈，匹配最近的Loop_End标记                                  
		/// </summary>
		/// <returns>任务执行器</returns>
		public TaskExecutor Loop_Enter(Func<bool> Loop_Event)
		{
			//地址栈添加当前任务地址
			Loop_Address.Push(Task_Loops.Count);
			//新建Loop循环类结构
			Loop_Struct loop_Struct = new Loop_Struct();
			//添加循环判断事件
			loop_Struct.Loop = Loop_Event;
			//匹配循环开始位置标记
			loop_Struct.Loop_Enter = this.ParserPaths.Count;
			//指令添加：Loop循环进入 解析器地址 ，任务地址
			TaskAdd(Parser_LoopEnter, Task_Loops.Count);
			Task_Loops.Add(loop_Struct);//写入任务储存器
			return this;
		}

		/// <summary>
		/// 任务：[Loop_End循环]                                  
		/// 注：写入时会更具内部地址栈，匹配最近的Loop_Enter标记                                  
		/// </summary>
		/// <returns>任务执行器</returns>
		public TaskExecutor Loop_End(Func<bool> Loop_Event)
		{
			//地址栈取出任务地址
			int Address = Loop_Address.Pop();
			//添加循环判断事件
			Task_Loops[Address].Loop = Loop_Event;
			//匹配循环退出位置标记
			Task_Loops[Address].Loop_End = this.ParserPaths.Count;
			//指令添加：Loop循环结尾 解析器地址 ，任务地址
			TaskAdd(Parser_LoopEnd, Address);
			return this;
		}
	}
	#endregion

	#region 方法

	public partial class TaskExecutor
	{

		/// <summary>
		/// 嵌套调用深度限制，防止无限递归
		/// </summary>
		public const int MAX_CALL_DEPTH = 100;

		/// <summary>
		/// 方法调用任务
		/// </summary>
		private List<string> _methodCallTasks = new List<string>();

		/// <summary>
		/// 方法定义结束任务
		/// </summary>
		private List<int> _methodDefineEndTasks = new List<int>();

		/// <summary>
		/// 方法参数定义
		/// </summary>
		private Dictionary<string, List<string>> _methodParameterNames = new Dictionary<string, List<string>>();

		/// <summary>
		/// 地址栈：用于运行时，对Method的标记地址匹配
		/// </summary>
		private Stack<int> Method_ReturnAddress = new Stack<int>();


		/// <summary>
		/// 地址栈：用于指令写入时，Method的标记地址匹配
		/// </summary>
		private Stack<int> Method_Address = new Stack<int>();

		/// <summary>
		/// 方法声明地址
		/// </summary>
		private Dictionary<string, int> Method_NameAddress = new Dictionary<string, int>();


		/// <summary>
		/// 方法调用解析器 - 支持参数栈
		/// </summary>
		private void Parser_MethodCall(int TaskPath)
		{
			string callTaskName = _methodCallTasks[TaskPath];

			// 递归深度检查
			if (Method_ReturnAddress.Count > MAX_CALL_DEPTH)
			{
				throw new StackOverflowException($"方法调用深度超限: {MAX_CALL_DEPTH}");
			}

			if (Method_NameAddress.TryGetValue(callTaskName, out int jumpAddress))
			{
				// 进入新作用域
				EnterScope();

				// 从参数栈弹出参数并设置到作用域变量
				if (_methodParameterNames.TryGetValue(callTaskName, out List<string> paramNames))
				{
					// 创建临时数组存储参数（因为栈是LIFO）
					VarValue[] parameters = new VarValue[paramNames.Count];
					for (int i = paramNames.Count - 1; i >= 0; i--)
					{
						parameters[i] = _parameterStack.Pop();
					}

					// 按正确顺序设置参数
					for (int i = 0; i < Math.Min(paramNames.Count, parameters.Length); i++)
					{
						SetVariable(paramNames[i], parameters[i]);
					}
				}
				else
				{
					// 没有参数定义，直接弹出参数
					for (int i = 0; i < paramNames.Count; i++)
					{
						_parameterStack.Pop();
					}
				}

				Method_ReturnAddress.Push(this.Pointer + 1);
				this.Pointer = jumpAddress + 1;
			}
			else
			{
				throw new InvalidOperationException($"方法 '{callTaskName}' 未定义");
			}
		}

		/// <summary>
		/// 方法返回解析器
		/// </summary>
		/// <param name="TaskPath"></param>
		private void Parser_MethodEnd(int TaskPath)
		{
			// 退出作用域
			ExitScope();

			if (Method_ReturnAddress.Count != 0)
			{
				this.Pointer = Method_ReturnAddress.Pop();
			}
			else
			{
				this.Pointer++;
			}
		}

		/// <summary>
		/// 方法定义解析器 - 跳过方法体到方法结束处
		/// </summary>
		private void Parser_MethodDefine(int TaskPath)
		{
			int endAddress = _methodDefineEndTasks[TaskPath];

			// 如果方法结束地址已设置，直接跳转到方法结束后
			if (endAddress != -1)
			{
				this.Pointer = endAddress + 1;
			}
			else
			{
				// 如果方法结束地址未设置，说明还没有遇到MethodEnd，跳过当前指令
				throw new InvalidOperationException($"方法 定义错误：缺少 MethodEnd 标记");
			}
		}

		/// <summary>
		/// 任务：[方法调用] - 支持参数栈
		/// </summary>
		public TaskExecutor MethodCall(string methodName)
		{
			TaskAdd(Parser_MethodCall, _methodCallTasks.Count);
			_methodCallTasks.Add(methodName);
			return this;
		}

		/// <summary>
		/// 任务：[方法定义] - 支持参数名
		/// </summary>
		public TaskExecutor MethodDefine(string methodName, params string[] parameterNames)
		{
			Method_NameAddress[methodName] = this.ParserPaths.Count;

			if (parameterNames != null && parameterNames.Length > 0)
			{
				_methodParameterNames[methodName] = new List<string>(parameterNames);
			}

			// 将当前方法定义任务索引压入栈，用于后续MethodEnd匹配
			Method_Address.Push(_methodDefineEndTasks.Count);

			// 添加方法定义解析器，用于跳过方法体
			TaskAdd(Parser_MethodDefine, _methodDefineEndTasks.Count);

			//定义方法结束地址初始为-1，表示未设置
			_methodDefineEndTasks.Add(-1);

			return this;
		}

		/// <summary>
		/// 任务：[方法结束]
		/// </summary>
		public TaskExecutor MethodEnd()
		{
			int Address = Method_Address.Pop();
			// 设置对应方法定义的结束地址
			_methodDefineEndTasks[Address] = this.ParserPaths.Count;
			// 添加方法结束解析器
			TaskAdd(Parser_MethodEnd, 0);
			return this;
		}
	}

	#endregion

	#region 返回值和参数栈操作

	public partial class TaskExecutor
	{
		/// <summary>
		/// 传递参数栈 - 用于传递匿名参数
		/// </summary>
		private Stack<VarValue> _parameterStack = new Stack<VarValue>();

		/// <summary>
		/// 推送字面量存储
		/// </summary>
		private List<VarValue> _pushTasks = new List<VarValue>();

		/// <summary>
		/// 推送弹出变量值任务存储
		/// </summary>
		private List<string> _PopPushVariableTasks = new List<string>();

		public VarValue Pop()
		{
			return _parameterStack.Count > 0 ? _parameterStack.Pop() : new VarValue();
		}

		public void Push(VarValue varValue)
		{
			_parameterStack.Push(varValue);
		}

		/// <summary>
		/// 推送字面量参数到栈
		/// </summary>
		public TaskExecutor PushValue(VarValue value)
		{
			TaskAdd(Parser_PushValue, _pushTasks.Count);
			_pushTasks.Add(value);
			return this;
		}

		/// <summary>
		/// 推送变量值到参数栈（支持多个变量）
		/// </summary>
		/// <param name="variableName">变量名</param>
		public TaskExecutor PushVariable(string variableName)
		{
			TaskAdd(Parser_PushVariables, _PopPushVariableTasks.Count);
			_PopPushVariableTasks.Add(variableName);
			return this;
		}


		/// <summary>
		/// 从栈弹出参数到变量
		/// </summary>
		public TaskExecutor PopVariable(string variableName)
		{
			TaskAdd(Parser_PopVariables, _PopPushVariableTasks.Count);
			_PopPushVariableTasks.Add(variableName);
			return this;
		}


		/// <summary>
		/// 推送参数解析器
		/// </summary>
		private void Parser_PushValue(int taskPath)
		{
			_parameterStack.Push(_pushTasks[taskPath]);
			this.Pointer++;
		}

		/// <summary>
		/// 推送变量值解析器
		/// </summary>
		private void Parser_PushVariables(int taskPath)
		{
			string variableName = _PopPushVariableTasks[taskPath];
			// 推送所有指定的变量到参数栈
			VarValue value = GetVariable(variableName);
			_parameterStack.Push(value);
			this.Pointer++;
		}


		/// <summary>
		/// 弹出变量值解析器
		/// </summary>
		private void Parser_PopVariables(int taskPath)
		{
			var value = _parameterStack.Count > 0 ? _parameterStack.Pop() : new VarValue();
			string variableName = _PopPushVariableTasks[taskPath];
			SetVariable(variableName, value);
			this.Pointer++;
		}
	}


	#endregion
}
