# AGENTS.md — AI 助手快速上手上下文

> 供新对话了解框架怎么用。先看文首「宿主」。已定下的能力写在第 2 节，写成用法，不写完成记录。进行中的事在第 3 节，坑在第 4 节。

---

## 宿主（先读）

你在 **VSCode 的 OMP 控制台**里。OMP 是 oh-my-pi（命令 `omp`）。窗口是 VSCode 集成终端，真正跑命令的是 OMP 自带的 POSIX shell，不是 PowerShell，也不是 git-bash。换外层终端不会改变工具行为。shell 的 shim 和 `rg -c` 坑在第 4.2 节。

这些东西会话启动时自己带上。架构只维护第 2 节，不要再抄一份「始终生效」的规矩。

| 东西 | 放哪 | 干什么 |
|---|---|---|
| 上下文 | 仓库根 `AGENTS.md`（本文件）；旁边还可有 `CLAUDE.md`。额外文件名用配置 `contextFiles.extra` | 每会话都读。用法写第 2 节 |
| MCP | 仓库 `.omp/mcp.json` | 启动时加载。本仓库挂了 Unity：`unity_exec` / `unity_refresh` / `unity_logs` / `unity_select`（`127.0.0.1:8099/mcp`）。改过 json 要 `/mcp reload`。细节在第 5 节 |
| 技能 | `omp skill`。不加 `-g` 用项目的 `skills.json`，`-g` 用用户全局 | 用到才加载的流程。本仓库没装。不要用技能复述架构。`--no-skills` 关掉 |
| TTSR 规则 | 项目 `.omp/rules/*.md`，全局 `~/.omp/agent/rules` | 带条件的警告，不是说明书。输出或工具调用对上了才插进来。`/omfg <抱怨>` 从一次失误生成；`omp ttsr test` / `omp ttsr scan` 试。`--no-rules` 关掉。同一种失误重复出现、第 2 节又没拦住，再加一条 |
| 子代理 | 工具 `task`。`omp agents unpack` 到 `~/.omp/agent/agents`，`--project` 到 `./.omp/agents` | 并行探查。本仓库没有自定义代理 |
| 配置 | `~/.omp/agent/config.yml` | 模型角色、`bash.patterns`。`git commit*` 已 `deny`，`yolo` 下也生效。模型 / 额度看 `MODEL-PROVIDERS.md`，这里不写 |

常用内建工具：`read` / `edit` / `write` / `grep` / `glob` / `bash`，`ast_grep`（已开，支持 csharp），`task`，`web_search`。`lsp` 在本仓库是空的，见第 4.2 节。不许 `git commit`，见第 6 节。

---

## 1. 项目是什么

**WorldTree**：作者「闪电黑客」的个人 C# 游戏框架（2022.5 起）。核心是树状节点、对象池，以及以接口为键的法则（Rule）。核心脱离 Unity，也能在 .NET / Console / Avalonia 上跑。大量依赖 Roslyn 源生成器和分析器。

---

## 2. 架构

### 对象
- `CoreObject`：启动期对象（Log、Rule、Id、池）。`RuleManager` 在单位池之前建好。
- `IUnit`：池里的自由对象。只有 `Id`、`InstanceId`，不挂树，不执行法则。容器和 `UnitBsonDocument` 直接实现它。
- `INode : IUnit`：挂在树上的业务对象。父节点、分支、生命周期和法则都在这一层。

`NodeRef<N>`（`N : IUnit`）可以指向单位或节点。只有 `INodeData` 按 `Id` 从引用池找回；纯单位还池后引用变空。

### 引导链
```
WorldTreeCore（全局唯一核心）
 └─ WorldLine（世界线 = 根节点，一条线一个线程）
     ├─ ReferencedPoolManager / RuleBroadcastManager / WorldTimeManager
     ├─ WorldHeart（心跳线程，Unity 侧是 UnityWorldHeart）
     ├─ WorldContext（SynchronizationContext，跨线程投递）
     └─ MainWorld（业务入口节点）
```

### 分支与池
节点用 `ChildOf<>` / `ComponentOf<>` / `FieldOf<>` / `AsBranch<>` / `AsRule<>` 这些空接口声明约束，生成器据此写出强类型方法。分支有 Child / Component / Field / Temp / ListNode / Generic。

池是 UnitPool / NodePool / ReferencedPool。引用池只收 `INodeData`。节点用 `PoolGetNode`，不要 `PoolGetUnit`。

### 编号
在 `New` / `PoolGet` 里发号，不在 `OnCreate`。单位 `Id = InstanceId`；直接 `new` 出来的单位不发号，`Id` 为 0。普通节点同样，每次取出换 `InstanceId`，并补上 `SupportNodeRule`。`INodeData` 用雪花 `Id`；反序列化时先保住存档里的 `Id`。

`INodeProxy.OnCreate` 留空转发，编号不在这里。`Branch` / `ListNodeBranch` / `Rattan` 重写了 `OnCreate` 且不调 `base`，它们继承容器，不能再继承 `Unit`。

### 法则
声明继承 `ISendRule` / `ISendRuleAsync` / `ICallRule` / `ICallRuleAsync` / `ISendRefRule` 的接口后，同命名空间自动生成空的抽象基类，不要手写：

```csharp
public abstract class 接口名Rule<N, 接口自己的泛型…>
    : 基类名<N, 接口名, 基接口的泛型…>
    where N : class, INode, AsRule<接口名>
{ }
```

基类名是基接口去掉开头的 `I`。例如 `interface TreeTaskSetResuIt : ISendRule` 得到 `TreeTaskSetResuItRule<N> : SendRule<N, TreeTaskSetResuIt>`。业务类继承它，或直接继承 `SendRule<节点, 接口, 参数…>`，`Execute` 写在子类上。

再标 `IMethodRule` 会生成 `self.接口名(...)`。`ISourceGeneratorIgnore` 不生成。`Awake` 按参数个数另生成。带 `Invoke` / `Execute` 的 `SendRule` / `CallRule` 基底也按参数个数生成，不是每个业务接口一份。

法则和执行器只服务 `INode`：`where N : class, INode`，`Invoke(INode self)`。执行器参数名是 `node`。`NodeRuleRef` 存节点和法则，`RuleUnicast.TargetNode` 是 `NodeRef<INode>`。生命周期（Awake / Add / Remove / Enable / Update 等）也只挂在节点上。

找不到实现时 `TrySendRule` / `TryCallRule` 返回 false，`Call` 返回 `default`。异步没有实现时 `await self.TreeTaskCompleted()`。`TreeTaskCompleted` 是 `IUnit` 扩展：节点用自己，纯单位用 `World`，任务挂到 `TreeTaskManager`。

### 异步
`async TreeTask` / `async TreeTask<T>`。任务是节点，挂在 `TreeTaskManager`（`World` 的组件，只有 `Awake`）的临时分支上。顶层用 `.Coroutine()` 或 `.Coroutine(token)`。

节点上写 `self.AsyncDelay` / `self.AsyncYield` / `self.GetAwaiter`。延迟走 `CascadeTicker`，`AddTimerDelay` 用真实时间的 Utc Ticks，目标是 `INode`。`CascadeTickerData.Node` 是 `NodeRef<INode>`。

令牌是节点 `TreeTaskToken`，而且是**内联令牌**：不写进方法参数（C# 的 `CancellationToken` 要逐层传参，这里没有）。`async` 方法的构建器在每次 `await` 时，把当前任务的令牌 `SetToken` 到这次等的 awaiter 上。`SetToken` 再顺着 `RelevanceTask` 往下传，遇到已经有令牌的任务就停，所以内层新建的令牌不会被外层盖掉。业务要看当前令牌就 `await self.TreeTaskTokenCatch()`，不要自己加参数。顶层用 `Coroutine(token)` 或 `chain.SetToken(token)` 把令牌放上链。`parent.Add(child)` 后，父令牌取消时子令牌一起取消。`AddToken` 是另一条：新令牌既能取消老令牌，也能被老令牌取消。令牌取消不抛，`GetResult` 把 `Canceled` 当完成。

BCL `Task` 失败或取消则 `SetException`：只包一层的 `AggregateException` 拆开，取消抛 `TaskCanceledException`。`GetResult` 会把任务还池，所以同一个任务不要既 `Coroutine()` 又 `await`。

队列锁是 `await self.AsyncLock(key)`。同一个 key 排队，`Dispose` 凭证后下一位才继续。还在排队时取消，结果是 null。等待节点还池前要清掉 `Result`，节点池不抹字段。

### 其余
- 执行器替代委托，遍历中可以增删。
- 时间还有 RealTimeManager（校时）和 WorldTimeManager（游戏时间 / 帧）。
- 序列化：TreePack（二进制）/ TreeData（自描述，可循环引用）/ TreeCopy（深拷贝）。
- 源生成器在 `Framework/SourceGeneratorCore`，分析器在 `Framework/AnalyzerCore`。

### 程序集
`Framework/Core` 编成 `WorldTree.Core.dll`（`netstandard2.1`），拷到 `Unity/Assets/Plugins/WorldTree/`。Unity 的 asmdef 用 `overrideReferences` 显式引用这份 DLL。HybridCLR 里 Core 是 AOT，不热更。依赖 `NLog.dll`（5.4.0，程序集 5.0.0.0）、`CommandLine.dll`（2.9.1，程序集 2.9.1.0）、`System.Runtime.CompilerServices.Unsafe.dll`，都在 `Assets/Plugins/`。改 Core 要重新出包。改生成器后重编 `Unity.Analysis.dll`，并重启 Unity。

### 配置导出
导表在 `ExcelTool`，跑 `Excel/配置导出.bat`。本机要有 `Microsoft.NETCore.App` 8。脚本先检查 Game 和 Start 的 client、server，四次都通过才清空 `ExcelTool/Output`，再导出。失败是 `exit /b 1`，没有 `pause`。产物不进 Unity，也不进 `Console/Server`。

分组只有 `c`（客户端）和 `s`（服务器）。共用表写成 `c,s`。不建 `cs`、不建 `e`，不导 json，没有编辑器代码。命名空间是 `WorldTree`。数据是 Luban 的 bin（`.bytes`），代码目标是 `cs-bin`。`manager` 不能空：空了会生成没有类名的 `.cs`。

| | 入口 | 聚合类 | 现在有的表 |
|---|---|---|---|
| Game | `Excel/Excels/_Game.conf`，登记 `_Tables.xlsx` | `Tables` | `ClientProbe`（`c`）、`ServerProbe`（`s`）、`SharedProbe`（`c,s`）。表类用这个名字，记录类加 `Data`。这是探针，不是业务表 |
| Start | `Excel/Excels/StartConfig/Localhost/_Start.conf` | `StartTables` | `WorldLineConfig`，记录类 `WorldLineConfigData`，索引 `LineId`。只有 Localhost，表是空的 |

两边的 C# 都写进 `Output/CSharp/{c,s}`，所以聚合类不能同名。Start 导出时 `cleanUpOutputDir=0`，避免清掉已经写出的 Game 代码。bytes 分开：`Output/Bytes/{c,s}/GameConfig` 和 `Output/Bytes/{c,s}/StartConfig/Localhost`。

外壳仍写死 `Create(0, MainWorld, …)`。世界线 0 是模型主线程，不进 Start 表；`LineId` 从 1 起。`Options.ConfigPath` 默认 `StartConfig/Localhost`，只被 `SetOptions` 存下，没有人读。`ConfigManager` 挂在 `WorldLine` 上，还没有读 bytes 的加载器。主线程起来之后才该读 Start，再 `Create` 别的线；Start 只在主线程读一次。

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
| `ExcelTool/` | Luban 导表。入口 `Excel/配置导出.bat`，产物在 `Output/`。用法见「配置导出」 |
| `Unity/ProjectSettings/HybridCLRSettings.asset` | HybridCLR 配置（hotUpdateAssemblies / patchAOTAssemblies） |
| `Unity/.../Editor/ThirdParty/HybridCLR/` | HybridCLR 自定义脚本（HybridCLREditor / AOTGenericReferences 生成物） |
| `Unity/Assets/Scripts/AOT/GameEntry/_MonoTest/UnityMCP/Editor/` | Unity MCP 服务器（`unity_exec` / `unity_refresh` / `unity_logs` / `unity_select`，HTTP `127.0.0.1:8099/mcp`）；客户端配置在 `.omp/mcp.json` |

不单开 `IUnitData`。继承是 `IUnit` → `INode` → `INodeData`，这三个名字和 `NodeRef`、`NodeRule` 都不要改。默认法则不要挂到 `IUnit`。

---

## 3. 进行中

### Luban
导表的用法在第 2 节「配置导出」。下一步是阶段 2：在 `WorldTree.UnityEditor`（`Unity/Assets/Scripts/WorldTreeFramework/UnityEnvironment/Editor/WorldTree.UnityEditor.asmdef`）写扩展，把**客户端**那套产物拷进 Unity。不要照参照工程的 `Export.bat` 去拷。这一阶段不改服务器怎么读配置，外壳也不改成读表拉线。

对照管线只看 `E:\Storage\Daily\BaiduNetdisk\WX20220921\config\`。不要照 HeadMaster 的 `Luban.ClientServer.exe`。

#### 项目结构（阶段 2 前设计，暂不搬文件）

框架、产品、工具拆成四个平级仓库，放同一父目录（可选叫 `WorldTreeWorkspace`）。这是设计蓝图，现在的实际目录还没搬。

```
（同一父目录，父目录名随便、不归 git）
WorldTreeFramework/        框架核心仓库（库 1），只放纯 C#
  Core/                    核心：节点 / 法则 / 异步 / 序列化 / 池
  AnalyzerCore/            分析器主体
  SourceGeneratorCore/     源生成器主体
  Module/                  平台无关模块

Project/                   项目分类目录（不归 git）
  _Template/               演示 + 模板仓库（库 2），各平台平级
    Console/               控制台宿主
    Avalonia/              Avalonia 宿主
    WinForm/               WinForm 宿主
    WPF/                   WPF 宿主
    Godot/                 Godot 宿主（预留，走 .NET 模式）
    Unity/                 Unity 宿主工程（含 UnityTool 分析器打包）
    ExcelTool/             导表演示（含 Luban 引擎，自包含）
  xxx游戏项目1/            从 _Template clone 派生（独立库）
    Tables/                策划 SVN：xlsx 源表 + conf
    ExcelTool/             配置导出.bat + Output/
    Server/                控制台服务器
    Unity/                 Unity 客户端

ToolProject/               工具分类目录（不归 git）
  ConfigTool/              配置工具
  OperateLinkFile/         VS 插件
```

关键约定：

- 依赖不靠目录嵌套，靠 csproj `ProjectReference` 指向 `WorldTreeFramework/Core`（Unity 侧用编好的 `WorldTree.Core.dll`）。框架稳定后再上 NuGet。
- `WorldTreeFramework/` 只放纯 C#（核心、模块、分析器主体）。依赖具体平台的适配不归框架：CoreUnity 依赖 UnityEngine，是 Unity 适配，跟 `Project/_Template/Unity` 宿主走；`Unity.Analysis` 打包层在 `Unity/UnityTool`，配置 asmdef + 拷 Plugins，每个 Unity 项目一份。
- `_Template` 里各平台和共享件**平级**（不分 Project/Tool 层），这样复制到项目时相对路径不变。各平台的 InputBindTool 保留在各自宿主目录下，不合并。
- Luban 引擎在 `ExcelTool/Luban`，`ExcelTool` 自包含，自己放 `conf` / `xlsx` / `bat`。
- 表代码分离：xlsx 源表归策划（SVN，Excel 二进制 diff 不了，靠 lock 防并发），工具和产物归程序员（git）。个人项目先不建 SVN，但源表目录和产物目录保持分开，将来拆就是挪源表 + 改 bat 路径。
- git 粒度：`WorldTreeFramework` / `Project/_Template` 各一个库；`Project` / `ToolProject` 是分类目录，里面每个项目/工具从 `Project/_Template` clone 派生独立库。
- 命名全英文，`_Template` 不叫 `Demo`。

---

## 4. 踩过的坑（重要）

> 分三段：**4.1 工程与程序集**（仓库结构/依赖/程序集）、**4.2 本机环境与工具用法**（agent shell / 编辑器工具 / 编码约定）、**4.3 运行时与源生成器**。已解决的只留"症状 → 解法"，未解决的留判据。**模型 / provider / 额度 / 凭据相关不在这里**，全部见仓库根 `MODEL-PROVIDERS.md`。

### 4.1 工程与程序集

- **`overrideReferences: true` 会切断项目里所有预编译 DLL 的自动引用**：改成 true 的 asmdef，必须把代码（含源生成器生成的那部分）直接 `using` 到的每个 dll 显式写进 `precompiledReferences`。踩过 Sirenix、Unsafe。
- **netstandard 少 `System.Runtime.CompilerServices.Unsafe`**（net6.0 才内置）→ 引包即可。Core 报"Unsafe 不存在"就是它。
- **`WorldTree.Core.dll` 的 3 个外部依赖，Unity 里都得有**：`NLog.dll`、`CommandLine.dll`、`System.Runtime.CompilerServices.Unsafe.dll`（均在 `Assets/Plugins/`）。**Unity 按强名称精确校验版本**——NLog 必须 5.4.0（程序集 `5.0.0.0`）、CommandLineParser 必须 2.9.1（程序集 `2.9.1.0`），与 `WorldTree.Core.csproj` 的包版本一致；旧版会让 Core.dll **拒绝加载**。清插件时别把 Unsafe.dll 一起删。
- **源生成器/分析器与目标框架无关**（跑在 Roslyn 编译器进程里）→ netstandard2.1 照用。
- **HybridCLR 三条**：① `AOTGenericReferences.cs` 是自动生成物**别手改**——真正生效的 `PatchedAOTAssemblyList` 由"热更代码的泛型用法"**自动检测**得出，`patchAOTAssemblies` 设置在本版本基本没接进生成流程。② 社区版里 **Assets 下的预编译 dll = AOT，不可热更**；热更 dll 必须由 `CompileDll` 从**源码**编，光把 dll 丢 `Temp/Bin/Debug` 无效（旗舰版 DHE 才有 AOT 热更）。③ **AOT 程序集不能引用热更程序集**——`WorldTree.Config` 曾在 `HotUpdate/` 下却漏进 `hotUpdateAssemblies`，MyGenAll 的 IL2CPP 构建报 `Failed to resolve assembly: WorldTree.HotFix`（已修）。
- **`_Template` 路径是脆弱的**：`_Template` 是占位名，clone 后会被重命名或直接当项目开发，任何引用 `_Template` 内部内容的相对路径（如 `..\..\Luban`）都会断。运行时依赖必须放进自包含目录（如 Luban 引擎放 `ExcelTool/Luban`，`配置导出.bat` 用 `..\Luban`），跨仓库引用只允许指向独立的平级仓库（如 `WorldTreeFramework`）。
### 4.2 本机环境与工具用法

- **agent 的 shell 是 OMP 自带 POSIX 风格 shell**（不是 git-bash、也不是 PowerShell）：`$0`/`$SHELL` 空、`uname` 报 `MS/Windows`、`Get-ChildItem` 等 pwsh cmdlet 不存在；`ls`/`cat`/`sed`/`find`/`rg`/`grep` **全是内建 shim**（`type rg` → "shell builtin"，`rg --version` 却谎称 15.1.0），只有 `dotnet`/`git`/`python`/`code` 是真 exe。**唯一会咬人的 shim 坑：`rg -c <pat> <dir>` 走 GNU 语义，会给每个扫到的文件打印 `path:0`**（实测 `rg -c "self as INode" .` 输出 1978 行、其中 1972 行是 `:0`）→ 极易误读成"0 命中"，叠上输出截断更糟。查命中用 `rg <pat> <path> | wc -l` 或 `grep` 工具（**grep 工具本身正常**：能搜到被 `.gitignore` 屏蔽的 `Framework/Core/obj`，也能搜 `.idea/**` 等隐藏文件；唯一真会返回 0 的情况是**模式里带字面引号**）。
- **宿主终端只决定继承的环境变量，不参与工具行为**（VS2026 Developer PowerShell 与 VSCode 集成终端实测逐字节一致：cwd / shell 身份 / `uname` / `rg` 输出 / grep 命中数）→ "换终端"解释不了工具行为差异。**渲染只认 `mermaid`**（渲染成 ASCII 方框箭头）：`svg` 代码块渲染成**空白**（整块连文字一起消失，不是降级成源码）→ 本终端画图只用 `mermaid`，精确布局/几何用表格或文字。
- **写盘无限制**（更正旧记录）：工作区、`%TEMP%`、`C:\` 根、用户目录实测可写；`dotnet build` + NuGet 拉包正常。构建时的 `NU1900 无法加载源 …/vulnerability.base.json` 只是**漏洞审计源**取不到，**不是还原失败**，别当网络问题。旧记录 `SetNamedSecurityInfoW failed (Win32 5): grantWrite` 已不适用。
- **`xd://lsp` 在本仓库空转**（`No language servers configured`；本机无 `csharp-ls`/`OmniSharp`，`~/.dotnet/tools` 空）→ 系统提示里"改导出符号前先查 references"只能退回 grep。可用候选：VSCode C# 扩展自带的 `~/.vscode/extensions/ms-dotnettools.csharp-*/.roslyn/roslyn-language-server.exe`（stdio 握手实测能返回合法 initialize，但重复运行不稳定，**未接进 OMP**）。
- **`git commit` 有硬拦截但 `eval` 能绕过（护栏 ≠ 边界）**：`bash.patterns` 把 `git commit*` / `git -C * commit*` 设为 `deny`（`yolo` 下仍生效、也管 headless 子代理、复合命令分段同样命中；`git status`/`log` 等读操作不受影响），但**管不到 `eval` 里 spawn 的子进程**（实测 `subprocess.run(["git","commit","--dry-run"])` 畅通）→ 所以"不许自动提交"实际靠 §6 的约定 + 这条护栏；要变真边界得设 `tools.approval.eval: prompt|deny`（代价是每次确认或彻底失去 eval），作者已选护栏方案。
- **9 个 `.cs` 曾是 GBK，2026-10-15 已全部转成 UTF-8（无 BOM）**（全仓 720 个 `.cs` 里就这 9 个）。发现途径：`ast_grep` 的 parse issue（`stream did not contain valid UTF-8`，它会**静默跳过**该文件）+ `read` 直接拒收非 UTF-8（`Cannot read binary file … not valid UTF-8 text`）。**有实际影响的只有两个**：`Avalonia/AvaloniaMVVMTest/Views/MainWindow.axaml.cs`、`Unity/Assets/Scripts/HotUpdate/HotFix/Server/_Test/MainWorldRule.cs` —— 它们的**字符串字面量里有中文**，按 UTF-8 读会变 U+FFFD（运行期文案乱码）；其余 7 个的非 ASCII 只在注释里，所以一直没暴露。修法：按 gbk 解码 → 按 UTF-8 **无 BOM** 重写。**另注**：仓库里有 **502 个 `.cs` 是 UTF-8 带 BOM**（`.idea/encodings.xml` 的 `addBOMForNewFiles: with BOM`），属既有状态、**别去"修"**；`core.autocrlf=true`（blob 存 LF、工作区 CRLF）→ **比对内容前先归一化行尾**，否则纯编码改动会被误判成整文件改动。
- **模型 / provider / 额度 / 凭据的事一律看仓库根 `MODEL-PROVIDERS.md`**（本地文件，已 gitignore，不随仓库分发）：角色配置与坑、Cursor 命名坑、Claude 在 Copilot/Cursor 的不对称、Premium 计费机制与重测法、fallback 链写法、`omp usage` 失效、provider 断开、自定义 provider 写法与 `~/.env` 凭据语义、OpenDesign 订阅通道（§2.1）、DeepSeek 三条通道的经济性（§3）、`alibaba-token-plan`（已禁用）四条坑。**本节不复述**（2026-10-16 从本节挪过去）。
- **Python 子进程默认按 cp936 解码，会把非 ASCII 读成乱码**：本机 `locale.getpreferredencoding()` = `cp936`，`subprocess.run(..., text=True)` 不加 `encoding` 时读子进程的 UTF-8 中文会变乱码 —— 实测模型回 `收到` 而显示成 `鏀跺埌`（`'收到'.encode('utf-8').decode('gbk')` 可还原）。**读子进程输出一律显式 `encoding="utf-8"`**（shell 里的 `cat`/`rg` 不受影响）。
- **`ast_grep` 已启用**（原生实现、支持 `csharp`、免安装）。注意它的输出在 hashline 编辑模式下只渲染到模式的字面前缀：`class $N` 的命中行显示成 `*46|class`，不带上下文 → 当**定位器**用（拿 file+line），内容再 `read` 看。
- **`配置导出.bat` 的中文行会吃掉下一行**：文件是 UTF-8、无 BOM、CRLF。代码页 65001 下，一行中文紧挨着下一行时，cmd 会把下一行拆开。每一行中文后面空一行。行尾 `^` 续行，`^` 后面不能有空格。
- **Luban 的 `manager` 留空会生成 `.cs`**：类声明变成 `public partial class `，文件名也是空的。Game 写 `Tables`，Start 写 `StartTables`。两套 C# 落在同一个 `Output/CSharp/{c,s}`，同名 `partial` 的构造器对不上。Start 导出必须 `cleanUpOutputDir=0`，否则清掉已经写出的 Game 代码。
- **`cs-bin` 模板里命名空间内的类少一层缩进**：`ExcelTool/Luban/Templates/cs-bin` 的 `tables.sbn`、`table.sbn`、`bean.sbn` 已在命名空间内部整体加了 4 格。换官方模板时这层会丢。
### 4.3 运行时与源生成器

- **BCL `Task` 完成后的恢复要等下一次心跳 tick**：`INode.GetAwaiter(Task)` / `TreeTaskLink` 完成后都走 `WorldContext.Post`，而该队列由 `UpdateRule` **每帧整队排空**。心跳 `frameTime=1000ms` 时，一个 300ms 的 `Task` 实测 **1003ms** 才恢复 → 别拿它做毫秒级计时。
- **`Console/Server` 必须在 `Bin` 下启动**：`WorldLog` 只在 `../ProjectConfig/NLog` 存在时才建 NLog logger（配置 `Console/Server/ProjectConfig/NLog/NLog.config`，日志 `Console/Server/Logs/Log.txt`，同时打到控制台）。在仓库根跑时 logger 为 null → `Log`/`Error` 空引用，而心跳线程上这个 NRE 会**盖住真正的异常**。本 shell 不把当前目录当命令路径（`App.exe` / `./App.exe` 都 command not found）→ 用 `Console/Server/Bin/App.exe`（cwd 仍设 Bin）；`WorldHeart` 不退出，看完日志 `taskkill /F /IM App.exe`。
- **`INodeProxy.OnCreate` 不能删**：直接实现 `INode` 的类型由 `INodeProxyGenerator` 生成 `INodeProxyRule.OnCreate(this)`（编号和 `SupportNodeRule` 已不在这里）。
- **`SupportNodeRule` 每次都进写锁**（类型已支持也一样）→ 热路径（每次 `PoolGetUnit`）不要加；但读档的法则查找发生在 `PoolGetNode` **之前**，`ReadValue` 里的调用不能省。
- **法则接口对应的 `XxxRule<N>` 不要手写** —— `RuleSupplementGenerator` 会生成同名抽象基类，再写一个就 CS0101；要跳过生成就给接口标 `ISourceGeneratorIgnore`。
- **生成器静态集合不能跨程序集共享**：`RuleSupplementHelper` 等用静态字典收集再 `AddSource`，而 MyGenAll 的 IL2CPP 构建用 `/shared` 编译器服务器**并行**编 `CoreUnity`/`Module`/`HotFix`，字典串台后会把 Module 的 `InputBindSupplement` 写进 CoreUnity（CoreUnity 不引用 Module → CS0246），接着 `GenerateStripedAOTDlls failed`（编辑器/VS 分开编所以能过）。已让 `SourceGeneratorBase.Execute` 加锁串行。**改 `Unity.Analysis.dll` 后必须重启 Unity**，否则旧编译器服务器还握着旧 dll。程序集名单：Unity 侧 `UnityTool/Analysis/ProjectGeneratorSetting.cs`（与 asmdef 对得上），.NET 侧 `Framework/SourceGeneratorCore/ProjectGeneratorSetting.cs`（`WorldTree.Core` / `WorldTree.Module`），**别混**。
- **改程序集 `AssemblyName` 要同步 `ProjectGeneratorSetting` 名单**：`SourceGeneratorBase` 用 `Compilation.AssemblyName` 匹配 `ProjectGeneratorSetting` 里的名单，`AssemblyName` 改了不同步名单，生成器整包跳过、法则基类全不生成（报一串 CS0246 找不到 `XxxRule<>`）。改 `AssemblyName` 后记得改名单。
- **跨平台视图组件别用跟 uGUI 同名的裸名**：WorldTree 的 `Text`/`Image`/`Button` 跟 `UnityEngine.UI.Text/Image/Button` 重名。之前 Module 是 asmdef（AOT asmdef 不引用它，不冲突）；Module 移出编成 dll 放 `Assets/Plugins` 后，`overrideReferences:false` 的 asmdef 自动引用它，类型解析冲突（`GameEntry` 报 `error CS1061: 'Text' does not contain 'text'`）。已改名 `ViewText`/`ViewImage`/`ViewButton`。

---

## 5. 工具链：Unity 代码执行（MCP）

在运行中的 Unity 编辑器里**编译并执行 C# 代码**（注入代码走内存编译，不需要 Unity 重编译，也不需要用户点一下 Unity）。

- **服务端**：`Unity/.../UnityMCP/Editor/UnityMcpServer.cs`（`[InitializeOnLoad]`）：生命周期 + 主线程编组 + 路由分发。
- **端点**：`http://127.0.0.1:8099/mcp`（JSON-RPC over 手写最小 HTTP，裸 `TcpListener`）。
- **客户端配置**：仓库 `.omp/mcp.json` → `{"mcpServers":{"unity":{"type":"http","url":"http://127.0.0.1:8099/mcp"}}}`。
- **鉴权**：默认无（只绑 `127.0.0.1`）。往 `.unitymcp/mcp-token.txt` 写个非空字符串即启用，之后要求 `Authorization: Bearer <token>`。
- **在 OMP 里**：工具以 `unity_exec` / `unity_refresh` / `unity_logs` / `unity_select` 暴露（路由 `xd://mcp__unity_exec`）。**MCP 在会话启动时加载**：改过 `mcp.json`、或在别的会话里新建过工具，已运行的会话要 `/mcp reload` 才看得到。

### 四个工具

| 工具 | 参数 | 行为 |
|---|---|---|
| `unity_exec` | `code`（必填） | 主线程执行一段 C#（填方法体，可 `return`）；返回值 `ToString()` 回传，**本次执行期间**的 `Debug.Log` 一起带上 |
| `unity_refresh` | 无 | **响应先写回**，再执行 `AssetDatabase.Refresh()`（刷新触发域重载、服务器随之重启，实测 ~3s 回来） |
| `unity_logs` | `count?`(默认 50) / `type?`(all/log/warning/error) / `clear?` | 读**常驻**环形缓冲（500 条，静态字段）—— 能拿到调用**之前**的日志；**域重载会清空** |
| `unity_select` | `target`（必填）/ `ping?` | 选中并 Ping 一个 GameObject（名称或层级路径）；`GameObject.Find` 未命中时按名字搜已加载**场景**对象（含未激活） |

### 分层（2026-10-16 拆过）

| 文件（都在 `Unity/.../AOT/GameEntry/_MonoTest/UnityMCP/Editor/`） | 职责 |
|---|---|
| `UnityMcpHttpTransport.cs` | 传输：裸 `TcpListener` + 最小 HTTP，**不懂 MCP** |
| `UnityMcpServer.cs` | 生命周期（域重载停/起）+ 主线程编组 + 协议分发（`initialize`/`ping`/`tools/list`/`tools/call`） |
| `Tools/UnityMcpToolRegistry.cs` | 工具清单 —— **新增工具在此加一行** |
| `Tools/UnityMcpTool.cs` | 工具基类（名字/描述/schema/执行 + schema 与取参小工具） |
| `Tools/UnityExecTool.cs` 等 4 个 | 每个工具一个类 |
| `CSharpExecutor.cs` | 内存编译 + 执行（`unity_exec` 用；引用集规则见下） |
| `UnityMcpLogBuffer.cs` | 日志环形缓冲（`unity_logs` 用） |
| `UnityMcpPaths.cs` | 运行时路径（`.unitymcp/`：`mcp-token.txt`、诊断用 `refs.json`） |

**设计要点**：连接在**后台线程**接收（不依赖编辑器循环），执行**编组回主线程**（`EditorApplication.update` 排队消费，等待上限 120s → 超时回 504 而不是静默）—— 执行必须主线程这点换传输也改不掉，MCP 换来的是**确定性反馈**与工具化。

**`unity_refresh` 的时序**：**响应先写回，再执行 `AssetDatabase.Refresh()`** —— 刷新会触发域重载、服务器随之被杀，靠 `[InitializeOnLoad]` + `AssemblyReloadEvents.beforeAssemblyReload` 停/起，监听在 `EditorApplication.delayCall` 里绑定。实测重载后 **3 秒**恢复监听。

**`unity_select` 的补充**：未命中时按名字搜已加载**场景**对象（含未激活，用 `scene.IsValid()` 排除预制体等资源），命中多个时返回候选路径让调用方改用完整路径；选中后 `EditorGUIUtility.PingObject` 在 Hierarchy 高亮。实测 `Sphere` / `Main Camera`（含空格）/ 嵌套 `Canvas/Background` 均成功，并用 `unity_exec` 读回 `Selection.activeGameObject` 验证真的改了编辑器状态。

**MCP 实测（2026-10-16，全绿）**：`initialize` → `protocolVersion/capabilities.tools/serverInfo`；`tools/list` → **4 工具**，schema 含 `properties` 与 `required`；`unity_exec` → 主线程执行 + 能访问 WorldTree 类型 + `Debug.Log` 捕获；`unity_select` → 命中/未命中两条路径都验过（`isError` 正确）；`unity_logs` → 常驻缓冲能读到调用之前的启动日志；`unity_refresh` → 重载后自动回来，且**删掉 `AgentExec.cs` 后重载仍正常**（`AgentExec` / `CSEditorHelper` 反射扫描命中 0）。协议边界：GET → **405**（让客户端静默禁用 SSE 后台监听）、通知 → **202**、`ping` → `{}`、未知方法 → **-32601**、坏 JSON → **-32700**、未知工具/缺参 → `isError:true`、DELETE → **204**。**OMP 端到端**：新会话挂上并调用成功（路由 `xd://mcp__unity_exec`）。

**`UnityMCPConnection.cs` / `UnityMCPWindow.cs` 是更早的 WebSocket 通道遗产**：Unity 主动连出 `ws://localhost:8080`，协议**自造**（`selectGameObject` / `togglePlayMode` / `executeEditorCommand` + 出站 `{"type":"log"}`），**不是 MCP**，且缺外部那一半（8080 上的桥进程）。**这两个文件已于 2026-10-16 删除**（整文件 `/* */` 注释掉的 733 行死代码，删前确认全仓无引用；`Tools/UnityMCP/Debug Window` 菜单项随之消失）。**已决定不加状态窗口（2026-10-16）**：实测 24 次粗暴断开（空连接 ×5／半个请求头 ×5／谎报 `Content-Length` ×3／在途 RST／延时 RST ×5）**日志一条没多、服务器照旧** —— MCP 是**纯服务端拉取**（无持久连接、无心跳、无重连、不回调客户端），没有"客户端一断就刷屏"的机制。所以不需要窗口；功能上真该补的是**仪表**（请求数／耗时／最近调用 → 做成 `unity_status` 工具 + 每次调用一行 `Debug.Log`），不是 UI。另：`UnityMCP.Editor.asmdef` 是 `overrideReferences: true`，`precompiledReferences` 只有 `Newtonsoft.Json.dll`（**不含 `WorldTree.Core.dll`** —— 注入代码靠运行时动态引用）；它原本引用的 GUID `478a2357cc57436488a56e564b08d223` **全仓任何 `.meta` 里都没有**（含 `Library/PackageCache`）→ 悬空引用，**已于 2026-10-16 删掉**（`"references": []`，删后编译 + 启动正常）。注：这个悬空引用**没有**在 `Editor.log` 里产生任何警告（旧记录写"Unity 会警告"，未复现）。

**限制与坑**：

- **`CSharpCodeProvider` 是老编译器**（Unity 2021 自带）：别用太新 C# 语法；返回值被 `ret?.ToString()` **拍平成一个字符串**，要结构化数据自己在注入代码里 `JsonConvert.SerializeObject`。
- **引用集规则**（`CSharpExecutor`，2026-10-16 二分定位）：`CSharpCodeProvider` **隐式带了一套核心 BCL**，所以**显式引用 `mscorlib` 会重复定义** —— 症状是 ~105 条 `CS1685` + 一旦触碰跨门面类型就 `CS0433`（`The imported type 'System.AppDomain' is defined multiple times`）。**但 `netstandard` 不能跳** —— `WorldTree.Core` 等 netstandard2.1 程序集的类型要靠它解析身份，跳掉会得到 `CS0012`（`System.IDisposable ... is defined in an assembly that is not referenced`）。最终做法：按**程序集简单名去重** + 跳过 `mscorlib` / `System.Runtime` / `System.Private.CoreLib`（**保留 `netstandard`**），遍历时 `if (asm.IsDynamic) continue;`。修复后验收：`GetAssemblies().Length` → 233、`typeof(WorldTree.WorldTreeCore).FullName` 可解析、LINQ + `Debug.Log` 捕获正常、反射 `RuleManager` 正常，且**警告 0**。
- **`.unitymcp/refs.json` 是引用白名单**：放一个程序集简单名数组即可临时限制引用集（不用改代码就能二分引用集问题；**用完记得删掉**）。
- **没有看门狗**：注入死循环会让编辑器永久卡死，通道也救不回来。长任务拆小。
- **可以自己触发重编译**：`unity_refresh` 之后判据是 `Library/ScriptAssemblies/UnityMCP.Editor.dll` 的 mtime 更新（实测 3~7s）。**编译失败时旧 dll 仍在内存里，服务器照常活着**，改完再刷即可 —— 这是安全的调试姿势。
- **端口 8099 只绑回环**，无鉴权时也只对本机开放。

**2026-10-16 已删除文件通道**（原 `AgentExec.cs` + `Unity/.dshexec/`）：两条通道共用同一个执行器没必要，MCP 有确定性反馈与工具化；`CSharpExecutor.cs` 就是从它里面提出来的。删前实测：`AgentExec` / `CSEditorHelper` 类型在已加载程序集里命中 **0**，写进 `command.json` 无人消费。

## 6. 维护约定

- **模型不许自动提交**：绝不运行 `git commit`（含 `git -C … commit` 等任何等价形式）。改动做完就停下报告，由作者审阅后**手动提交**。硬保险：`~/.omp/agent/config.yml` 的 `bash.patterns` 已把 `git commit*` 设为 `deny`，`yolo` 模式下同样生效。
- 用法写进第 2 节，写成怎么用，不写完成记录。
- 宿主和 OMP 提供的东西写文首。不要把架构抄进技能或始终生效的规则；TTSR 只在同一种失误重复出现时加。
- 还没做完的事写第 3 节。新坑追加到第 4 节。
- 保持精炼：目标是 2 分钟内能 re-onboard。
