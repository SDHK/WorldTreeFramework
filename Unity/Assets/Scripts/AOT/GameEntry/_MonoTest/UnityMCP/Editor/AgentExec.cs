using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AgentExec
{
	/// <summary>
	/// 文件通道的 C# 代码执行器（供 DSH 助手调用）。
	/// 助手往 {工程根}/.dshexec/command.json 写命令，本类轮询执行，结果写回 result.json。
	/// 取代旧的 UnityMCP WebSocket 通道。
	/// </summary>
	[InitializeOnLoad]
	public static class AgentExec
	{
		private static readonly string CommandPath;
		private static readonly string ResultPath;

		private static string lastId;
		private static double nextPoll;

		static AgentExec()
		{
			string dir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, ".dshexec");
			Directory.CreateDirectory(dir);
			CommandPath = Path.Combine(dir, "command.json");
			ResultPath = Path.Combine(dir, "result.json");

			EditorApplication.update += Poll;
			Debug.Log($"[AgentExec] 文件通道已就绪，命令文件: {CommandPath}");
		}

		private static void Poll()
		{
			double now = EditorApplication.timeSinceStartup;
			if (now < nextPoll) return;
			nextPoll = now + 0.5;

			if (!File.Exists(CommandPath)) return;

			string raw = File.ReadAllText(CommandPath);
			if (string.IsNullOrWhiteSpace(raw)) return;

			Command cmd;
			try { cmd = JsonConvert.DeserializeObject<Command>(raw); }
			catch (Exception e)
			{
				Write(new Result { success = false, error = "命令 JSON 解析失败: " + e.Message });
				File.Delete(CommandPath);
				return;
			}

			if (cmd == null || string.IsNullOrEmpty(cmd.id) || cmd.id == lastId) return;

			lastId = cmd.id;
			Write(Execute(cmd));
			File.Delete(CommandPath);
		}

		private static Result Execute(Command cmd)
		{
			var r = new Result { id = cmd.id };
			var logs = new List<string>();
			Application.logMessageReceived += OnLog;
			try
			{
				object ret = CSEditorHelper.Execute(cmd.code);
				r.success = true;
				r.result = ret?.ToString() ?? "null";
			}
			catch (Exception e)
			{
				r.success = false;
				r.error = e.ToString();
			}
			finally
			{
				Application.logMessageReceived -= OnLog;
			}
			r.logs = logs;
			return r;

			void OnLog(string message, string stackTrace, LogType type) => logs.Add($"[{type}] {message}");
		}

		private static void Write(Result r)
		{
			File.WriteAllText(ResultPath, JsonConvert.SerializeObject(r, Formatting.Indented));
		}

		[Serializable]
		private class Command { public string id; public string code; }

		[Serializable]
		private class Result
		{
			public string id;
			public bool success;
			public string result;
			public string error;
			public List<string> logs;
		}

		/// <summary>
		/// 核心方法（保留自 UnityMCP）：内存编译一段 C# 并执行。
		/// 补充：引用所有已加载的程序集，使注入代码能使用 WorldTree 框架等任意类型。
		/// </summary>
		public static class CSEditorHelper
		{
			public static object Execute(string code)
			{
				string wrapped = $@"
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

public class CodeExecutor
{{
	public static object Execute()
	{{
		{code}
		return ""Success"";
	}}
}}
";
				var opts = new CompilerParameters { GenerateInMemory = true };
				AddRef(opts, typeof(object));
				AddRef(opts, typeof(Enumerable));
				AddRef(opts, typeof(GameObject));
				AddRef(opts, typeof(Editor));

				// 引用所有已加载的程序集，使注入代码能操作 WorldTree 框架等任意类型
				foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (asm.IsDynamic) continue; // 动态程序集没有 Location，跳过
					if (!string.IsNullOrEmpty(asm.Location))
					{
						opts.ReferencedAssemblies.Add(asm.Location);
					}
				}

				using (var provider = new CSharpCodeProvider())
				{
					var res = provider.CompileAssemblyFromSource(opts, wrapped);
					if (res.Errors.HasErrors)
					{
						string errs = string.Join("\n", res.Errors.Cast<CompilerError>().Select(e => e.ToString()));
						throw new Exception("编译失败:\n" + errs);
					}
					var type = res.CompiledAssembly.GetType("CodeExecutor");
					return type.GetMethod("Execute").Invoke(null, null);
				}
			}

			private static void AddRef(CompilerParameters opts, Type t)
			{
				string loc = t.Assembly.Location;
				if (!string.IsNullOrEmpty(loc))
				{
					opts.ReferencedAssemblies.Add(loc);
				}
			}
		}
	}
}
