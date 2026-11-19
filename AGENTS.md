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
| 技能 | `omp skill`。不加 `-g` 用项目的 `skills.json`，`-g` 用用户全局 | 用到才加载的流程。项目级没装；全局 `~/.agents/skills` 有画图和工程两类，见下表。不要用技能复述架构。`--no-skills` 关掉 |
| TTSR 规则 | 项目 `.omp/rules/*.md`，全局 `~/.omp/agent/rules` | 带条件的警告，不是说明书。输出或工具调用对上了才插进来。`/omfg <抱怨>` 从一次失误生成；`omp ttsr test` / `omp ttsr scan` 试。`--no-rules` 关掉。同一种失误重复出现、第 2 节又没拦住，再加一条 |
| 子代理 | 工具 `task`。`omp agents unpack` 到 `~/.omp/agent/agents`，`--project` 到 `./.omp/agents` | 并行探查。本仓库没有自定义代理 |
| 配置 | `~/.omp/agent/config.yml` | 模型角色、`bash.patterns`。`git commit*` 已 `deny`，`yolo` 下也生效。模型 / 额度看 `MODEL-PROVIDERS.md`，这里不写 |

常用内建工具：`read` / `edit` / `write` / `grep` / `glob` / `bash`，`ast_grep`（已开，支持 csharp），`task`，`web_search`。`lsp` 在本仓库是空的，见第 4.2 节。不许 `git commit`，见第 6 节。

### 全局技能（`~/.agents/skills`）

触发用自然语言（“用 grill-me 过一遍”），`/名字` 那种 slash 这里不认。项目级 `skills.json` 仍空。三组：

- **画图**：`diagram-design`（静态 SVG，45 种类型，正文 7–12px → 终端会糊，要内联就手写大字号）、`archify`（交互式 HTML，10 种 + JSON schema + commit 钉行取证；要 node，`finalize` 过四道门，产物落 `.archify/<type>-<slug>-<ts>/`，`--repo-root` 指被引用的仓库）
- **工程**（mattpocock/skills，MIT）：`grilling` / `grill-me`（按设计树一轮轮拷问，每问带推荐答案）、`writing-for-agents`（写本文件的方法：指针、两级负载、剪枝）、`diagnosing-bugs`（先造变红的循环）、`codebase-design`、`improve-codebase-architecture`、`retro`、`code-review`（Standards + Spec 两轴，报完不提交）
- **没装**它那批 `implement*` / `to-*` / `triage` / `wayfinder` / `pr` / `setup-*`：收尾要提交、依赖 issue tracker，与第 6 节冲突

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
     ├─ WorldHeart（当前宿主的心跳）
     ├─ WorldContext（SynchronizationContext，跨线程投递）
     ├─ StartWorld（线 0 启动世界，Add 规则读配置 + 创建其他线）
     └─ MainWorld（业务线入口）
```

### 分支与池
节点用 `ChildOf<>` / `ComponentOf<>` / `FieldOf<>` / `AsBranch<>` / `AsRule<>` 这些空接口声明约束，生成器据此写出强类型方法。分支有 Child / Component / Field / Temp / ListNode / Generic。Component 分支按**类型**为键，同类型只有一个；Field 分支是带 int 序号的列表，`AddField` 不传 key 就追加，适合固定槽位。

挂载方法由生成器给出，签名带约束：`AddComponent` / `AddField` 要求 `T : class, INode, NodeOf<N, 分支>, AsRule<Awake>`，带参数的重载要求 `AsRule<Awake<T1>>`。自己写泛型助手时按这个抄，少一个就 `CS0311`；父节点要接收 Field 分支还得声明 `AsFieldBranch`（参见 `AssetManager`、`DataBase`）。

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
`async TreeTask` / `async TreeTask<T>`。任务是节点，挂在 `TreeTaskManager`（`World` 的组件，只有 `Awake`）的临时分支上。取一条用 `TreeTaskManager.GetTask(node)` / `GetTask<T>(node)`；带 `Awake` 参数的节点仍走 `AddTemp`。顶层用 `.Coroutine()` 或 `.Coroutine(token)`。

**所有 `async TreeTask` / `async TreeTask<T>` 方法在每条执行路径上至少要有一个 `await`**；第一个 `await` 之前不能 `return` 或 `throw`。被 await 的方法自己必须有 await（否则它返回 null 任务，调用方 await 会空引用）；await 一个已完成的有效任务是可以的。路径上天然没有待等任务时，用第一行 `await self.TreeTaskCompleted();` 补同步点，它当场同步恢复，不额外等帧。`TreeTaskVoid` 不走这个构建器，不受此约束。分析器 `TreeTaskDiagnostic`（`TreeTaskDiagnosticConfig`，`WTREE001` 无 await，`WTREE002` 第一个 await 前 return/throw）已按此检查；在 `ProjectAnalyzerSetting` 里按程序集增删 `new TreeTaskDiagnosticConfig()` 即可启停。`return await` 不报，生成代码跳过。

节点上写 `self.AsyncDelay` / `self.AsyncYield` / `self.GetAwaiter`。延迟走 `CascadeTicker`，`AddTimerDelay` 用真实时间的 Utc Ticks，目标是 `INode`。`CascadeTickerData.Node` 是 `NodeRef<INode>`。

令牌是节点 `TreeTaskToken`，而且是**内联令牌**：不写进方法参数（C# 的 `CancellationToken` 要逐层传参，这里没有）。`async` 方法的构建器在每次 `await` 时，把当前任务的令牌 `SetToken` 到这次等的 awaiter 上。`SetToken` 再顺着 `RelevanceTask` 往下传，遇到已经有令牌的任务就停，所以内层新建的令牌不会被外层盖掉。业务要看当前令牌就 `await self.TreeTaskTokenCatch()`，不要自己加参数。顶层用 `Coroutine(token)` 或 `chain.SetToken(token)` 把令牌放上链。`parent.Add(child)` 后，父令牌取消时子令牌一起取消。`AddToken` 是另一条：新令牌既能取消老令牌，也能被老令牌取消。令牌取消不抛，`GetResult` 把 `Canceled` 当完成。

BCL `Task` 失败或取消则 `SetException`：只包一层的 `AggregateException` 拆开，取消抛 `TaskCanceledException`。`GetResult` 会把任务还池，所以同一个任务不要既 `Coroutine()` 又 `await`。

队列锁是 `await self.AsyncLock(key)`。同一个 key 排队，`Dispose` 凭证后下一位才继续。还在排队时取消，结果是 null。等待节点还池前要清掉 `Result`，节点池不抹字段。

`AwaiterState` 是 `Pending` / `Succeeded` / `Faulted` / `Canceled`。取出就是 `Pending`，表示结果还没写上，不是已经有人在等。`SetResult` 把结果留在任务上，有延续才叫醒；还池发生在 `GetResult`。完成后才来的 `await` 不再登记延续，编译器直接 `GetResult`。所以 `SetResult` 时没人等也不能还池。

`await self.WhenAll(tasks)` / `WhenAll<T>`：每条立刻启动，全部结束再完成返回的那条新任务。失败先把其余任务取完，再抛第一条异常。泛型按原来的顺序给结果。空数组无结果的直接返回，泛型返回空数组。`await self.WhenAny(tasks)` / `WhenAny<T>`：谁先完成，返回的新任务就完成。最快的失败从这里抛，其余结果在各自的 Start 里取走。没有任务就抛。两套共用 `TreeTaskWhenAll.Check`：空元素、已释放、同一任务出现两次直接拒绝。这些输入只能交给这里等。


### 其余
- 执行器替代委托，遍历中可以增删。
- 时间还有 RealTimeManager（校时）和 WorldTimeManager（游戏时间 / 帧）。
- 序列化：TreePack（二进制）/ TreeData（自描述，可循环引用）/ TreeCopy（深拷贝）。

> 以下（程序集 / 配置导出 / 配置运行时 / 离线包 / 管理器 / 关键文件地图）提到的 `Unity/…`、`Console/…`、`ExcelTool/…` 都在**项目仓库**（`Project/<项目>/`）里；本仓库只有 `Framework/`（源码）和 `Bin/`（产物）。

### 程序集
`Framework/Core` 编成 `WorldTree.Core.dll`（`netstandard2.1`），拷到 `Unity/Assets/Scripts/AOT/Plugins/WorldTree/`。Unity 的 asmdef 用 `overrideReferences` 显式引用这份 DLL。HybridCLR 里 Core 是 AOT，不热更。依赖 `NLog.dll`、`CommandLine.dll`、`System.Runtime.CompilerServices.Unsafe.dll`，都在 `Assets/Scripts/AOT/Plugins/`（版本要精确匹配，见 4.1）。改 Core 要重新出包。改生成器后重编 `Unity.Analysis.dll`，并重启 Unity。

### 配置导出
导表在 `ExcelTool`，跑 `Excel/配置导出.bat`。引擎是官方 Luban 5.1.0，本机要有 `Microsoft.NETCore.App` 8。检查用 `--strict`，5.1 不再认 `validationFailAsError`。脚本先检查 Game 和 Start 的 client、server，四次都通过才清空 `ExcelTool/Output`，再导出。失败是 `exit /b 1`，没有 `pause`。产物不进 Unity，也不进 `Console/Server`。`Luban/Runtime` 不在官方包里，是本地的 `ByteBuf` / `BeanBase`，升级时留着。

分组只有 `c`（客户端）和 `s`（服务器）。共用表写成 `c,s`。不建 `cs`、不建 `e`，不导 json，没有编辑器代码。命名空间是 `WorldTree`。数据是 Luban 的 bin（`.bytes`），代码目标是 `cs-bin`。`manager` 不能空：空了会生成没有类名的 `.cs`。

| | 入口 | 聚合类 | 现在有的表 |
|---|---|---|---|
| Game | `Excel/Excels/_Game.conf`，登记 `_Tables.xlsx` | `Tables` | 还没有业务表。表类用表名，记录类加 `Data` |
| Start | `Excel/Excels/StartConfig/Localhost/_Start.conf`，只导出 `s` | `StartTables` | 服务器业务线。一行：进程 1、线 1、`MainWorld`、`WorldHeart`、帧时长 1000（线 0 是硬编码的 `StartWorld`） |
| Client | `Excel/Excels/StartConfig/Client/_Start.conf`，只导出 `c` | `StartTables` | 客户端业务线。一行：进程 1、线 1、`MainWorld`、`WorldHeart`、帧时长 0（线 0 是硬编码的 `StartWorld`） |

`__beans__` 和 `__enums__` 是 Luban 的结构定义，列名固定，文件名只是习惯。`bean` 放共用或嵌套类型，`enum` 放枚举。现在字段都写在数据表的 `##type` 行，类型只有 `int` 和 `string`，这两份先不加。出现枚举或要单独定义的嵌套类型时再加。

两边的 C# 都写进 `Output/CSharp/{c,s}`，所以聚合类不能同名。后导出的用 `cleanUpOutputDir=0`，避免清掉已经写出的代码。bytes 分开：游戏表 `Output/Bytes/{c,s}/GameConfig`，服务器起服 `Output/Bytes/s/StartConfig/Localhost`，客户端启动 `Output/Bytes/c/StartConfig/Client`。

服务端 bytes 根目录是 `ExcelTool/Output/Bytes/s`（相对 `Console/Server/Bin` 向上三级）。Luban 的 `ByteBuf` / `BeanBase` 在 `ExcelTool/Luban/Runtime`，生成的表代码在 `Output/CSharp/s`，和 `ConfigLoader` 一起由 `Server.HotFix` 链接编译。`HotFix/Server` 的程序集约束是 `IGNORE`，Unity 不编译这目录。运行期怎么读表、怎么建线见「配置运行时」。

### 文件运行时
`FileManager` 是 `WorldTreeCore` 上的 `CoreObject`，进程级；地址是 `FileAddress` 的 `channel/group/tag:key`（`channel` 目前 `Pack` / `User` / `Temp`）。`IFileSource` 有两个实现：`LocalFileSource`（`Framework/Module/CoreModule/File/Local`，真实文件，可写）和 `YooAssetFileSource`（`CoreUnity`，只读 `TextAsset`，按文件名映射 Yoo 地址）。配置统一用 `Group = Config`，key 是 `GameConfig/...`、`StartConfig/Client/...`。读取走 `FileManager.ReadAllBytes`；句柄是 `FileHandle`，`StreamFileHandle` / `YooAssetFileHandle` 都是临时节点，由 NodePool 回收。编辑器挂 `LocalFileSource`，Player 挂 `YooAssetFileSource`，`ConfigLoader` 不再直接 `File.ReadAllBytes` / `LoadAsset<TextAsset>`。

### 配置运行时
配置不进节点树。`ConfigManager` 是 `WorldTreeCore` 上的 `CoreObject`，进程内一份，`Add` / `Get` / `TryGet` 按类型登记整张 Luban 表，所有世界线共用。框架不引用具体表类型。行对象继承 `BeanBase`，不要改成 `Node`。不要恢复 `Config` / `ConfigGroup` / `ConfigRef` / `CreateWorldConfig`。

起服表里的心跳类型写 `WorldHeart`。框架里的线程心跳叫 `WorldHeartThread`。服务器的 `WorldHeart : WorldHeartThread`，Unity 的 `WorldHeart : WorldHeartBase`（主线程）。两边不要编进同一个程序，`NameToType` 碰到重复短名会抛错。

Unity 由 `StartWorld.Add` 调用 `ConfigLoader.Load<T>` 和 `CreateWorldLines`。`StartWorldRule` 先 `GetAssetManager().AddAssetSource<YooAssetManager>()`，再按平台挂配置源：编辑器 `LocalFileSource` 读 `ExcelTool/Output/Bytes/c`，Player `YooAssetFileSource` 读配置 bundle；`ConfigLoader` 只从 `FileManager` 按 `Group = Config` 取 `GameConfig/...` 与 `StartConfig/Client/...`。Server 的 `StartWorldRule` 同样先挂 `LocalFileSource` 到 `Bytes/s`，再走同一套 `ConfigLoader`。地址规则见「文件运行时」。

线 0 由启动流程硬编码 `Create(0, StartWorld, WorldHeart)`（Unity 帧时长 0，Server 1000）。`StartWorld.Add` 读完配置调 `CreateWorldLines`，按起服表建 `LineId 1..N`（遇 `LineId == 0` 抛错），每建一条打日志；`MainWorld` 是业务线入口，它的 `Add` 不建线。`CreateWorldLines` 先把 `WorldTree`/`HotFix`/`GameEntry` 前缀的程序集登记进 `TypeInfo`，`WorldType` / `HeartType` 再走 `NameToType`。服务端游戏表在 `GameConfig/`，起服表在 `Options.ConfigPath`（默认 `StartConfig/Localhost`）；客户端只读 `StartConfig/Client`。

拷贝进 Unity 的位置：

- 表代码 → `HotUpdate/HotFix/Generated/`（`WorldTree.HotFix`）
- Luban 运行时 → `ThirdParty/Luban/`（`WorldTree.ThirdParty`，AOT，`allowUnsafeCode`）
- bytes → `AssetBundles/Config/{GameConfig,StartConfig/Client}/`
- YooAsset 收集器组 `Config`：`Assets/AssetBundles/Config`，`AddressByFileName` + `PackDirectory`

菜单 `WorldTree/拷贝客户端配置` 清空目标后全量拷贝，再 `AssetDatabase.Refresh()`。程序集从底到顶是 `ThirdParty` → `CoreUnity` → `HotFix` 与 `App` → `HotFixUnity`。`App` 引用 `CoreUnity`、`HotFix`、`ThirdParty`。`HotFix` 的 `allowUnsafeCode` 保持 false。

对照只看 `E:\Storage\Daily\BaiduNetdisk\WX20220921\config\`，不要照 HeadMaster 的 `Luban.ClientServer.exe`。客户端配置加载参考工程：`D:\Projects\GameEngine\Unity\HeadMaster_81`（对照 `ConfigLoader` / `ConfigImporterWindow` 的目录约定，不照它的 Luban 工具）。Windows 离线包的点击顺序见下一节。

### Windows 离线包
激活平台先切到 `StandaloneWindows64`。`MyGenAll`、`CopyDlls`、YooAsset 都跟当前激活平台走。Scripting Backend 保持 IL2CPP。场景里 `GameEntry.playMode` 选 `OfflinePlayMode`，不要用 `EditorSimulateMode`。

按这个顺序点，后一步吃前一步的产物：

1. 表有改动时，先跑 `ExcelTool/Excel/配置导出.bat`。没改表就跳过。
2. Unity 菜单 `WorldTree/拷贝客户端配置`。然后看 `Assets/AssetBundles/Config/StartConfig/Client/worldlineconfig.bytes` 是不是导出的那份（现在正常是 25 字节）。只剩 1 字节 `0x00` 就不要继续。
3. 菜单 `HybridCLR/MyGenAll`。这一步编译热更 dll，并跑 IL2CPP 生成裁剪后的 AOT dll，会锁住编辑器。
4. 菜单 `HybridCLR/CopyDlls`。热更 dll 进 `Assets/AssetBundles/Dlls/HotUpdateDlls`，AOT dll 进 `Assets/AssetBundles/Dlls/AotDlls`。
5. 菜单 `YooAsset/AssetBundle Builder`，按下面填完再点构建：

| 项 | 填 |
|---|---|
| Build Target | `StandaloneWindows64` |
| Build Package | `DefaultPackage` |
| Build Pipeline | `BuiltinBuildPipeline` |
| Build Mode | `ForceRebuild` |
| Build Version | 任意非空字符串 |
| Compression | `LZ4` |
| Output Name Style | `HashName` |
| Copy Buildin File Option | `ClearAndCopyAll` |
| Copy Buildin File Tags | 空 |
| Encryption | 不勾 |

不要选 `SimulateBuild`。构建完成后，包在 `Assets/StreamingAssets/yoo/DefaultPackage`。这一步必须在第 2 步和第 4 步之后，否则包里是旧 dll 或空配置。

6. `File → Build Settings`。平台 `Windows, Mac, Linux`，架构 `x86_64`。勾上挂了 `GameEntry` 的启动场景。点 `Build`。输出目录用工作区的 `Build/<项目名>/StandaloneWindows64/`，不要放在 `Unity` 工程里面或 `Assets` 下面。

`HybridCLRHelper` 在 AOT 的 `GameEntry` 里。改了加载顺序要重新做第 6 步，只重复第 3 到第 5 步不会进 Player。只改了热更代码或 bytes 时，第 3 到第 6 步仍要按顺序重做，因为 dll 和 bytes 都打进了 StreamingAssets。

### 管理器与来源
管理器分两层：进程级在 `WorldTreeCore`（`FileManager` / `ConfigManager`），世界级在 `World`（`DataBaseManager`）。`AssetManager` 是进程级：只有一份，挂在主世界线（`Core.MainLine`）的 world（第一个启动的世界；Unity 下就是主线程那个）上；`World.GetAssetManager()` 直接取这一份。管理器的来源是 `FieldOf` 子节点；装配走 `AddXxxSource` / `AddField`。`FileManager` 和 `DataBaseManager` 支持多来源，其余管理器默认单来源。

- `Framework/Core/CoreModule/File`：`FileManager` + `IFileSource` + `FileAddress`；`Framework/Module/CoreModule/File/Local` 放 `LocalFileSource` / `StreamFileHandle`，Unity 放 `YooAssetFileSource` / `YooAssetFileHandle`。详见「文件运行时」。
- `Framework/Core/CoreModule/Config`：`ConfigManager` 是进程级表注册表；详见「配置运行时」。
- `RunTime/Core/CoreModule/Asset/`：`IAssetSource` 接口 + `Base/AssetManager` + `YooAsset/YooAssetManager`（`IAssetSource` 实现）。`AssetManager` 进程内一份，挂在主世界线的 world 上；`World.GetAssetManager()` 直接取这一份。按 group + priority 注册多个来源，`AddAssetSource<TSource>(group, priority)` 的 group 同时作为 `Awake<string>` 参数（YooAsset 包名）；`LoadAsset<T>(key)` 无 `@` 前缀时按优先级链式找第一个 `Exists(key)` 的来源，`LoadAsset<T>("@Pack:Key")` / `LoadAsset<T>(pack, key)` 指定包不 fallback。默认组是 `DefaultPackage`。`IAssetSource` 必须有 `Exists`。`YooAssetManager` 按地址缓存 `AssetEntry`（Node），异步同地址只发一次加载；`UnloadAsset` / `UnloadAllAssets` 显式卸载，暂不做引用计数。业务写 `world.GetAssetManager().LoadAsset<T>(key)`，名字里不出现 `Yoo`；`YooAssetFileSource` 是 `FileManager` 的源，不要和 `AssetManager` 混。
- `Framework/Core/CoreModule/DataBase`：`IDataBase` 接口 + `DataBaseManager` + `DataBaseManagerRule`（同步与异步 CRUD）。`Framework/Module/CoreModule/DataBase`：`LiteDB/LiteDBManager`（`LiteDBCollection<T>` 是它的组件）。`DataBaseManager` 按 group 存多个源，空字符串是默认组，组名重复报错；`GetSource` / `TryGetSource` 取指定组，`AddDataSource` / `AddGroupDataSource` 挂源。用法 `db.AddDataSource<LiteDBManager>(path)`，之后 `db.Insert` / `Find` / `Delete` 走默认组的 `DataBaseManagerRule`。LiteDB 没有原生异步，异步方法是同步执行后返回已完成的 `TreeTask`。

### 仓库
框架、产品、工具拆成平级仓库，放同一父目录（`WorldTreeWorkspace`）。父目录名随便，不归 git。

```
WorldTreeFramework/        框架核心仓库，只放纯 C#
  Core/                    节点 / 法则 / 异步 / 序列化 / 池
  AnalyzerCore/            分析器主体
  SourceGeneratorCore/     源生成器主体
  Module/                  平台无关模块

Project/                   项目分类目录（不归 git）
  _Template/               演示 + 模板仓库，各平台平级
    Console/ Avalonia/ WinForm/ WPF/ Godot/ Unity/ ExcelTool/
  xxx游戏项目1/            从 _Template clone 派生
    Tables/                策划表：xlsx + conf
    ExcelTool/             配置导出.bat + Output/
    Server/                控制台服务器
    Unity/                 Unity 客户端

ToolProject/               工具分类目录（不归 git）
  OperateLinkFile/         VS 插件
```

依赖靠 csproj `ProjectReference` 指向 `WorldTreeFramework/Core`。Unity 用编好的 `WorldTree.Core.dll`。平台适配不进框架仓库：`CoreUnity` 在 Unity 宿主里，`Unity.Analysis` 在 `Unity/UnityTool`。`_Template` 里各平台平级，复制到项目后相对路径不变。各平台的 InputBindTool 留在各自宿主下。Luban 在 `ExcelTool/Luban`，`ExcelTool` 自包含。xlsx 源表和导出产物目录分开。`WorldTreeFramework` 和 `Project/_Template` 各是一个 git 库。命名用英文，模板不叫 `Demo`。

### 关键文件地图
| 路径 | 说明 |
|---|---|
| `Framework/Core/` | .NET 侧核心源码（`WorldTree.Core.csproj`，`DOTNET` 宏） |
| `Framework/Core/Core/` | 启动入口与装配（`WorldTreeCore` / `StartWorld` / `MainWorld` / `CodeLoader`） |
| `Framework/Core/CoreModule/` | 内核实现（`WorldLine` / `WorldHeart` / `WorldContext` / `Config` / `DataBase` / `File` / `ObjectPool` / `Log` / `Time` / `Type` / `Id`） |
| `Framework/Core/Object/` | 对象模型（Basic / CoreObject / Unit / Node / World / Branch / Rattan / ObjectBox / DynamicNodeQueue） |
| `Framework/Core/Rule/` | 规则系统（Base / Group / List / Manager / Executor） |
| `Framework/Core/TreeTask/` | 任务：`GetTask`、`WhenAll`、`WhenAny` |
| `Framework/Core/CoreModule/File/` | `FileManager` / `FileAddress` / `IFileSource` / `FileHandle`；进程级 VFS |
| `Framework/Core/CoreModule/DataBase/` | `IDataBase` / `DataBaseManager` / `DataBaseManagerRule`；Line 级，group 到单源 |
| `Framework/Core/CoreModule/Time/Timer/ClockTimer/` | `ClockTimer`：可 `Restart` 的节点倒计时器 |
| `Framework/Module/` | 模块（csproj 链接 Unity 的 Module 源码）：CoreModule(DataBase/File 实现)/Input/NeuralNetwork/TreeTween/View |
| `Framework/Module/CoreModule/File/Local/` | `LocalFileSource` + `StreamFileHandle` |
| `Framework/Module/CoreModule/DataBase/LiteDB/` | `LiteDBManager` / `LiteDBCollection<T>` / `UnitBsonDocument` |
| `Framework/SourceGeneratorCore/`、`Framework/AnalyzerCore/` | 源生成器 + 分析器 |
| `Unity/Assets/Scripts/WorldTreeFramework/WorldTree/Module/` | 平台无关模块（被 .NET Module csproj 链接） |
| `Unity/Assets/Scripts/WorldTreeFramework/UnityEnvironment/RunTime/` | `WorldTree.CoreUnity`（Unity 运行时环境：心跳/日志/组件，引用 `WorldTree.Core` + `WorldTree.Module`）；固定实现按 `Core/CoreModule`、`Module`、`Object` 分层；`Core/CoreModule/Asset/YooAsset/` 放 `YooAssetManager` + `AssetEntry` |
| `Unity/.../RunTime/Module/View/Coat/CoatCollector.cs` | Unity 视图外套收集器；编辑器在 `Editor/View/Coat/CoatCollectorEditor.cs`，按字段名反射填充 entries |
| `Unity/.../RunTime/Core/CoreModule/File/YooAsset/` | `FileManager` 的 Unity Yoo 源：`YooAssetFileSource` + `YooAssetFileHandle` |
| `Unity/.../RunTime/Core/CoreModule/ObjectPool/LoadPool/` | `LoadPoolDrawCollectorRule`：`DrawCollector` 的加载池，预制体留在 `CopyObject` |
| `Unity/Assets/Scripts/WorldTreeFramework/UnityEnvironment/App/` | `WorldTree.App`（`UnityWorldTree` 启动器 + `ConfigLoader`） |
| `Unity/Assets/Scripts/WorldTreeFramework/UnityEnvironment/Editor/` | `WorldTree.UnityEditor`（编辑器工具，用 Odin/Sirenix） |
| `Unity/Assets/Scripts/ThirdParty/` | `WorldTree.ThirdParty`（第三方库，目前 `Luban/ByteBuf`、`BeanBase`） |
| `Unity/Assets/Scripts/HotUpdate/{HotFix,HotFixUnity}/` | HybridCLR 热更程序集（`HotFix/Generated` 放表代码） |
| `Unity/Assets/Scripts/AOT/{GameEntry,Plugins}/` | AOT 入口 + 全部预编译 dll：`Plugins/` 放框架产物（`WorldTree.Core/Module.dll`、`Unity.Analysis.dll` 分析器）和 Core 依赖（`NLog`/`CommandLine`/`Unsafe`），另有厂商包 `Sirenix/`（Odin）、`LiteDB/`、`Sqlite/`。顶层 `Assets/Plugins` 已不再使用 |
| `Console/Server/` | 纯 .NET 服务器示例（`Program.cs` 里 `WorldTreeCore.Create(...)`） |
| `ExcelTool/` | Luban 导表。入口 `Excel/配置导出.bat`，产物在 `Output/`。用法见「配置导出」 |
| `Unity/ProjectSettings/HybridCLRSettings.asset` | HybridCLR 配置（hotUpdateAssemblies / patchAOTAssemblies） |
| `Unity/.../Editor/ThirdParty/HybridCLR/` | HybridCLR 自定义脚本（HybridCLREditor / AOTGenericReferences 生成物） |
| `Unity/Assets/Scripts/AOT/GameEntry/_MonoTest/UnityMCP/Editor/` | Unity MCP 服务器（`unity_exec` / `unity_refresh` / `unity_logs` / `unity_select`，HTTP `127.0.0.1:8099/mcp`）；客户端配置在 `.omp/mcp.json` |

不单开 `IUnitData`。继承是 `IUnit` → `INode` → `INodeData`，这三个名字和 `NodeRef`、`NodeRule` 都不要改。默认法则不要挂到 `IUnit`。

---

## 3. 进行中

### 资源加载与池
- `AssetManager` 已有 group + priority 多来源、`Load` 的无前缀链式 / `@Pack:Key` 指定包、异步加载合并、`UnloadAsset` / `UnloadAllAssets`；进程内一份，挂在主世界线的 world 上；暂不做引用计数。

### View 骨架（引擎层显示物体）
代码在 `Framework/Module/View`，目录按角色划分：

```
Base/       视图基类与绑定
  IViewDraw.cs / ViewDraw.cs / IView.cs / View.cs
  ViewLayerItem.cs（ViewObject / ViewWidget / ViewContainer / ViewLayerItem / Form） / ViewBranch.cs
  ViewRuleBase.cs（Open/Close/ViewDrawOpen/ViewDrawClose/ViewOpenAnim/ViewCloseAnim/Show/Hide/LayerChange/SubViewClose）；规则类与节点同文件（ViewProxyRule）
Widget/     控件级 ViewWidget：Button / Text / Image
Layer/      ViewLayer（规则与节点同文件）；ViewRoot.cs 在 View 根目录
ViewManager.cs（含 ViewManagerRule）   模块根
```

### View 当前交接（新会话先看）

- **ViewName**：`Add` 时还是空就写成类名。控件 `AddComponent(out control, name)` 走 `Awake<string>` 覆盖，空字符串忽略。`ViewLayer` 仍按类型码打开窗口，不传这个参数。还池前清掉名字。收集器只按这个名称取。
- **结构层已完成**：`View` 下分 `ViewBranch`（子 View / widget）与 `ChildBranch`（普通子节点）。`View.Draw` 是 `NodeRef<IViewDraw>`。`ViewDraw : LoadUnit, IViewDraw` 是壳基类，不是节点，没有法则。平台壳以 `Draw` 开头并继承 `ViewDraw`，只放在 `Draw` 上。
- **视图状态**：`ViewState` 是位标记：`Close`、`Load`、`OpenAnim`、`Open`、`CloseAnim`。`OpenAnim`、`Open`、`CloseAnim` 都算已打开，子组件和壳都在。`OpenAnim` 和 `CloseAnim` 对调不加载、不拆组件。`OnOpen` 从 `Close` 进入 `Load`，把内部令牌挂到 `ViewDrawOpen` 上再 `await`。回来时这一次的令牌还在，先 `await Open`，再改成 `OpenAnim`，发 `LayerChange/Show`，再播打开动画，播完改成 `Open`。加载中被关则取消这次令牌，状态直接改回 `Close`，并马上离开打开栈。令牌留到这次 `await` 结束再卸。回来后只看这次令牌：已经取消就返回，不改状态，也不挂壳。资源加载继续走完，但不再 `New` 实例。`CloseAnim` 播完才 `CloseCore` 和 `ViewDrawClose`，然后改回 `Close`。播到一半又打开，取消当前动画的令牌，改成 `OpenAnim`。`Dispose` 不等动画，组件和壳一起拆。令牌由 `View` 内部挂在动画任务上，取消时那段 `await` 返回，`RunAnim` 按新状态接着播。
- **层级已定**：`ViewManager -> ViewRoot -> ViewLayer -> 窗口` 都走 `ViewBranch`。`ViewLayer` 是子级页面管理器：`ViewDict` 是类型码 → 节点，同类型只有一个，关掉的页面也留在字典里。`ViewOpenList` 从接受打开起占槽，直到变成 `Close` 才摘掉，含 `Load`、`OpenAnim`、`Open`、`CloseAnim`。加载被取消后马上离开。顺序就是叠放顺序，`LayerCount` 等于它的长度。`ViewBranch` 只负责挂载，层关闭时子节点随分支回收，然后清空这两份记录。
- **Unity 平台已开始**：`CoatCollector` 在 `CoreUnity/RunTime/Module/View/Coat`，编辑器在 `Editor/View/Coat`。壳是 `DrawCollector : ViewDraw<CoatCollector>`，放在 `Module/View/Draw`。`Coat` 属性就是这个 `CoatCollector`，存在 `Target` 上，物体用 `Coat.gameObject`。加载池规则是 `LoadPoolDrawCollectorRule`，在 `CoreUnity/RunTime/Core/CoreModule/ObjectPool/LoadPool`；`CoreUnity` 引用 `WorldTree.Module`。收集器按字段名自动填充 entries，空 Target 红色显示，可手动开启 Target 编辑；不支持路径字符串和特性标记。
- **场景标记**：视图根用场景里唯一的 `CoatCollector`。`DrawRootOpen` 调用 `GetDraw`。根没有视图父级，所以去场景里找到唯一的 `CoatCollector`，把这个 `CoatCollector` 写入壳的 `Coat`，再写入 `View.Draw`，并 `DontDestroyOnLoad`。关掉 `Dispose` 并清空 `Draw`，不销毁场景物体。编号作废后 `Draw.Value` 为空。`CoatLayer`（层，`byte layer`）挂在根收集器的 `Layer0` 到 `Layer7` 上，层号要和字段下标一致。壳是 `DrawLayer : ViewDraw<CoatLayer>`。排序留到后续 View 侧数值绑定。
- **生成器**：`Framework/SourceGeneratorCore/ProjectGeneratorSetting.cs` 的 `ModuleConfigs` 已启用 `BranchSupplementGeneratorRun`，`ViewBranch` 的 `AddView/RemoveAllView` 由它生成；UnityTool 的 ModuleConfigs 不启用，Module 以预编译 DLL 进入 Unity。
- **平台加载链**：`DrawGet.GetDraw<T>(view)` 只收视图，T 是要取出的壳类型。名字用 `ViewName`，空则用类名。没有视图父级才 `FindSceneCollector`，写入壳的 `Coat`。有父级就用父级壳所在物体上的 `CoatCollector`：根的壳直接拿着它，层的壳拿着 `CoatLayer`，再从同一物体上取。没有这张表就停，不往上找，也不去场景里找。表里查到的对象已经是壳要的组件就直接写入 `Coat`，否则取它所在物体上的那个组件，没有就失败。窗口表里没有就向池借，挂到父级物体下。`ViewLayer` 没有对应名称就失败，不向池借。不往子级找。查到的物体不改父级，路径留空。关掉由壳的路径决定还池还是只丢引用。
- **层已接入**：`ViewRoot` 的 `Open` 在壳接上之后建八个层。`ViewName` 写成 `Layer0` 到 `Layer7`，和收集器面板上的名称对上，再 `OnOpen`。某一层没有这个名称或目标是空的，八层拆掉，根打开失败。`GetLayer(int)` 只取已经打开的层，不建节点。窗口和控件的父级物体上还要有 `CoatCollector`。动画和控件字段绑定还没写。
- **下一步**：接动画和 widget 字段绑定。
- `FormTest` 在 `HotFix/Client/_Test`，`ViewTest` 在 `HotFixUnity/Client/_Test`；Play 时由 `InitialDomain` 触发。
- `Depth/Order` 的位压缩设计仍待后续收口。

当前实现的结构：

- `View` 是「节点 + 绘制壳」。`View.Parent` 的 setter 按父类型推导 `ParentLayer`；`Layer`（byte）、`Depth`、`Order` 用位压缩排序。
- 视图分三支：`ViewObject`（加载物体：按类型名加载预制体）、`ViewWidget`（子级/组件：不加载，字段绑定）、`ViewContainer : ViewWidget`（容器级：能承载窗口与其他 widget）。`ViewLayer` / `ViewRoot` 都是 `ViewContainer`。
- `ViewLayerItem : ViewObject, ViewOf<ViewLayer>`，`Form : ViewLayerItem`（有自身表现的界面）；`ViewLayerRule.TryAddView` 的约束是 `where T : ViewLayerItem`。
- **窗口的宿主是层 `ViewLayer` 的 `ViewBranch`**（`ViewOf<ViewLayer>`，层逻辑保证同类型只有一个）；`View.Draw` 只放壳，不承载子级 widget。
- **壳不是组件**：`ViewDraw` 继承 `LoadUnit`，平台壳再继承 `ViewDraw`。显示子级（子 View / widget）挂在 `View` 的 `ViewBranch`（`ViewBranch : ChildBranch`，`View : ViewOf<View>`）下；普通子节点仍可用 `ChildBranch`。`ParentLayer` 继承父视图所在的层，`Depth` 不增长。
- 元素挂载由父级调用 `OnOpen` / `OnClose`。`OpenAnim` 和 `CloseAnim` 都算已打开。`OnOpen` 从 `Close` 进入 `Load` 并 `await ViewDrawOpen`；回来恰好是 `Load` 就 `await Open`，然后才改成 `OpenAnim`，发 `LayerChange`（`IsShow` 时再发 `Show`），再播打开动画，播完改成 `Open`。`CloseAnim` 播完才 `CloseCore`（`Hide` / `Close`）和 `ViewDrawClose`。`CloseAnim` 中又打开只改成 `OpenAnim`。`Load` 中关闭则取消这次加载令牌并直接改回 `Close`。回来后只认这次令牌，取消了就不挂壳。`OnBeforeDispose` 在已打开时直接 `CloseCore` 和 `ViewDrawClose`，不播关闭动画。`IsShow` 只发 `Show`/`Hide`。平台按视图类型实现 `ViewDrawOpen` / `ViewDrawClose` 和 `ViewOpenAnim` / `ViewCloseAnim`。
- **字段声明在 View，实例挂在 ViewBranch**：子级 widget 字段写在 View 上（如 `FormTest.text`），类型用 `NodeRef<T>`（还池后自动变空，防悬空）；实例节点挂在 View 的 `ViewBranch` 下，关闭时由 `RemoveAllView` 一起回收。
- 子级 widget 的绑定契约写在 **View 的字段**上（如 `FormTest.text / button`），由编辑器工具反射字段列表、生成器生成强类型赋值，运行时按字段名分派，零反射。
- `ViewLayerRule.OpenView` 不先看页面状态。不在打开栈上才压入 `ViewOpenList` 并设 `Layer`，然后一律 `await View.OnOpen()`。回来时，这次调用前就在栈上并且仍然开着才置顶；否则 `FinishOpen`。新开的页面按压栈顺序，加载完成不抢到顶上。
- `ViewRoot` 固定八个层，名称是 `Layer0` 到 `Layer7`，不写成字段。`Open` 规则建层并打开，`ViewManager.GetLayer(int)` 转发后只取已经打开的层。层号超出 `0–7` 或这一层还没打开会失败。
- `View.Parent` 的每个分支都要显式写 `Layer`：节点池不抹字段，复用的节点会带着上次的值；
  父是 `ViewLayer` 时 `ParentLayer = 父`（窗口直接挂层下），父是普通 View 时继承父的 `Layer`（`Order` 与父级相同，渲染同层），窗口的槽位由 `ViewLayerRule` 写。
- 层的深度恒为 0（`View.Parent` 里对 `ViewLayer` 提前返回 `ParentLayer = null`）：`ViewLayer` 通过 `ViewRoot` 的 `ViewBranch` 挂载（可多个），窗口在 `ViewLayer` 的 `ViewBranch` 上（同类型唯一，查找时只认 `ViewLayerItem`）。
- 两个宿主各自持有层时 `Layer` 号是全局的：层号相同会得到相同 `Order`，需要调用方保证不撞号。

层的连接方式（已跑通）：

- `ViewManager.GetLayer(int)` 取根打开时建好的层。层的 `ViewDict` / `ViewOpenList` 是 `ViewLayer` 自己的字段，在 `ViewLayer` 的 `Awake` 里分配。
- `ViewLayerRule.TryAddView` 把窗口挂到层的 `ViewBranch`，并写入 `ViewDict`，不进打开栈。`OpenView` 不先看页面状态：不在栈上才压入 `ViewOpenList` 并设 `Layer`，再一律 `await View.OnOpen()`。回来时，这次调用前就在栈上并且仍然开着才置顶；否则 `FinishOpen`。新开的按压栈顺序，加载完成不抢到顶上。
- `CloseView` 不先看状态，直接 `await View.OnClose()`，回来 `FinishOpen`：已经是 `Close` 才从 `ViewOpenList` 摘掉并重排 `Layer`。还开着或还在加载就留在栈上。节点仍留在 `ViewDict` 和 `ViewBranch` 上。`CloseAllView` 从顶往下走 `CloseView`，不另行清栈。`ViewLayer.OnClose` 先 `await CloseAllView()`，子节点随分支释放后清空这两份记录。

- `View.OnBeforeDispose` 在已打开时直接拆组件和壳，不播关闭动画，然后把窗口改回关闭。还在加载中则只改回关闭。级联回收走遍历里的 `OnBeforeDispose`，子 View 也会先关闭。
- 冒烟：`TryAddView<FormTest>` → `Parent == ViewLayer`、`ParentLayer == ViewLayer`；`await OpenView` 后 `Draw` 是 `WorldTree.DrawCollector`、`count=1 slot=1`；`await CloseView` 后 `count=0`、`IsOpen=false`。事件发送点是 View（`OnOpen`/`OnClose`/`IsShow`/`OnBeforeDispose`）；`OnCutSelf` 清 `ParentLayer`/`Layer`（`RemoveNode` 不清 Parent）。
- Unity 侧冒烟在 `HotFixUnity/Client/_Test/ViewTest.cs`，测试界面 `FormTest.cs` 与规则在 `HotFix/Client/_Test`（模块仓库不再放测试界面）：开 → 隐/显 → 关 → 重开 → Dispose，失败抛异常。已在 `InitialDomain` 的 `AddRule` 启用（每次 Play 跑），Q 键那处仍注释。

剩下的问题：

- `Depth` 恒为 1：`ParentLayer` 取的是「父视图所在的层」，第 2–7 位段空置；`Order` 实际只用 3 bit 层号（有效 0–7）加 4 bit 槽位（1–15）。
- `IsShow` 只记下显隐并发 `Show` / `Hide`，不碰壳。

---

## 4. 踩过的坑（重要）

> 分三段：**4.1 工程与程序集**（仓库结构/依赖/程序集）、**4.2 本机环境与工具用法**（agent shell / 编辑器工具 / 编码约定）、**4.3 运行时与源生成器**。已解决的只留"症状 → 解法"，未解决的留判据。**模型 / provider / 额度 / 凭据相关不在这里**，全部见仓库根 `MODEL-PROVIDERS.md`。

### 4.1 工程与程序集

- **`overrideReferences: true` 会切断项目里所有预编译 DLL 的自动引用**：改成 true 的 asmdef，必须把代码（含源生成器生成的那部分）直接 `using` 到的每个 dll 显式写进 `precompiledReferences`。踩过 Sirenix、Unsafe。
- **netstandard 少 `System.Runtime.CompilerServices.Unsafe`**（net6.0 才内置）→ 引包即可。Core 报"Unsafe 不存在"就是它。
- **`WorldTree.Core.dll` 的 3 个外部依赖，Unity 里都得有**：`NLog.dll`、`CommandLine.dll`、`System.Runtime.CompilerServices.Unsafe.dll`（均在 `Assets/Scripts/AOT/Plugins/`）。**Unity 按强名称精确校验版本**——NLog 必须 5.4.0（程序集 `5.0.0.0`）、CommandLineParser 必须 2.9.1（程序集 `2.9.1.0`），与 `WorldTree.Core.csproj` 的包版本一致；旧版会让 Core.dll **拒绝加载**。清插件时别把 Unsafe.dll 一起删。
- **改 Core/Module 后没重编 `Unity.Analysis`，Unity 里的 dll 还是旧的**：`Assets/Scripts/AOT/Plugins/WorldTree/` 的 `WorldTree.Core/Module.dll` 由 `Unity.Analysis` 的 PostBuild 拷入；旧 dll 会报 `CS1061 GetFileManager` / `CS0246 FileAddress` / `LocalFileSource` 找不到。先 `dotnet build UnityTool/Analysis/Unity.Analysis.csproj`，再 `unity_refresh`。
- **源生成器/分析器与目标框架无关**（跑在 Roslyn 编译器进程里）→ netstandard2.1 照用。
- **HybridCLR 三条**：① `AOTGenericReferences.cs` 是自动生成物**别手改**——真正生效的 `PatchedAOTAssemblyList` 由"热更代码的泛型用法"**自动检测**得出，`patchAOTAssemblies` 设置在本版本基本没接进生成流程。② 社区版里 **Assets 下的预编译 dll = AOT，不可热更**；热更 dll 必须由 `CompileDll` 从**源码**编，光把 dll 丢 `Temp/Bin/Debug` 无效（旗舰版 DHE 才有 AOT 热更）。③ **AOT 程序集不能引用热更程序集**——`WorldTree.Config` 曾在 `HotUpdate/` 下却漏进 `hotUpdateAssemblies`，MyGenAll 的 IL2CPP 构建报 `Failed to resolve assembly: WorldTree.HotFix`（已修）。
- **HybridCLR `CopyDlls` 源目录要对齐 `CompileDll` 输出**：`CompileDll` 输出到 `hotUpdateDllCompileOutputRootDir/{target}`（`HybridCLRData/HotUpdateDlls/StandaloneWindows64/`），而 `externalHotUpdateAssembliyDirs` 默认 `Temp/Bin/Debug`（空）会让 `CopyHotUpdateDlls` 报 `FileNotFoundException`。改成 `SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target)`。构建链 `MyGenAll` → `CopyDlls`：热更 dll 进 `AssetBundles/Dlls/HotUpdateDlls`，`PatchedAOTAssemblyList`（含 `WorldTree.Core.dll`/`WorldTree.Module.dll`）进 `AssetBundles/Dlls/AotDlls`；运行时 `LoadAOT` 按 tag `aotDlls` 加载。`LoadHotUpdate` 只收地址以 `.dll` 结尾的资源，并按依赖顺序装入：`WorldTree.CoreUnity.dll` → `WorldTree.HotFix.dll` → `WorldTree.App.dll` → `WorldTree.HotFixUnity.dll`。按标签原顺序会先装到 `App`，解析 `WorldLineConfigData` 时抛 `TypeLoadException`（`HybridCLRHelper` 已按这个顺序写死）。这段在 AOT 的 `GameEntry` 里，改了要重新出 Player。
- **Luban 的 `ByteBuf` 要开 `allowUnsafeCode`**：unsafe 在 `WorldTree.ThirdParty`，不在 `HotFix`。`HotFix` 保持 `allowUnsafeCode:false`。Server 侧 csproj 写 `AllowUnsafeBlocks=true`。
- **`_Template` 路径是脆弱的**：`_Template` 是占位名，clone 后会被重命名或直接当项目开发，任何引用 `_Template` 内部内容的相对路径（如 `..\..\Luban`）都会断。运行时依赖必须放进自包含目录（如 Luban 引擎放 `ExcelTool/Luban`，`配置导出.bat` 用 `..\Luban`），跨仓库引用只允许指向独立的平级仓库（如 `WorldTreeFramework`）。

### 4.2 本机环境与工具用法

- **agent 的 shell 是 OMP 自带 POSIX 风格 shell**（不是 git-bash、也不是 PowerShell）：`$0`/`$SHELL` 空、`uname` 报 `MS/Windows`、`Get-ChildItem` 等 pwsh cmdlet 不存在；`ls`/`cat`/`sed`/`find`/`rg`/`grep` **全是内建 shim**（`type rg` → "shell builtin"，`rg --version` 却谎称 15.1.0），只有 `dotnet`/`git`/`python`/`code`/`node`/`npm`/`curl`/`winget` 是真 exe。
- **`rg -c` 会假装「0 命中」**：它走 GNU 语义，给每个扫到的文件打印 `path:0`（实测 `rg -c "self as INode" .` 输出 1978 行、其中 1972 行是 `:0`）→ 极易误读成"0 命中"，叠上输出截断更糟。查命中用 `rg <pat> <path> | wc -l` 或 `grep` 工具（**grep 工具本身正常**：能搜到被 `.gitignore` 屏蔽的 `Framework/Core/obj`，也能搜 `.idea/**` 等隐藏文件；唯一真会返回 0 的情况是**模式里带字面引号**）。
- **装完工具要重开 VSCode**：winget/msi 改的是注册表，改不了已经在跑的父进程 → 会话中途装完 node，当前 shell 仍 `command not found`（`cmd /c where node` 也一样），新开会话才生效。当前会话的临时办法是加前缀，但**分隔符必须是 `;`、路径必须是 Windows 形式**：`PATH="C:/Program Files/nodejs;$PATH" node -v` 能跑；用 `:` 分隔或写成 `/c/...` 都不行（`/c/` 只给 `ls`/`cat` 这类 shim 用）。
- **宿主终端只决定继承的环境变量，不参与工具行为**（VS2026 Developer PowerShell 与 VSCode 集成终端实测逐字节一致：cwd / shell 身份 / `uname` / `rg` 输出 / grep 命中数）→ "换终端"解释不了工具行为差异。**渲染**：`mermaid` 渲染成 ASCII 方框箭头；`svg` 代码块**需要** `terminal.integrated.enableImages: true`（VS Code 默认关，2026-10-16 已在本机打开）——开了之后 640×170 的 SVG 能正常显示（文字因栅格化+缩放到 `tui.maxInlineImageColumns=100` 网格略糊），没开时整块连文字一起消失（不是降级成源码）。→ 画图默认仍用 `mermaid`（可复制、可 diff）；要精确布局或给人看的成品图才用 `svg`（omp 二进制支持 Kitty/Sixel/iTerm2/OSC 1337 四种图像协议）。
- **写盘无限制**：工作区、`%TEMP%`、`C:\` 根、用户目录实测可写；`dotnet build` + NuGet 拉包正常。构建时的 `NU1900 无法加载源 …/vulnerability.base.json` 只是**漏洞审计源**取不到，**不是还原失败**，别当网络问题。
- **`xd://lsp` 在本仓库空转**（`No language servers configured`；本机无 `csharp-ls`/`OmniSharp`，`~/.dotnet/tools` 空）→ 系统提示里"改导出符号前先查 references"只能退回 grep。可用候选：VSCode C# 扩展自带的 `~/.vscode/extensions/ms-dotnettools.csharp-*/.roslyn/roslyn-language-server.exe`（stdio 握手实测能返回合法 initialize，但重复运行不稳定，**未接进 OMP**）。
- **`git commit` 有硬拦截但 `eval` 能绕过（护栏 ≠ 边界）**：`bash.patterns` 把 `git commit*` / `git -C * commit*` 设为 `deny`（`yolo` 下仍生效、也管 headless 子代理、复合命令分段同样命中；`git status`/`log` 等读操作不受影响），但**管不到 `eval` 里 spawn 的子进程**（实测 `subprocess.run(["git","commit","--dry-run"])` 畅通）→ 所以"不许自动提交"实际靠 §6 的约定 + 这条护栏；要变真边界得设 `tools.approval.eval: prompt|deny`（代价是每次确认或彻底失去 eval），作者已选护栏方案。
- **编码与行尾**：仓库 `core.autocrlf=true`（blob 存 LF、工作区 CRLF）→ **比对内容前先归一化行尾**，否则纯编码改动会被误判成整文件改动；**502 个 `.cs` 带 BOM 是既有状态，别去修**。`read` 遇到非 UTF-8 直接拒收（`not valid UTF-8 text`），`ast_grep` 会**静默跳过**该文件 —— 文件莫名搜不到先查编码。
- **模型 / provider / 额度 / 凭据的事一律看仓库根 `MODEL-PROVIDERS.md`**（本地文件，已 gitignore，不随仓库分发）：角色配置与坑、Cursor 命名坑、Claude 在 Copilot/Cursor 的不对称、Premium 计费机制与重测法、fallback 链写法、`omp usage` 失效、provider 断开、自定义 provider 写法与 `~/.env` 凭据语义、OpenDesign 订阅通道（§2.1）、DeepSeek 三条通道的经济性（§3）、`alibaba-token-plan`（已禁用）四条坑。**本节不复述**。
- **Python 子进程默认按 cp936 解码，会把非 ASCII 读成乱码**：本机 `locale.getpreferredencoding()` = `cp936`，`subprocess.run(..., text=True)` 不加 `encoding` 时读子进程的 UTF-8 中文会变乱码 —— 实测模型回 `收到` 而显示成 `鏀跺埌`（`'收到'.encode('utf-8').decode('gbk')` 可还原）。**读子进程输出一律显式 `encoding="utf-8"`**（shell 里的 `cat`/`rg` 不受影响）。
- **`ast_grep` 已启用**（原生实现、支持 `csharp`、免安装）。注意它的输出在 hashline 编辑模式下只渲染到模式的字面前缀：`class $N` 的命中行显示成 `*46|class`，不带上下文 → 当**定位器**用（拿 file+line），内容再 `read` 看。
- **`配置导出.bat` 的中文行会吃掉下一行**：文件是 UTF-8、无 BOM、CRLF。代码页 65001 下，一行中文紧挨着下一行时，cmd 会把下一行拆开。每一行中文后面空一行。行尾 `^` 续行，`^` 后面不能有空格。
- **Luban 的 `manager` 留空会生成 `.cs`**：类声明变成 `public partial class `，文件名也是空的。Game 写 `Tables`，Start 写 `StartTables`。两套 C# 落在同一个 `Output/CSharp/{c,s}`，同名 `partial` 的构造器对不上。Start 导出必须 `cleanUpOutputDir=0`，否则清掉已经写出的 Game 代码。
- **`cs-bin` 模板里命名空间内的类少一层缩进**：`ExcelTool/Luban/Templates/cs-bin` 的 `tables.sbn`、`table.sbn`、`bean.sbn` 已在命名空间内部整体加了 4 格。换官方模板时这层会丢。
- **Inspector 顶部标签**：用 `Editor.finishedDefaultHeaderGUI`，可以给所有 Inspector 塞 toggle/label。
- **ToolKit 工具页 `ScriptableSingleton` 类名要和文件名一致**：`ScriptInitSetting` 类在 `ScriptInitSettings.cs`（差个 s）→ Unity 找不到 MonoScript，`CreateAsset` 永远失败，`OnWillCreateAsset` 里访问 `Inst` 重入 → 无限递归 StackOverflow（改文件名 + `LoadOrCreate` 里 `CreateAsset` 前先赋值 `mInstance` 防重入）。
- **`ConfigCopyTool` 的拷贝目标目录会被清空重拷，别让它和 asmdef 同层**：`RuntimeDst` 要指到 `Scripts/ThirdParty/Luban`（子目录），asmdef 在 `ThirdParty` 根。踩过：`RuntimeDst` 写成 `Scripts/ThirdParty`（根），运行后清空了 `WorldTree.ThirdParty.asmdef` + `Luban/` 子目录，`HotFix` 引用找不到报一串 `CS0246`。
- **打 YooAsset 包前确认 bytes 不是空文件**：`Assets/AssetBundles/Config/StartConfig/Client/worldlineconfig.bytes` 曾只剩 1 字节 `0x00`。Player 能读到表、没有异常、也不打 `创建世界线`，因为 Luban 把 `0x00` 读成 0 行。导出的原文件在 `ExcelTool/Output/Bytes/c/StartConfig/Client/`，正常是 25 字节。拷贝之后再打资源包。

### 4.3 运行时与源生成器

- **BCL `Task` 完成后的恢复要等下一次心跳 tick**：`INode.GetAwaiter(Task)` / `TreeTaskLink` 完成后都走 `WorldContext.Post`，而该队列由 `UpdateRule` **每帧整队排空**。心跳 `frameTime=1000ms` 时，一个 300ms 的 `Task` 实测 **1003ms** 才恢复 → 别拿它做毫秒级计时。
- **`Console/Server` 必须在 `Bin` 下启动**：`WorldLog` 只在 `../ProjectConfig/NLog` 存在时才建 NLog logger（配置 `Console/Server/ProjectConfig/NLog/NLog.config`，日志 `Console/Server/Logs/Log.txt`，同时打到控制台）。在仓库根跑时 logger 为 null → `Log`/`Error` 空引用，而心跳线程上这个 NRE 会**盖住真正的异常**。本 shell 不把当前目录当命令路径（`App.exe` / `./App.exe` 都 command not found）→ 用 `Console/Server/Bin/App.exe`（cwd 仍设 Bin）；`WorldHeart` 的线程不退出，看完日志 `taskkill /F /IM App.exe`。
- **自定义节点的 `XxxProxyRule` 不要回调 `self.OnBeforeDispose()` / `self.Dispose()`**：覆盖方法进代理、代理再调回覆盖方法会无限递归（视图壳踩过，StackOverflow）。代理里直接写实际逻辑（如 `INodeProxy.OnBeforeDispose` 的 `BeforeRemoveRuleGroup?.Send(self)`）。
- **`INodeProxy.OnCreate` 不能删**：直接实现 `INode` 的类型由 `INodeProxyGenerator` 生成 `INodeProxyRule.OnCreate(this)`（编号和 `SupportNodeRule` 已不在这里）。
- **`SupportNodeRule` 每次都进写锁**（类型已支持也一样）→ 热路径（每次 `PoolGetUnit`）不要加；但读档的法则查找发生在 `PoolGetNode` **之前**，`ReadValue` 里的调用不能省。
- **生成器静态集合不能跨程序集共享**：`RuleSupplementHelper` 等用静态字典收集再 `AddSource`，而 MyGenAll 的 IL2CPP 构建用 `/shared` 编译器服务器**并行**编 `CoreUnity`/`Module`/`HotFix`，字典串台后会把 Module 的 `InputBindSupplement` 写进 CoreUnity（CoreUnity 不引用 Module → CS0246），接着 `GenerateStripedAOTDlls failed`（编辑器/VS 分开编所以能过）。已让 `SourceGeneratorBase.Execute` 加锁串行。**改 `Unity.Analysis.dll` 后必须重启 Unity**，否则旧编译器服务器还握着旧 dll。程序集名单：Unity 侧 `UnityTool/Analysis/ProjectGeneratorSetting.cs`（与 asmdef 对得上），.NET 侧 `Framework/SourceGeneratorCore/ProjectGeneratorSetting.cs`（`WorldTree.Core` / `WorldTree.Module`），**别混**。
- **改程序集 `AssemblyName` 要同步 `ProjectGeneratorSetting` 名单**：`SourceGeneratorBase` 用 `Compilation.AssemblyName` 匹配 `ProjectGeneratorSetting` 里的名单，`AssemblyName` 改了不同步名单，生成器整包跳过、法则基类全不生成（报一串 CS0246 找不到 `XxxRule<>`）。改 `AssemblyName` 后记得改名单。
- **跨平台视图组件别用跟 uGUI 同名的裸名**：WorldTree 的 `Text`/`Image`/`Button` 跟 `UnityEngine.UI.Text/Image/Button` 重名。之前 Module 是 asmdef（AOT asmdef 不引用它，不冲突）；Module 移出编成 dll 放 `Assets/Plugins`（现 `Assets/Scripts/AOT/Plugins`）后，`overrideReferences:false` 的 asmdef 自动引用它，类型解析冲突（`GameEntry` 报 `error CS1061: 'Text' does not contain 'text'`）。已改名 `ViewText`/`ViewImage`/`ViewButton`。
- **`ISourceGenerator` 的 `ISyntaxReceiver` 在 Roslyn 4.x 下失效**：`RegisterForSyntaxNotifications` 注册的 receiver，`context.SyntaxReceiver` 在 .NET 6+ 增量生成器框架下恒为 null（官方已把 `ISourceGenerator`/`ISyntaxReceiver` 标 obsolete，推荐 `IIncrementalGenerator`）。dotnet 8/10 都复现，升级分析器 `Microsoft.CodeAnalysis` 版本无效。症状：用 `ISyntaxReceiver` 收集数据的生成器静默 return、强类型方法（`GetParent`/`AddComponent`/法则基类）全不生成，宿主报 CS0246/CS1061；只有直接遍历 `Compilation.SyntaxTrees` 的 `RuleMethodGenerator` 正常。解法：数据收集改成遍历 `context.Compilation.SyntaxTrees`（加了 `SyntaxReceiverHelper.Fill` 手动填充，12 个生成器已改），8 个 `ISyntaxReceiver` 类保留复用 `OnVisitSyntaxNode`。
- **同程序集声明的法则接口，`[NodeRule(nameof(XxxRule<...>))]` 会静默跳过**：`RuleMethodGenerator` 用 `GetSymbolInfo` 解析基类，而 `XxxRule<N>` 由 `RuleSupplementGenerator` 在**同一次编译**里生成，生成器之间互相看不到输出。**已支持接口形式 `[NodeRule(nameof(Open))]`**：节点类型取方法第一个参数（要求 `N : INode, AsRule<接口>`），生成器按名字继承 `RuleSupplementGenerator` 生成的基类 `OpenRule<N>`（Call/Async 同理，字段 `RuleClassData.BaseTypeText`），类型层次与手写规则一致；`SendRule<N,R>` 只用于推导 `Execute` 参数。**类形式已删除**：生成器和分析器只剩接口判断，`[NodeRule(nameof(XxxRule<...>))]` 不再支持。迁移映射：`AddRule<T>`→`Add`、`RemoveRule<T>`→`Remove`、`UpdateRule<T>`→`Update`、`UpdateTimeRule<T>`→`UpdateTime`、`AwakeRule<T,P…>`→`Awake<P…>`、`EnableRule<T>`→`Enable`、`DisableRule<T>`→`Disable`、`TestNodeEventRule<N,X>`→`TestNodeEvent<X>`。 静态规则类可标 `[NodeRuleTarget(nameof(节点))]`：分析器在方法不存在（特性误置/补全）时用它兜底取 self 类型。解析顺序是 **方法第一个参数 → 类上的 `[NodeRuleTarget]` → 包含类本身（实现 INode）→ 默认 `INode`（生成占位；生成器不校验约束，`SendRule<INode, Open>` 会报 CS0311，手改成真实节点即可）**；签名和类标签都有且冲突会报 `NodeRule法则方法参数错误修复`。注意 `GeneratorHelper.INode` 是全名 `WorldTree.INode`，而 `TryGetInterfaceName` 比的是短名，别混用。 `NodeRuleClassAnalysis`（孤立类型）同时认接口 `Open`（IdentifierName）和规则类 `OpenRule<T>`（GenericName），代码修复按接口生成 `[NodeRule(nameof(Open))]`，N 取类上的 `[NodeRuleTarget]` 或包含类。

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

### 分层

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

**`unity_refresh` 的时序**：**响应先写回**，再执行 `AssetDatabase.Refresh()` —— 刷新触发域重载、服务器随之被杀，靠 `[InitializeOnLoad]` + `AssemblyReloadEvents.beforeAssemblyReload` 停/起，监听在 `EditorApplication.delayCall` 里绑定。

**`unity_select`**：未命中时按名字搜已加载**场景**对象（含未激活，用 `scene.IsValid()` 排除预制体等资源），命中多个时返回候选路径；选中后 `EditorGUIUtility.PingObject` 在 Hierarchy 高亮。

**协议边界**：GET → **405**（客户端据此静默禁用 SSE 后台监听）、通知 → **202**、`ping` → `{}`、未知方法 → **-32601**、坏 JSON → **-32700**、未知工具或缺参 → `isError:true`、DELETE → **204**。`UnityMCP.Editor.asmdef` 是 `overrideReferences: true`，`precompiledReferences` 只有 `Newtonsoft.Json.dll`；注入代码靠运行时已加载程序集动态引用，不直接引 `WorldTree.Core.dll`。

**不要加 UI 状态窗口，也不要实现 SSE 推送**：MCP 是纯服务端拉取（无持久连接、无心跳、无重连、不回调客户端），客户端断连不会刷屏；要补的是仪表（请求数／耗时／最近调用 → `unity_status` 工具），不是窗口。

**限制与坑**：

- **`CSharpCodeProvider` 是老编译器**（Unity 2021 自带）：别用太新 C# 语法；返回值被 `ret?.ToString()` **拍平成一个字符串**，要结构化数据自己在注入代码里 `JsonConvert.SerializeObject`。
- **引用集规则**（`CSharpExecutor`）：`CSharpCodeProvider` **隐式带了一套核心 BCL**，所以**显式引用 `mscorlib` 会重复定义** —— 症状是 ~105 条 `CS1685`，一旦触碰跨门面类型还会 `CS0433`（`The imported type 'System.AppDomain' is defined multiple times`）。**但 `netstandard` 不能跳** —— `WorldTree.Core` 等 netstandard2.1 程序集的类型要靠它解析身份，跳掉会得到 `CS0012`。做法：按**程序集简单名去重** + 跳过 `mscorlib` / `System.Runtime` / `System.Private.CoreLib`（**保留 `netstandard`**），遍历时 `if (asm.IsDynamic) continue;`。
- **`.unitymcp/refs.json` 是引用白名单**：放一个程序集简单名数组即可临时限制引用集（不用改代码就能二分引用集问题；**用完记得删掉**）。
- **没有看门狗**：注入死循环会让编辑器永久卡死，通道也救不回来。长任务拆小。
- **MCP 监听不能开 `ReuseAddress`**：Windows 上 `SO_REUSEADDR` 允许新旧两个 socket 同时 bind 同一端口。域重载后旧 accept 线程已退出，新连接可能落到那个死 socket 上 → 端口在 `LISTENING`、请求永不回包，`netstat` 里同一 PID 出现两行 `LISTENING`。已改成 `ExclusiveAddressUse`（`SO_EXCLUSIVEADDRUSE`）+ `Stop()` 里 `Join` accept 线程，并关闭所有已接受的连接（`clients`；只关监听不够，`CLOSE_WAIT` 连接也会占住端点），保证域卸载前 socket 已关闭。改完必须重启一次 Unity 进程，清掉旧代码留下的僵尸监听。
- **可以自己触发重编译**：`unity_refresh` 之后判据是 `Library/ScriptAssemblies/UnityMCP.Editor.dll` 的 mtime 更新（实测 3~7s）。**编译失败时旧 dll 仍在内存里，服务器照常活着**，改完再刷即可 —— 这是安全的调试姿势。
- **端口 8099 只绑回环**，无鉴权时也只对本机开放。

## 6. 维护约定

- **模型不许自动提交**：绝不运行 `git commit`（含 `git -C … commit` 等任何等价形式）。改动做完就停下报告，由作者审阅后**手动提交**。硬保险：`~/.omp/agent/config.yml` 的 `bash.patterns` 已把 `git commit*` 设为 `deny`，`yolo` 模式下同样生效。
- 用法写进第 2 节，写成怎么用，不写完成记录。
- 方法摘要只留一句。长说明写 `<remarks>`。
- 宿主和 OMP 提供的东西写文首。不要把架构抄进技能或始终生效的规则；TTSR 只在同一种失误重复出现时加。
- 还没做完的事写第 3 节。新坑追加到第 4 节。
- 保持精炼：目标是 2 分钟内能 re-onboard。
