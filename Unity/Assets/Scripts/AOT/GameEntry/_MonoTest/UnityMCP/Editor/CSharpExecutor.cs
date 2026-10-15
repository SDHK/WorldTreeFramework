/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： 内存编译并执行一段 C# 语句（Unity 编辑器内）

* 原为 AgentExec.cs 里的 CSEditorHelper（注释写着"保留自 UnityMCP"）。
* 文件通道移除后独立成文件，供 MCP 的 unity_exec 工具调用。
* 注意：它只负责"编译 + 执行"，不涉及任何传输（文件 / HTTP / MCP）。

* 关键经验（都是实测二分定位出来的，改回去会立刻踩坑）：
*   · 【必须跳过 mscorlib】CSharpCodeProvider 自己隐式带了一套核心 BCL，
*     显式再引它 → 105 条 CS1685 + CS0433（System.AppDomain 定义多次）。
*   · 【不能跳过 netstandard】WorldTree.Core 等 netstandard2.1 程序集的类型
*     要靠它解析身份，跳掉 → CS0012（System.IDisposable 定义在未引用的 netstandard）。
*   · 按程序集简单名去重：同一个程序集可能从两个位置各加载一份。
*   · 返回值会被调用方 ToString()，要结构化数据得自己在注入代码里序列化。

* 诊断：往 .unitymcp/refs.json 写一个程序集简单名数组即可临时充当引用白名单
*       （不用改代码就能二分引用集问题；**用完记得删掉**）。

****************************************/

using Microsoft.CSharp;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityMCP.Editor
{
	/// <summary>
	/// 把一段 C# 语句包进方法体，内存编译并执行
	/// </summary>
	public static class CSharpExecutor
	{
		/// <summary>
		/// 执行一段 C# 语句。代码会被包进 CodeExecutor.Execute() 的方法体，
		/// 可直接 return 一个值；返回值由调用方 ToString()。
		/// </summary>
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
			CompilerParameters opts = new CompilerParameters { GenerateInMemory = true };

			// 按「程序集简单名」去重
			HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			HashSet<string> skip = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"mscorlib",                 // 隐式已有，再引即重复定义（见文件头注释）
				"System.Runtime",
				"System.Private.CoreLib",
			};

			// 引用白名单（可选，仅诊断用；见文件头）
			HashSet<string> allow = ReadAllowList();

			// 根引用（同样受白名单裁剪，这样才能测"完全不显式引用"的情况）
			foreach (Type t in new[] { typeof(object), typeof(Enumerable), typeof(UnityEngine.GameObject), typeof(UnityEditor.Editor) })
			{
				string rn;
				try { rn = t.Assembly.GetName().Name; } catch (Exception) { continue; }
				if (allow != null && !allow.Contains(rn)) continue;
				AddRef(opts, seen, skip, t);
			}

			// 其余已加载程序集 —— 让注入代码能访问 WorldTree 等任意类型
			foreach (System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (asm.IsDynamic) continue;
				string asmName;
				string location;
				try { asmName = asm.GetName().Name; location = asm.Location; } catch (Exception) { continue; }
				if (allow != null && !allow.Contains(asmName)) continue;
				AddRef(opts, seen, skip, location, asmName);
			}

			using (CSharpCodeProvider provider = new CSharpCodeProvider())
			{
				CompilerResults res = provider.CompileAssemblyFromSource(opts, wrapped);
				if (res.Errors.HasErrors)
				{
					List<CompilerError> all = res.Errors.Cast<CompilerError>().ToList();
					IEnumerable<string> errs = all.Where(e => !e.IsWarning)
						.Select(e => e.ErrorNumber + " 行" + e.Line + ": " + e.ErrorText);
					IEnumerable<string> warns = all.Where(e => e.IsWarning)
						.Select(e =>
						{
							int i = e.ErrorText.LastIndexOf("definition from ", StringComparison.Ordinal);
							return e.ErrorNumber + ":" + (i >= 0 ? e.ErrorText.Substring(i + 16).Trim('`', '\'', ' ') : "?");
						})
						.GroupBy(s => s).Select(g => g.Key + " x" + g.Count());
					throw new Exception(
						"编译失败：错误 " + all.Count(e => !e.IsWarning) + " 个 / 警告 " + all.Count(e => e.IsWarning) + " 个\n"
						+ "已引用程序集(" + seen.Count + ")： " + string.Join(", ", seen.OrderBy(x => x)) + "\n"
						+ "错误：\n" + string.Join("\n", errs.Take(20)) + "\n"
						+ "警告汇总： " + string.Join(", ", warns.Take(10)));
				}
				Type type = res.CompiledAssembly.GetType("CodeExecutor");
				return type.GetMethod("Execute").Invoke(null, null);
			}
		}

		/// <summary>
		/// 读 .unitymcp/refs.json（程序集简单名数组）作为引用白名单；不存在/为空/解析失败返回 null（= 不限制）
		/// </summary>
		private static HashSet<string> ReadAllowList()
		{
			try
			{
				string p = UnityMcpPaths.RefsFile;
				if (!File.Exists(p)) return null;
				List<string> names = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(p));
				if (names == null || names.Count == 0) return null;
				return new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
			}
			catch (Exception) { return null; }
		}

		private static void AddRef(CompilerParameters opts, HashSet<string> seen, HashSet<string> skip, Type t)
		{
			if (t == null) return;
			System.Reflection.Assembly a = t.Assembly;
			AddRef(opts, seen, skip, a.Location, a.GetName().Name);
		}

		private static void AddRef(CompilerParameters opts, HashSet<string> seen, HashSet<string> skip, string location, string name)
		{
			if (string.IsNullOrEmpty(location)) return;
			if (string.IsNullOrEmpty(name)) name = location;
			if (skip.Contains(name)) return;   // 重复定义预定义类型的门面
			if (!seen.Add(name)) return;        // 同一个程序集已引用过
			opts.ReferencedAssemblies.Add(location);
		}
	}
}
