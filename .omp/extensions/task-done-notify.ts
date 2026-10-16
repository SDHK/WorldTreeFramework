/**
 * omp 扩展：AI 干完活（一轮跑完）时弹 Windows 桌面通知 + 提示音。
 *
 * 触发：agent_start → agent_end（一个用户回合），过滤短于 MIN_TURN_MS 的回合 + COOLDOWN_MS 冷却。
 *       （不要用 turn_start 做锚点：每次工具调用后它都会再触发。）
 * 内容：标题行 `omp · <会话名>` —— 会话名取 `pi.getSessionName()`，无标题时退化为
 *       `<cwd目录名> · <会话id前8位>`；正文行 `已完成 · N 秒 · <本轮摘要>`。
 *       摘要 = 本轮最后一条助手文本的首句（退化到最后一个工具调用的 `intent`），剥 markdown、截 40 字。
 * 派发：**await** PowerShell 跑完再返回 —— 之前用 detached spawn，日志显示子进程没跑完
 *       （可能是 omp 在 handler 返回后清理子进程树），改为在 handler 内等待，代价是收尾多 ~1 秒。
 *       toast() 会回传退出码 / stderr，写进日志便于定位。
 * 后端：PowerShell 5.1 + WinRT toast，AUMID 借 Windows PowerShell 自己的（系统必有，零依赖）。
 * 日志：~/.omp/logs/omp-notify.log（含加载自检、触发、派发结果）
 *        ~/.omp/logs/omp-notify-shot.log（PowerShell 侧落点，证明脚本跑到 Show() 之后）
 * 关闭：删本文件 / config.yml `disabledExtensions: [task-done-notify]` / `PI_NOTIFICATIONS=off`
 * 手动测：会话里输入 `/notify-test`
 *
 * 装完要**重启 omp**（扩展在启动时发现；Windows 上连同进程重载也不生效）。
 * toast 不出现先查 Windows 专注助手/勿扰 + 通知设置里是否允许 Windows PowerShell。
 */
import { execFile } from "node:child_process";
import { appendFile, mkdir } from "node:fs/promises";
import { homedir } from "node:os";
import { join } from "node:path";
import type { ExtensionAPI } from "@oh-my-pi/pi-coding-agent";

const LOG_DIR = join(homedir(), ".omp", "logs");
const LOG = join(LOG_DIR, "omp-notify.log");
/** PowerShell 侧落点：证明脚本执行到了 Show() 之后。 */
const SHOT_LOG = join(LOG_DIR, "omp-notify-shot.log");

const MIN_TURN_MS = 20_000; // 短于此长度的回合不通知
const COOLDOWN_MS = 30_000; // 两条通知之间的最小间隔

/** WinRT toast（ToastText02 = 粗体标题 + 正文）。文案走环境变量，避开命令行转义问题。 */
const PS_TOAST = String.raw`
$ErrorActionPreference = 'Stop'
[void][Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime]
[void][Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime]
$xml = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::ToastText02)
$nodes = $xml.GetElementsByTagName('text')
[void]$nodes.Item(0).AppendChild($xml.CreateTextNode($env:OMP_NOTIFY_TITLE))
[void]$nodes.Item(1).AppendChild($xml.CreateTextNode($env:OMP_NOTIFY_BODY))
$notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier($env:OMP_NOTIFY_AUMID)
$notifier.Show([Windows.UI.Notifications.ToastNotification]::new($xml))
Add-Content -Path $env:OMP_NOTIFY_MARK -Value ((Get-Date).ToString('o') + " " + $env:OMP_NOTIFY_BODY) -Encoding UTF8 -ErrorAction SilentlyContinue
Write-Output 'shown'
`;

const POWERSHELL_AUMID =
  "{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}\\WindowsPowerShell\\v1.0\\powershell.exe";

async function writeLog(line: string): Promise<void> {
  try {
    await mkdir(LOG_DIR, { recursive: true });
    await appendFile(LOG, `${new Date().toISOString()} ${line}\n`);
  } catch {
    /* 日志失败不影响主流程 */
  }
}

/** 等 PowerShell 跑完并回传结果串（含退出码 / stderr），便于定位"没弹"是哪一环断的。 */
function toast(title: string, body: string): Promise<string> {
  const { promise, resolve } = Promise.withResolvers<string>();
  if (process.platform !== "win32") {
    resolve("skipped (non-win32)");
    return promise;
  }
  execFile(
    "powershell.exe",
    ["-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-Command", PS_TOAST],
    {
      timeout: 15_000,
      windowsHide: true,
      env: {
        ...process.env,
        OMP_NOTIFY_TITLE: title,
        OMP_NOTIFY_BODY: body,
        OMP_NOTIFY_AUMID: POWERSHELL_AUMID,
        OMP_NOTIFY_MARK: SHOT_LOG,
      },
    },
    (err, stdout, stderr) => {
      if (err) {
        const detail = `${err.message}${stderr ? ` | stderr: ${stderr.trim().slice(0, 200)}` : ""}`;
        resolve(`FAILED: ${detail}`);
        return;
      }
      resolve(`ok (stdout=${String(stdout).trim().slice(0, 40) || "empty"})`);
    },
  );
  return promise;
}

/** 会话标识：优先会话名（自动标题或 /rename 的），退化到 cwd 目录名，再退化到会话 id 前缀。 */
async function sessionLabel(
  pi: ExtensionAPI,
  ctx: { cwd?: string; sessionManager?: { getSessionId?: () => string } } | undefined,
): Promise<string> {
  try {
    // ExtensionAPI 上不一定每个版本都暴露 getSessionName → 可选调用，缺失就走退化路径
    const raw = await pi.getSessionName?.();
    // 卡片形如 `<图标> <代号>: `（title.icons=nf+emoji 生成）。只留图标、去掉代号单词 ——
    // omp 的 title.icons 只有 nf+emoji/emoji/boring 三档，没有"仅图标"，所以在这里剥。
    const name = String(raw ?? "")
      .replace(/^(\S{1,3})\s+[A-Z0-9]{2,6}:\s*/, "$1 ")
      .trim();
    if (name) return name.length > 60 ? `${name.slice(0, 59)}…` : name;
    const dir = String(ctx?.cwd ?? process.cwd())
      .split(/[\\/]+/)
      .filter(Boolean)
      .pop();
    const short = (ctx?.sessionManager?.getSessionId?.() ?? "").slice(0, 8);
    if (dir && short) return `${dir} · ${short}`;
    return dir || short || "omp";
  } catch {
    return "omp";
  }
}

/** 这一轮"干了什么"：取本轮最后一条助手文本的首句，退化到最后一个工具调用的 intent。 */
function summarizeTurn(
  ctx: { sessionManager?: { getBranch?: () => unknown } } | undefined,
): string {
  const branch = ctx?.sessionManager?.getBranch?.();
  if (!Array.isArray(branch)) return "";
  let text = "";
  let intent = "";
  for (let i = branch.length - 1; i >= 0; i--) {
    const entry = branch[i];
    if (!entry || typeof entry !== "object") continue;
    const message = "message" in entry ? entry.message : undefined;
    if (!message || typeof message !== "object") continue;
    const role = "role" in message ? message.role : undefined;
    if (role === "user") break; // 本轮边界：再往前就是上一轮了
    if (role !== "assistant") continue;
    const content = "content" in message ? message.content : undefined;
    if (!Array.isArray(content)) continue;
    for (const block of content) {
      if (!block || typeof block !== "object" || !("type" in block)) continue;
      if (!text && block.type === "text" && "text" in block && typeof block.text === "string") {
        text = block.text;
      }
      if (!intent && block.type === "toolCall" && "intent" in block && typeof block.intent === "string") {
        intent = block.intent;
      }
    }
    if (text && intent) break;
  }
  const line =
    (text || intent)
      .split("\n")
      .map((s) => s.replace(/[#*`>\s]+/g, " ").trim())
      .find((s) => s.length > 3) ?? "";
  return line.length > 40 ? `${line.slice(0, 39)}…` : line;
}

export default function taskDoneNotify(pi: ExtensionAPI) {
  let runStart = Date.now();
  let lastNotify = 0;

  pi.on("session_start", async () => {
    // 加载自检：后续从日志就能断定"扩展到底有没有被加载"
    await writeLog(`扩展已加载 (pid=${process.pid} platform=${process.platform})`);
  });

  pi.on("agent_start", async () => {
    runStart = Date.now();
  });

  pi.on("agent_end", async (_event, ctx) => {
    const elapsed = Date.now() - runStart;
    const since = Date.now() - lastNotify;
    const secs = Math.round(elapsed / 1000);

    if (elapsed < MIN_TURN_MS) {
      await writeLog(`agent_end 跳过（回合仅 ${secs}s）`);
      return;
    }
    if (since < COOLDOWN_MS) {
      await writeLog(`agent_end 跳过（冷却 ${Math.round(since / 1000)}s）`);
      return;
    }

    lastNotify = Date.now();
    const label = await sessionLabel(pi, ctx);
    const what = summarizeTurn(ctx);
    // 先落盘再弹：headless（omp -p）可能在 await 结束前就退出
    await writeLog(`agent_end 触发（${secs}s · 会话=${label} · 摘要=${what || "无"}）`);
    const body = what ? `已完成 · ${secs} 秒 · ${what}` : `已完成 · ${secs} 秒`;
    const result = await toast(`omp · ${label}`, body);
    await writeLog(`agent_end 派发 ${result}`);
  });

  pi.registerCommand("notify-test", {
    description: "发一条测试用的 Windows 桌面通知（验证通知通道与会话名）",
    handler: async (_args, ctx) => {
      const label = await sessionLabel(pi, ctx);
      const result = await toast(`omp · ${label}`, `通知通道测试 · ${new Date().toLocaleTimeString()}`);
      await writeLog(`手动测试 /notify-test → ${result}（会话=${label}）`);
      ctx?.ui?.notify?.(
        `会话名=${label}｜派发结果：${result}（没看到 toast 就查 Windows 专注助手 + 是否允许 Windows PowerShell）`,
        "info",
      );
    },
  });
}
