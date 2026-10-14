# AGENTS.md — AI 助手快速上手上下文

> 本文件供 AI 助手（以及协作者）在开启新对话时快速了解框架结构、当前进行中的工作、已做的决策和踩过的坑。**"当前重构"与"决策"两节变化最频繁，请随进展更新，避免过期误导。**

---

## 1. 项目是什么

**WorldTree**：作者"闪电黑客"的个人 C# 游戏框架（2022.5 起）。核心思路——**一切皆节点**、树状结构 + 对象池自动回收、反射自动注册的**接口式事件系统（法则 Rule）**。可跨平台：核心脱离 Unity，也能在纯 .NET / Console / Avalonia / WPF / WinForm 运行。大量依赖 Roslyn 源生成器 + 分析器。

---

## 2. 架构速览

### 对象三层
- `CoreObject`（启动期对象：LogManager / RuleManager / IdManager / 池管理器。`RuleManager` 在单位池之前建好）
- `Unit`（池化自由对象，有 `Id` + `InstanceId`，不挂树。容器和 `UnitBsonDocument` 直接实现 `IUnit`）
- `Node`（挂树上的业务对象。`INode : IUnit`，`Id` / `InstanceId` 定义在 `IUnit` 上）

### 引导链
```
WorldTreeCore（全局唯一核心）
 └─ WorldLine（世界线 = 根节点，一条线一个线程）
     ├─ ReferencedPoolManager / RuleBroadcastManager / WorldTimeManager
     ├─ WorldHeart（心跳线程，Unity 侧是 UnityWorldHeart）
     ├─ WorldContext（SynchronizationContext，跨线程投递）
     └─ MainWorld（业务入口节点）
```

### 核心概念
- **Branch 分支**：树的组织结构（Child/Component/Field/Temp/ListNode/Generic）。节点用 `ChildOf<>`/`ComponentOf<>`/`FieldOf<>`/`AsBranch<>`/`AsRule<>` 等**空标记接口**声明约束，源生成器据此生成强类型方法。
- **Rule 法则**：以接口为键的事件系统，启动时反射扫描 `IRule` 实现类自动注册。支持多播/多态/泛型/逆变/返回值/异步。生命周期（Awake/Enable/Disable/Add/Remove/Graft/Cut/Update/Serialize…）也是法则。
- **RuleExecutor 法则执行器**：替代委托，双模式遍历（+1 连续 / +2 奇偶），遍历中增删时序稳定。
- **对象池**：UnitPool / NodePool / ReferencedPool（只收 `INodeData`，按雪花 `Id` 反查）。
- **IdManager**：实例 Id（进程位 + 递减）+ 数据 UID（雪花：14位进程 | 30位秒时间戳 | 20位偏移）。
- **时间**：RealTimeManager（NTP 校时）、WorldTimeManager（游戏时间/帧）、CascadeTicker（二进制时间轮）。
- **TreeTask**：自研 async/await（`[AsyncMethodBuilder]` + 自写状态机 + 内联令牌）。
- **序列化**：TreePack（生成器主导二进制）/ TreeData（自描述、支持循环引用）/ TreeCopy（无反射深拷贝）。
- **源生成器** `Framework/SourceGeneratorCore` + **分析器** `Framework/AnalyzerCore`（生成 ISendRule/ICallRule 接口、NodeBranchHelper、INodeProxy、序列化/拷贝等）。

### 关键文件地图
| 路径 | 说明 |
|---|---|
| `Framework/Core/` | .NET 侧核心源码（`WorldTree.Core.csproj`，`DOTNET` 宏） |
| `Framework/Module/` | 模块（csproj 链接 Unity 的 Module 源码）：DataBase(LiteDB)/Input/Loader/NeuralNetwork/TreeTween/Window |
| `Framework/SourceGeneratorCore/`、`Framework/AnalyzerCore/` | 源生成器 + 分析器 |
| `Unity/Assets/Scripts/WorldTreeFramework/WorldTree/Module/` | 平台无关模块（被 .NET Module csproj 链接） |
| `Unity/Assets/Scripts/WorldTreeFramework/UnityEnvironment/RunTime/` | `WorldTree.CoreUnity`（Unity 运行时环境） |
| `Unity/Assets/Scripts/WorldTreeFramework/UnityEnvironment/Editor/` | `WorldTree.UnityEditor`（编辑器工具，用 Odin/Sirenix） |
| `Unity/Assets/Scripts/HotUpdate/{HotFix,HotFixUnity,Config}/` | HybridCLR 热更程序集 |
| `Unity/Assets/Scripts/AOT/{GameEntry,Plugins}/` | AOT 入口 + 第三方 dll（Sirenix/LiteDB/Sqlite） |
| `Console/Server/` | 纯 .NET 服务器示例（`Program.cs` 里 `WorldTreeCore.Create(...)`） |
| `Unity/ProjectSettings/HybridCLRSettings.asset` | HybridCLR 配置（hotUpdateAssemblies / patchAOTAssemblies） |
| `Unity/.../Editor/ThirdParty/HybridCLR/` | HybridCLR 自定义脚本（HybridCLREditor / AOTGenericReferences 生成物） |
| `Unity/.dshexec/` | AgentExec 文件通道（command.json → result.json，Unity 代码执行） |
| `Unity/Assets/Scripts/AOT/GameEntry/_MonoTest/UnityMCP/Editor/AgentExec.cs` | 文件通道的 Unity 代码执行器 |

---

## 3. 当前重构（进行中）

**目标**：把 Core 程序集从 Unity 移出到外部 `Framework` 程序集，编译成 `WorldTree.Core.dll` 给 Unity 引用（.NET 端与 Unity 端共用一份）。

**已完成 ✅**
- `Framework/Core/WorldTree.Core.csproj`：`net6.0 → netstandard2.1`；`AssemblyName → WorldTree.Core`；加 `AfterTargets=Build` 的 `CopyToUnity` 目标（拷 dll/pdb 到 `Unity/Assets/Plugins/WorldTree/`）；加 `System.Runtime.CompilerServices.Unsafe` 包。
- 5 个 asmdef 改为 `overrideReferences: true` 显式引用 `WorldTree.Core.dll`：`Module`(+LiteDB.dll)、`CoreUnity`、`HotFix`、`HotFixUnity`(+Unsafe.dll)、`Config`。
- `WorldTree.UnityEditor.asmdef` 补 4 个 Sirenix dll（Attributes/Editor/Utilities/Utilities.Editor）。
- 删除了 Unity 里的 Core 源码目录 `Unity/Assets/Scripts/WorldTreeFramework/WorldTree/Core/`（含 asmdef + meta）。
- HybridCLR：`WorldTree.Core` 从 `hotUpdateAssemblies` 移除（变 AOT）；`WorldTree.Config` 补进 `hotUpdateAssemblies`。
- **MyGenAll 全流程通过**：`WorldTree.Core.dll` 已被自动纳入 `AOTGenericReferences` 的 `PatchedAOTAssemblyList`（自动检测，无需手动加）。
- **Unity 编译 + 启动已验证通过。**
- 删除 MemoryPack：插件源码、`MemoryPackTest`、`MemoryPack.dll.bytes`、`HotFix`/`HotFixUnity` 的 asmdef 引用、`Unity.sln` / csproj 里的工程引用与生成器。`AOTGenericReferences.cs` 和 `link.xml` 里的 MemoryPack 条目已去掉（下次 MyGenAll 会整文件重写）。
- `System.Runtime.CompilerServices.Unsafe.dll` 已从 `AOT/Plugins/MemoryPack/` 挪到 `Assets/Plugins/`（与 NLog/CommandLine 放一起）。`AotDlls/System.Runtime.CompilerServices.Unsafe.dll.bytes` 保留。

**待办 / 待验证 ⏳**
- 下一步接入 Luban（插件已添加）。
- `AsyncDelay` 已修好：`WorldTimeManager` 补了 `AddTimer` / `AddTimerDelay`（真实时间 Utc Ticks），`AsyncDelay` 接上去，`TimeOut` 因此能生效。
- **TreeTask 测试代码**：`Unity/Assets/Scripts/HotUpdate/HotFix/Server/_Test/TreeTaskTest.cs` + `TreeTaskTestRule.cs`，挂在 `DotNetInitRule.OnAddRule`。7 个用例（嵌套 await / AsyncYield 逐帧 / AsyncDelay / Task 桥接 / TreeTaskLink / 令牌取消 / 异常传播），跑 `Console/Server/Bin/App.exe` 看日志，已实测全绿。
- TreeTask 仍未处理：BCL `Task` 失败不会 `SetException`（`INode.GetAwaiter(Task)` 一律 `SetResult`，`TreeTaskLink` 只 `LogError` 然后一直 Pending），取消被当成成功（`GetResult` 对 `Canceled` 不抛异常，业务只能自己查令牌）。
- **WorldTask** 已并列放在 `Framework/Core/WorldTask/`，继承 `Unit`，不挂树。顶层要 `.Coroutine()` / `.Coroutine(token)`。同步等待和状态机用完还池；普通任务完成时不还池。生成器仍输出 `TreeTask`。控制台测试已跑过。真实时间延迟目前走 `WorldTimeManager.AddTimerCallback`（委托槽位）。这是临时接法：定时器仍应回到 `CascadeTicker`，任务保持轻对象。等 `INode : IUnit`、法则能打到单位之后再接，不要继续加厚这套委托定时器。
- `patchAOTAssemblies` 当前已是空列表。删 MemoryPack 后编译、MyGenAll、启动已通过。

**编号与法则补充（进行中）**

已完成，`WorldTree.Core` 编译通过：
- `IUnit` 增加 `Id`、`InstanceId`（`[TreeDataIgnore]`）。属性补在 `Unit`、`UnitDictionary` / `UnitList` / `UnitHashSet` / `UnitQueue` / `UnitConcurrentQueue` / `UnitStack` / `UnitSortedDictionary`、`UnitBsonDocument`。`UnitDictionary` 另加 `[TreeCopyIgnore]`。
- 单位在 `NewUnit` / `PoolGetUnit` 的 `RefreshUnitId` 里发号，`OnCreate` 之前：`InstanceId = GetId()`，`Id = InstanceId`。直接 `new` 出来的单位不发号，`Id` 为 0。
- 节点在 `NewNode` / `PoolGetNode` 的 `RefreshNodeId` 里发号并 `SupportNodeRule`，`OnCreate` 之前。每次换 `InstanceId`。普通节点 `Id = InstanceId`。`INodeData` 非反序列化时 `Id = GetUID()`；`IsSerialize` 时不动 `Id`，等字段循环写回存档里的 `Id`。
- `NodeData` 不再重写 `OnCreate`。`INodeProxy.OnCreate` 留空方法：直接实现 `INode` 的类型会生成对它的转发，签名不能删。
- `INode : IUnit`。`Id` / `InstanceId` 只留在 `IUnit` 上。`Node` 自己的属性实现它们。生命周期约束仍在 `INode`。

还没做：
- `NodeRuleRef`、`RuleListExecutor` 的存储改成 `IUnit`。生成器只放宽 `SendRule` / `CallRule`（含异步、引用）的下界；`AddNode`、`NodeOf`、分支生成器保持 `INode`。
- `NodeRef`、`CascadeTicker` 先不动。不引入 `IUnitData`，名字也不对调。
- `PoolGetUnit<T>` 的约束现在也能匹配节点。节点仍走 `PoolGetNode`，不要拿单位池去取节点。

---

## 4. 已做的决策

- **目标框架**：`netstandard2.1` 单一目标（注：Unity 工程是 .NET Framework 模式 `apiCompatibilityLevel:3`，但 netstandard2.1 的 Core.dll 实测编译+启动通过）。
- **程序集名**：统一为 `WorldTree.Core`（与 Unity 生成的 csproj 一致）。
- **Unity 引用方式**：保留 asmdef 骨架 + `overrideReferences: true` 显式引用 DLL（而非 Auto Reference）。
- **Core 的 HybridCLR 角色**：AOT（固定不热更）。改 Core 代码需重新出包。
- **DLL 落点**：`Unity/Assets/Plugins/WorldTree/`。
- **编号只在 `New` / `PoolGet` 分配，不在 `OnCreate`。** 反序列化是先 `PoolGetNode(true)` 再写字段，`IsSerialize` 在进去之前已经设好。`Branch` / `ListNodeBranch` / `Rattan` 重写了 `OnCreate` 且不调 `base`；容器继承的是 `List` / `Dictionary`，不能再继承 `Unit`。
- **不单开 `IUnitData`。** 继承是 `IUnit` → `INode` → `INodeData`。`INode` 继续表示树上的对象，`IUnit` 继续表示自由对象，不要改名。
- **节点实例的 `SupportNodeRule` 跟发号走。** 序列化、`TreePack`、`TreeCopier` 里的调用留下：读档要在创建对象之前先补上反序列化法则；`int`、数组、`UnitList` 不走 `PoolGetNode`。`PoolGetUnit` 不要调用它，每次都会拿写锁。

---

## 5. 踩过的坑（重要）

- **`overrideReferences: true` 会切断对项目里所有预编译 DLL 的自动引用**。凡改成 true 的 asmdef，必须把它（含源生成器生成的那部分代码）直接 `using` 到的每个预编译 dll 都显式列进 `precompiledReferences`。踩过：Sirenix、Unsafe。
- **netstandard 缺 `System.Runtime.CompilerServices.Unsafe`**（net6.0 内置，netstandard 要引 `System.Runtime.CompilerServices.Unsafe` 包）。Core 编译报错"Unsafe 不存在"就是它。
- **Core.dll 现在有 3 个外部依赖，Unity 里都得有**：`NLog.dll`、`CommandLine.dll`、`System.Runtime.CompilerServices.Unsafe.dll`（都在 `Assets/Plugins/`）。删别的插件时不要把 Unsafe.dll 一起清掉。
- **源生成器/分析器与目标框架无关**（跑在 Roslyn 编译器进程里），netstandard2.1 完全能用它们。
- **HybridCLR：`AOTGenericReferences.cs` 是自动生成物，别手改**（`GenericReferenceWriter` 生成）。真正生效的 `PatchedAOTAssemblyList` 是**自动检测**热更代码的泛型用法得出的；`patchAOTAssemblies`（设置）在本版本基本没接进生成流程。
- **HybridCLR 社区版**：预编译 dll 放 Assets = AOT（不可热更）。热更 dll 是 `CompileDll` 从**源码**编译的，光把 dll 丢 `Temp/Bin/Debug` 无效。旗舰版 DHE 才能热更 AOT dll。
- **HybridCLR：AOT 程序集不能引用热更程序集**。`WorldTree.Config` 一直在 `HotUpdate/` 目录却漏加进 `hotUpdateAssemblies`，MyGenAll 的 IL2CPP 构建报 `Failed to resolve assembly: WorldTree.HotFix`。已把 Config 加进 hotUpdateAssemblies 解决。
- **环境限制**：pwsh 沙箱不能写盘（`SetNamedSecurityInfoW failed (Win32 5): grantWrite`）；agent 现在的 shell 是 POSIX 风格（git-bash），`dotnet build`、直接跑 `Console/Server/Bin/App.exe` 都能跑通，反倒是 `Get-ChildItem` 这类 pwsh cmdlet 会 command not found。
- **BCL `Task` 完成后的恢复要等下一次心跳 tick**。`INode.GetAwaiter(Task)` / `TreeTaskLink` 完成后都走 `WorldContext.Post`，而 `WorldContext.requestQueue` 由 `UpdateRule` 每帧排空一次（整队排空）。心跳 `frameTime=1000ms` 时，一个 300ms 的 `Task` 实测 1003ms 才恢复。别拿它做毫秒级计时。
- **`INodeProxy.OnCreate` 不能删。** 直接实现 `INode` 的类型由 `INodeProxyGenerator` 生成 `INodeProxyRule.OnCreate(this)`。编号和 `SupportNodeRule` 已不在这里。
- **`SupportNodeRule` 每次都进写锁**，类型已经支持也一样。热路径（每次 `PoolGetUnit`）不要加。读档的法则查找发生在 `PoolGetNode` 之前，`ReadValue` 里的调用不能省。
- **生成器静态集合不能跨程序集共享**。`RuleSupplementHelper` 等用静态字典收集再 `AddSource`。MyGenAll 的 IL2CPP Player 构建用 `/shared` 编译器服务器并行编 `CoreUnity`/`Module`/`HotFix`，字典被串台后，Module 的 `InputBindSupplement` 会被写进 CoreUnity（CoreUnity 不引用 Module，CS0246），接着 `GenerateStripedAOTDlls failed`。编辑器/VS 分开编所以能过。`SourceGeneratorBase.Execute` 已加锁串行。改 `Unity.Analysis.dll` 后要重启 Unity，否则旧编译器服务器还握着旧 dll。程序集名单在 `UnityTool/Analysis/ProjectGeneratorSetting.cs`，和 asmdef 是对得上的；`Framework/SourceGeneratorCore/ProjectGeneratorSetting.cs` 是 .NET 侧（`WorldTree.Core` / `Module`），别和 Unity 那份混。

---

## 6. 工具链：Unity 代码执行通道（AgentExec）

本机有一条文件通道，可在运行中的 Unity 编辑器里**编译并执行 C# 代码**（注入代码走内存编译，无需 Unity 重编译）。

- **执行器**：`Unity/Assets/Scripts/AOT/GameEntry/_MonoTest/UnityMCP/Editor/AgentExec.cs`（`[InitializeOnLoad]`，`EditorApplication.update` 每 0.5s 轮询）。
- **命令**（写）：`Unity/.dshexec/command.json` —— `{ "id": "<唯一id>", "code": "<C#代码>" }`。
- **结果**（读）：`Unity/.dshexec/result.json` —— `{ "id", "success", "result", "error", "logs" }`，用 `id` 对账。
- **流程**：写 command.json → Unity ~0.5s 后 `CSharpCodeProvider` 内存编译 + 执行 → 写 result.json、删 command.json。
- **能力**：注入代码可引用所有已加载程序集（含 WorldTree 框架）。
- **限制**：`CSharpCodeProvider` 是老编译器（别用太新 C# 语法）；遍历程序集引用时要 `if (asm.IsDynamic) continue;` 跳过动态程序集。
- 注：原 UnityMCP（WebSocket/MCP 通道）已废弃清空，仅保留此文件通道。

## 7. 维护约定

- 架构（第 2 节）相对稳定，重构影响结构时更新。
- **第 3 节"当前重构"、第 4 节"决策"变化最频繁**，每完成/变更一项就同步。
- 新踩的坑追加到第 5 节。
- 保持精炼：目标是 2 分钟内能 re-onboard。
