# AGENTS.md — AI 助手快速上手上下文

> 供新对话了解框架怎么用。已定下的能力写在第 2 节，写成用法，不写完成记录。进行中的事在第 3 节，坑在第 5 节。

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

令牌是节点 `TreeTaskToken`。`SetToken` 顺着关联任务下传，遇到已有令牌就停。`await self.TreeTaskTokenCatch()` 取当前令牌。`parent.Add(child)` 后，父令牌取消时子令牌一起取消。令牌取消不抛，`GetResult` 把 `Canceled` 当完成。

BCL `Task` 失败或取消则 `SetException`：只包一层的 `AggregateException` 拆开，取消抛 `TaskCanceledException`。`GetResult` 会把任务还池，所以同一个任务不要既 `Coroutine()` 又 `await`。

队列锁是 `await self.AsyncLock(key)`。同一个 key 排队，`Dispose` 凭证后下一位才继续。还在排队时取消，结果是 null。等待节点还池前要清掉 `Result`，节点池不抹字段。

### 其余
- 执行器替代委托，遍历中可以增删。
- 时间还有 RealTimeManager（校时）和 WorldTimeManager（游戏时间 / 帧）。
- 序列化：TreePack（二进制）/ TreeData（自描述，可循环引用）/ TreeCopy（深拷贝）。
- 源生成器在 `Framework/SourceGeneratorCore`，分析器在 `Framework/AnalyzerCore`。

### 程序集
`Framework/Core` 编成 `WorldTree.Core.dll`（`netstandard2.1`），拷到 `Unity/Assets/Plugins/WorldTree/`。Unity 的 asmdef 用 `overrideReferences` 显式引用这份 DLL。HybridCLR 里 Core 是 AOT，不热更。依赖 `NLog.dll`、`CommandLine.dll`、`System.Runtime.CompilerServices.Unsafe.dll`，都在 `Assets/Plugins/`。改 Core 要重新出包。改生成器后重编 `Unity.Analysis.dll`，并重启 Unity。

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

不单开 `IUnitData`。继承是 `IUnit` → `INode` → `INodeData`，这三个名字和 `NodeRef`、`NodeRule` 都不要改。默认法则不要挂到 `IUnit`。

---

## 3. 进行中

- 接入 Luban（插件已添加）。

---

## 5. 踩过的坑（重要）

- **`overrideReferences: true` 会切断对项目里所有预编译 DLL 的自动引用**。凡改成 true 的 asmdef，必须把它（含源生成器生成的那部分代码）直接 `using` 到的每个预编译 dll 都显式列进 `precompiledReferences`。踩过：Sirenix、Unsafe。
- **netstandard 缺 `System.Runtime.CompilerServices.Unsafe`**（net6.0 内置，netstandard 要引 `System.Runtime.CompilerServices.Unsafe` 包）。Core 编译报错"Unsafe 不存在"就是它。
- **Core.dll 现在有 3 个外部依赖，Unity 里都得有**：`NLog.dll`、`CommandLine.dll`、`System.Runtime.CompilerServices.Unsafe.dll`（都在 `Assets/Plugins/`）。删别的插件时不要把 Unsafe.dll 一起清掉。
- **源生成器/分析器与目标框架无关**（跑在 Roslyn 编译器进程里），netstandard2.1 完全能用它们。
- **HybridCLR：`AOTGenericReferences.cs` 是自动生成物，别手改**（`GenericReferenceWriter` 生成）。真正生效的 `PatchedAOTAssemblyList` 是**自动检测**热更代码的泛型用法得出的；`patchAOTAssemblies`（设置）在本版本基本没接进生成流程。
- **HybridCLR 社区版**：预编译 dll 放 Assets = AOT（不可热更）。热更 dll 是 `CompileDll` 从**源码**编译的，光把 dll 丢 `Temp/Bin/Debug` 无效。旗舰版 DHE 才能热更 AOT dll。
- **HybridCLR：AOT 程序集不能引用热更程序集**。`WorldTree.Config` 一直在 `HotUpdate/` 目录却漏加进 `hotUpdateAssemblies`，MyGenAll 的 IL2CPP 构建报 `Failed to resolve assembly: WorldTree.HotFix`。已把 Config 加进 hotUpdateAssemblies 解决。
- **Agent shell 不是 git-bash，是 OMP 自带的 shell**。`$0` / `$SHELL` 皆空，`uname` 报 `MS/Windows`，`//FI` 这类写法不做路径转换；`Get-ChildItem` 这类 pwsh cmdlet 仍然 command not found。`ls`/`cat`/`sed`/`find`/`rg`/`grep` **全是 shell 内建 shim**，不是真实可执行（`type rg` → "rg is a shell builtin"；`rg --version` 却谎称 `15.1.0`）；`dotnet`/`git`/`python`/`code` 才是真实 exe。`dotnet build`、直接跑 `Console/Server/Bin/App.exe` 都能跑通。
- **宿主终端只决定继承进来的环境变量，不参与工具行为**。实测对比 VS2026 Developer PowerShell 与 VSCode 集成终端：cwd、shell 身份、`uname`、`rg` 输出、grep 工具命中数**逐字节一致**。差异全在环境变量层：从 VS2026 启动会继承 `VSINSTALLDIR`/`CommandPromptType`（MSVC/MSBuild 在 PATH 上，`dotnet build` 用不到）；从 VSCode 启动则带 `TERM_PROGRAM=vscode`、`VSCODE_INJECTION=1`，且 `TERM=dumb`。**已验证两条：`svg` 代码块渲染成空白**（整块内容连里面的文字一起消失，不是降级成源码文本），**`mermaid` 代码块则正常渲染成 ASCII 方框箭头图**。所以在本终端画图**只用 `mermaid`，不要用 `svg`**；两者都不合适的（精确布局、几何图形）就用表格或文字。这不影响任何工具执行。所以"换终端"不能解释工具行为差异。
- **写盘无限制（更正旧记录）**：工作区、`%TEMP%`、`C:\` 根、用户目录都实测可写；`dotnet build Framework/Core/WorldTree.Core.csproj` 0 错误，NuGet 也能拉新包。构建时的 `NU1900 无法加载源 …/vulnerability.base.json` 只是漏洞审计源取不到，**不是还原失败**，别当成网络问题。旧记录的 `SetNamedSecurityInfoW failed (Win32 5): grantWrite` 已不适用。
- **shim `rg -c` 会制造假零，别用它查命中**。`rg -c <pat> <dir>` 走 GNU `grep -rc` 语义：**遍历到的每个文件都打印 `path:0`**（真 ripgrep 只打印有命中的文件）。实测 `rg -c "self as INode" .` 输出 **1978 行，其中 1972 行是 `:0`**，真正命中只有 6 个文件。再叠上 `shellMinimizer` 与输出截断，一屏全是 `:0`，极易被误读成"0 matches"。查命中用 `rg <pat> <path> | wc -l`（同一模式 → 13 行）或 grep 工具。**grep 工具本身正常**：同一模式 + `path: "."` 返回 11 处；`path` 指到被 `.gitignore` 屏蔽的 `Framework/Core/obj` 也能命中；`.gitignore`、`.idea/**` 这类隐藏文件同样会被搜。唯一真会返回 0 的坑是**模式里带了字面引号**（工具和 shim 都 0）。
- **`lsp` 工具在本仓库空转**：`xd://lsp` 的 `status` → "No language servers configured for this project"。本机没有 `csharp-ls`/`OmniSharp`/`roslyn-ls`，`~/.dotnet/tools` 为空，所以系统提示里"改导出符号前先查 references"在这里只能退回 grep。现成候选：VSCode C# 扩展自带 `~/.vscode/extensions/ms-dotnettools.csharp-*/.roslyn/roslyn-language-server.exe`（Roslyn 5.11.0，需 `Microsoft.NETCore.App 10.0.0`，本机 10.0.11 满足）。实测 stdio 握手能返回合法 initialize 响应（`Content-Length: 3065`），但重复运行不稳定，**尚未接进 OMP**。
- **`smol` / `slow` 已改接 DeepSeek（2026-10-15 已修）**。原先 `~/.omp/agent/config.yml` 只赋了 `default: cursor/claude-opus-4-7-fast`，`smol` / `slow` 未赋值**却不按文档继承 `default`**，一律 `Cursor PROVIDER_ERROR`，连带废掉 `judge` / `judge_batch` 与 `sonic` 子代理（`task` 正常，因为它走 default 系）。现在（2026-10-15 四改，Copilot 额度一天见底后换回 Cursor）：`default: cursor/claude-sonnet-5-high`（1M，$2/$10）、`smol: deepseek/deepseek-v4-flash`（按 token 直付，不碰任何订阅额度）、`slow: cursor/claude-opus-5-5`（1M，$4/$20）。注意 **Cursor 上 `claude-sonnet-5-5` 只有 200K**，1M 的 Sonnet 得用 effort 变体 `claude-sonnet-5-high`。改完**无需重启**：配置文件有监听，`cfg://modelRoles` 立刻可见，三个角色 + `judge` + `sonic` 实测全通。附带好处是杂活不再消耗 Cursor 的月度配额（DeepSeek 是独立 provider，按 token 直接计费；flash 约 $0.30/$1.20 每 M，错峰半价）。
- **`omp usage` 的报表能力退化了（2026-10-15 观察，原因不明）**：早先 Cursor 报三个桶（`Cursor Models` 百分比 / `Other Models` $20 / `On-Demand` $1），Copilot 报 `Premium Requests 93 / 1,500`；同一账号后来变成 Cursor 只剩 `gpt-4 requests · 0 used`、Copilot 变成 `OAuth account — no usage data`（Alibaba/Kimi 一直是 no usage data）。**凭据刷新丢用量读取能力、还是账号状态变化，我区分不了。** 后果：**再也无法在 OMP 里核对这些订阅的剩余额度**，只能去各自网页控制台看。别再依赖 `omp usage` 做配额判断。
- **`claude-opus-5-5` 的首字延迟明显高于 sonnet**：同一探针实测 default(sonnet-5-high) 2.3s / smol(deepseek-flash) 1.9s / **slow(opus-5-5) 6.7s**。放 `slow` 这种低频档是合适的。
- **写角色值时必须用 `omp models find <id>` 验一下 id 真实存在**：最早 `default` 写的是 `cursor/claude-opus-4-7-fast`，它在 catalog 里**不是真实 id**（`omp models find` 无匹配），只能靠模糊匹配落地；而 opus-4-7 的 `-fast` 变体估价是 $30/$150 每 M，非 fast 的 `high` 是 $5/$25 —— 差 6 倍，选错很贵。另注：改 `default` **不影响已在运行的会话** —— 会话级覆盖优先，要新开会话才用新 default。想知道某会话实际跑在哪个模型上，查 `~/.omp/agent/agent.db` 的 `model_usage` 表（`last_used_at` 最靠前的那个即当前会话模型），`model_perf` 表还有每模型的样本数 / TTFT / 生成耗时。
- **账号级自动替补：Copilot → Cursor → DeepSeek（2026-10-15 配好）**：`retry.fallbackChains` = `{"github-copilot/*": ["cursor/grok-4.7-fast", "deepseek/deepseek-v4-pro"], "cursor/*": ["deepseek/deepseek-v4-pro"]}`，两跳目标均实测可达。配合已有的 `retry.enabled`、`modelFallback: true`、`fallbackOn: "any"`，**任何** `github-copilot` 模型请求失败（额度耗尽，以及之前那种 `PROVIDER_ERROR`）都会自动按链切到 Cursor，再不行切 DeepSeek；`fallbackRevertPolicy: cooldown-expiry` 到期后回主模型。**撞墙前主动切也已开**：`retry.usageAwareFallback: true` + `retry.usageReservePolicy: "auto"` —— 剩余额度低于 `retry.usageReservePct`（默认 10%）时不再询问，直接静默切走（同 provider 多账号会先换账号）。注意 fallback **条目**写 `"deepseek/*"` 表示"保留失败模型的 id 只换 provider"，会得到不存在的 `deepseek/grok-4.7-fast`，所以条目必须写具体模型名；`"cursor/*"` 只有在**键**位置才表示"所有 cursor 模型"。
- **`git commit` 有硬拦截，但 `eval` 能绕过（护栏 ≠ 边界）**：`~/.omp/agent/config.yml` 的 `bash.patterns` 把 `git commit*` 与 `git -C * commit*` 设为 `deny`。`deny` 在 `yolo` 下仍然生效，也约束 headless 子代理（两者均实测），且 `true && git commit` 这类复合命令的分段检查同样命中；`git status`/`git log` 等读操作不受影响。**但 `bash.patterns` 管不到 `eval` 里 spawn 的子进程**（实测 `subprocess.run(["git","commit","--dry-run"])` 畅通）。所以"不许自动提交"实际靠 `AGENTS.md` 第 7 节的约定 + 这条工具护栏；要变成真边界需设 `tools.approval.eval: prompt|deny`，代价是每次确认或彻底失去 eval。作者已选护栏方案。
- **9 个 `.cs` 曾是 GBK 编码，已于 2026-10-15 转成 UTF-8（无 BOM）**。发现途径：`ast_grep` 的 parse issue（`stream did not contain valid UTF-8`，且它会**静默跳过**该文件）。`read` 会直接拒绝非 UTF-8 文件（`Cannot read binary file … not valid UTF-8 text`）。编译器按 UTF-8 处理它们，多数非 ASCII 只在注释里，所以一直没暴露。清单（全仓 720 个 `.cs` 里就这 9 个，其余都是 UTF-8）：`Framework/Core/Rule/RuleBaseLifecycle/SerializeRule.cs`（**core 目录，只有中文注释**）、`Avalonia/AvaloniaMVVMTest/Views/MainWindow.axaml.cs`、`Avalonia/Core/View/Control/WindowTitleBar.axaml.cs`、`Avalonia/InputBindTool/Node/View/App.axaml.cs`、`Avalonia/InputBindTool/Node/View/InputBindPage.axaml.cs`、`Unity/Assets/Scripts/HotUpdate/HotFix/Server/Modules/Input/InputDriverMouse.cs`、`Unity/Assets/Scripts/HotUpdate/HotFix/Server/_Test/MainWorldRule.cs`、`VSPlugin/OperateLinkFile/Windows/InputFieldWindow.cs`、`WinForm/InputBindTool/InputBind/Program.cs`。其中 `MainWindow.axaml.cs`（2 处）与 `MainWorldRule.cs`（1 处）的**字符串字面量里有中文** → 按 UTF-8 读会变 U+FFFD，运行时文案乱码。修复方式：按 gbk 解码、按 UTF-8 **无 BOM** 重写。修后已核对四项：与 HEAD 逐字一致（仅编码变化）、全仓 720 个 `.cs` 非 UTF-8 归零、`read` 能正常打开、Core 编译 0 错误。**另注：仓库里另有 502 个 `.cs` 是 UTF-8 带 BOM**（`.idea/encodings.xml` 的 `addBOMForNewFiles: with BOM`），属既有状态、无碍，不要去"修"。还有 `core.autocrlf=true`：git blob 存 LF、工作区是 CRLF，比对内容前先归一化行尾，否则会把纯编码改动误判成整文件改动。
- **`github-copilot` 可用，但"目录里的模型"≠"账号能用的模型"（2026-10-15 实测）**。抽测 14 个：12 个真跑通，包括 `claude-haiku-5.5`、`gpt-6-luna`、`gpt-6-luna-1m`、`gpt-5.6-luna(-1m)`、`gpt-5-mini`、`claude-sonnet-5` / `-5.5`（含 1M 变体）；`gpt-5.4-nano`、`raptor-mini`、`gpt-6-astra-1m`、`claude-fable-5` 返回 400 `model_not_supported`（账号未启用该模型；`enableAllGitHubCopilotModels` 会在 `/login` 时发 `POST /models/{id}/policy {state:"enabled"}`，**重新登录可能解锁更多**）。额度是 Premium Requests，两个要点：① **计数器有明显滞后**，别用"立刻重读没变"判断是否消耗（实测 1 → 22 是隔了几分钟才反映的，17 次请求约花 21 次额度）；② `getCopilotPremiumRequests` 对 **agent 发起的轮次计 0**，只有 user 发起的轮次按 multiplier 计——真实 agentic 工作里大量轮次是 agent 发起的，所以 1500 次远比看起来耐用。另有 <1 的倍率（文档例：`gpt-4o: 0`、`grok-code-fast-1: 0.25`、`claude-haiku-4.5: 0.33`、`claude-opus-4.6: 3`），但 `omp models --json` 不暴露 multiplier，挑模型时只能按文档。用法：`omp -p "..." --model github-copilot/<id>` 可以直接验证某个模型能不能用。**完整实测清单（46 可用 / 48 不支持）+ 价格表 + 计费机制 + 重测方法见仓库根目录 `MODEL-PROVIDERS.md`（本地文件，已加进 `.gitignore`，不会随仓库分发）。** **Claude 在两家之间是不对称的（2026-10-15 实测）**：Copilot 上**只有 Haiku 与 Sonnet 可用**（`claude-haiku-4.5`、`claude-haiku-5.5(-1m)`、`claude-sonnet-5(-1m)`、`claude-sonnet-5.5(-1m)`），**全部 Opus（4.5/4.6/4.7/4.8/4.8-1m/4.8-fast/5/5-1m/5.5/5.5-1m）、全部 Fable、sonnet-4/4.5/4.6 都是 400 `model_not_supported`**；而 **Cursor 上 Claude 全系可用（含 Opus）** —— `cursor/claude-opus-5-5-1m` 与 `claude-opus-5-5` **同价 $4/$20**（1M 版等于白送），`claude-opus-5`/`4.8`/`4.7` 是 $5/$25。所以「要 Opus 只能走 Cursor」。另注 `claude-sonnet-5.5` 的思考档支持到 **`max`**，而 `grok-4.7` 最高只到 `xhigh`。
- **`alibaba-token-plan` 就是阿里官方的千问 Token Plan —— 更正 2026-10-15 早先的误判**。先前我据"域名不是 `aliyuncs.com`、模型里混着 `siliconflow/…`"判定它是"第三方中转站"，**这个结论是错的**，证据不足就下了判断。硬证据：① `qianwenai.com` / `qwencloud.com` / `qianwenaiapi.com` 三个域名**全部**由 `Alibaba Cloud Computing Ltd. d/b/a HiChina`（阿里自有注册商）注册，全部用 `ALIBABADNS` 解析（RDAP 实测）；② `platform.qianwenai.com` 的**用户协议/产品协议/隐私政策托管在 `terms.alicdn.com`**（阿里自有法律条款 CDN）；③ 页面有 **浙ICP备2023034206号-53**；④ 控制台侧边栏与官方国际站 `home.qwencloud.com` **逐项一致**（含相同的 "Token Plan" / 按量付费 / 重复的按量付费条目）。实际端点 `https://maas.qianwenaiapi.com/compatible-mode/v1` 是阿里自有主机（域名建于 2026-04-24，对应"Token Plan 全面升级"）。**结论：没有中转站需要逃离，凭据本来就是官方的。**
- **但 `token-plan.*.maas.aliyuncs.com` 不是这个产品的端点**：实测把现有 key 发到 `token-plan.cn-beijing.maas.aliyuncs.com` 与 `token-plan.ap-southeast-1.maas.aliyuncs.com` 都是 **401 `InvalidApiKey`**，而它自己的 `maas.qianwenaiapi.com` 返回 200 / 176 个模型。文档里那两个 host 属于 QwenCloud 国际或另一套 Token Plan，**region key 不可互换**。所以 `/login alibaba-token-plan` 时选 region 1/2 可能让现有 key 失效 —— 要重登请选 **3（自定义 URL）**，或干脆沿用现有凭据（key 内嵌的 `baseUrl` 会覆盖目录默认）。凭据形态：`data` = `{"key": "<JSON 字符串>", "source": "login"}`，内层是 `{"token":"sk-ws-H.***","baseUrl":"…"}` —— **baseUrl 嵌在 key 里**，所以同 key 重登换不掉端点。查端点方法：`auth_credentials.data` 里正则抽 `https?://` 片段（**只抽 URL，别打印整个 data，那是密钥**）。
- **额度不可见 + 价格不可见，两条都可解决**：① 额度追踪要求凭据里带 `cookie`（从 `cs-data.qwencloud.com` 取的浏览器 Cookie）；当前只有 `{token, baseUrl}`，所以永远 "no usage data"。`/login` 会**可选**提示填 Cookie，或直接往 `agent.db` 那条 `data` 里补 `cookie` 字段（需重启 OMP）。② 价格缺失是因为上游 `/models` 只返回 `{id, object, created, owned_by}`，但**官方定价页是公开可查的**（站点自述"API 价格 按量计费，模型价格透明可查"）。从那儿抄数字填进 `~/.omp/agent/models.yml` 的 `providers.alibaba-token-plan.modelOverrides.<id>.cost` —— **已实测有效**（占位价测试后 `pricingStatus` 变 `fixed`，测完已删除占位文件）。**但附 Cookie 这条路对中国站大概率不成立**：OMP 的抓取逻辑写死 `https://home.qwencloud.com/tool/user/info.json` + `https://cs-data.qwencloud.com/data/api.json?product=sfm_bailian&...`；实测 `home.qwencloud.com` 该路径 **200（存在）** 而 `platform.qianwenai.com` 对应路径 **404（不存在）**，两个站后端不同、会话不通用（`cs-data.qianwenai.com` 存在但路径形状不同）。所以拿 `qianwenai.com` 的 Cookie 去问 `qwencloud.com` 的接口基本拿不到数据 —— **额度只在网页控制台看**最省事。另注：Cookie 是会话凭据（等同账号访问权），**不要贴进对话**；真要交，走 `omp browser-relay install` + `omp browser-relay` 让中继直接读取，值不落对话记录。
- **断开 provider 只能靠会话内 `/logout`**：`omp --help` 的子命令表里**没有 `logout`**（实测 `grep -c logout` → 0）；`omp auth` 只有 `export`/`import`，且 import 是**合并**不是替换 —— 导出一份去掉某 provider 的 JSON 再导入，**删不掉**旧凭据。要删只能：会话内 `/logout` 选 provider，或直接删 `agent.db` 的 `auth_credentials` 行（不推荐，绕过应用层）。
- **`/login alibaba-token-plan` 的 region 选项**：`1` = 国际（`token-plan.ap-southeast-1.maas.aliyuncs.com`）｜`2` = 中国北京（`token-plan.cn-beijing.maas.aliyuncs.com`）｜`3` = 自定义 URL。**选 1/2 前先确认 key 属于哪一套** —— `qianwenai.com` 那套 Token Plan 的 key 在这两个 host 上实测 401，别盲选。备选千问 provider：`alibaba-coding-plan`（DashScope 编码套餐，`coding-intl.dashscope.aliyuncs.com/v1` / `coding.dashscope.aliyuncs.com/v1`）、`qwen-portal`（`https://portal.qwen.ai/v1`，key 从 `chat.qwen.ai` 拿）。
- **Python 子进程默认按 cp936 解码，会把非 ASCII 输出读成乱码**：本机 `locale.getpreferredencoding()` = `cp936`，所以 `subprocess.run(..., text=True)` 不加 `encoding` 时，读子进程的 UTF-8 中文会变成乱码 —— 实测模型回 `收到` 而显示成 `鏀跺埌`（`'收到'.encode('utf-8').decode('gbk')` 可还原）。**读子进程输出一律显式 `encoding="utf-8"`**。shell 里的 `cat`/`rg` 不受影响。
- **`ast_grep` 已启用（2026-10-15）**，原生实现、支持 `csharp`、无需安装。注意它的输出在 hashline 编辑模式下只渲染到模式的字面前缀：`class $N` 的命中行显示成 `*46|class`，不带上下文。所以把它当**定位器**用（拿 file+line），内容再 `read` 看。
- **BCL `Task` 完成后的恢复要等下一次心跳 tick**。`INode.GetAwaiter(Task)` / `TreeTaskLink` 完成后都走 `WorldContext.Post`，而 `WorldContext.requestQueue` 由 `UpdateRule` 每帧排空一次（整队排空）。心跳 `frameTime=1000ms` 时，一个 300ms 的 `Task` 实测 1003ms 才恢复。别拿它做毫秒级计时。
- **控制台必须在 `Console/Server/Bin` 下启动。** `WorldLog` 只在 `../ProjectConfig/NLog` 存在时创建 NLog logger（配置在 `Console/Server/ProjectConfig/NLog/NLog.config`，日志 `Console/Server/Logs/Log.txt`，同时打到控制台）。在仓库根目录跑时 `logger` 为 null，`Log` / `Error` 空引用；心跳线程上 `WorldContext.Update` 捕获异常后再 `LogError`，这个 NRE 会盖住真正的异常。本 shell 不把当前目录当命令路径：`App.exe` 和 `./App.exe` 都是 command not found，要用带目录的路径（`Console/Server/Bin/App.exe`），工作目录仍设为 Bin。`WorldHeart` 不退出，看完日志后 `taskkill /F /IM App.exe`。
- **`INodeProxy.OnCreate` 不能删。** 直接实现 `INode` 的类型由 `INodeProxyGenerator` 生成 `INodeProxyRule.OnCreate(this)`。编号和 `SupportNodeRule` 已不在这里。
- **`SupportNodeRule` 每次都进写锁**，类型已经支持也一样。热路径（每次 `PoolGetUnit`）不要加。读档的法则查找发生在 `PoolGetNode` 之前，`ReadValue` 里的调用不能省。
- **法则接口对应的 `XxxRule<N>` 不要手写。** `RuleSupplementGenerator` 会生成同名抽象基类。再写一个就 CS0101。想跳过生成就给接口标 `ISourceGeneratorIgnore`。
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

- **模型不许自动提交**：绝不运行 `git commit`（含 `git -C … commit` 等任何等价形式）。改动做完就停下报告，由作者审阅后**手动提交**。硬保险：`~/.omp/agent/config.yml` 的 `bash.patterns` 已把 `git commit*` 设为 `deny`，`yolo` 模式下同样生效。
- 用法写进第 2 节，写成怎么用，不写完成记录。
- 还没做完的事写第 3 节。新坑追加到第 5 节。
- 保持精炼：目标是 2 分钟内能 re-onboard。
