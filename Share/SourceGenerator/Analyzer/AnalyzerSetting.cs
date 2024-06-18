/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:42

* 描述：

*/

using System;
using System.Collections.Generic;
using System.Text;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 项目命名规则检测枚举
	/// </summary>
	[Flags]
	public enum ProjectAnalyzer
	{
		/// <summary>
		/// 类名
		/// </summary>
		Class = 0,

		/// <summary>
		/// 公共字段名
		/// </summary>
		PublicField = 1,

		/// <summary>
		/// 私有字段名
		/// </summary>
		PrivateField = 1 << 1,

		/// <summary>
		/// 保护字段名
		/// </summary>
		ProtectedField = 1 << 2,

		/// <summary>
		/// 公共属性名
		/// </summary>
		PublicProperty = 1 << 3,

		/// <summary>
		/// 私有属性名
		/// </summary>
		PrivateProperty = 1 << 4,

		/// <summary>
		/// 保护属性名
		/// </summary>
		ProtectedProperty = 1 << 5,

		/// <summary>
		/// 方法名
		/// </summary>
		Method = 1 << 6,
	}

	/// <summary>
	/// 分析器设置
	/// </summary>
	public static class AnalyzerSetting
	{
		/// <summary>
		/// 项目分析器项
		/// </summary>
		public static Dictionary<string, ProjectAnalyzer> ProjectAnalyzers = new Dictionary<string, ProjectAnalyzer>()
		{
			{ "App", (ProjectAnalyzer) ~0},
			{ "DotNet.Core", (ProjectAnalyzer) ~0},
		};

		//!Regex.IsMatch(fieldName, "^[A-Z][a-zA-Z]*$")

		public static Dictionary<ProjectAnalyzer, Func<string, bool>> AnalyzerChecks = new Dictionary<ProjectAnalyzer, Func<string, bool>>()
		{
			{ ProjectAnalyzer.Class, s => s.Length > 0 && char.IsUpper(s[0]) },
			{ ProjectAnalyzer.PublicField, s => s.Length > 0 && char.IsUpper(s[0]) },
			{ ProjectAnalyzer.PrivateField, s => s.Length > 0 && char.IsLower(s[0]) },
			{ ProjectAnalyzer.ProtectedField, s => s.Length > 0 && char.IsUpper(s[0]) },
			{ ProjectAnalyzer.PublicProperty, s => s.Length > 0 && char.IsUpper(s[0]) },
			{ ProjectAnalyzer.PrivateProperty, s => s.Length > 0 && char.IsLower(s[0]) },
			{ ProjectAnalyzer.ProtectedProperty, s => s.Length > 0 && char.IsUpper(s[0]) },
			{ ProjectAnalyzer.Method, s => s.Length > 0 && char.IsUpper(s[0]) },
		};

		public static Dictionary<ProjectAnalyzer, Func<string, string>> AnalyzerCodeFix = new Dictionary<ProjectAnalyzer, Func<string, string>>()
		{
			{ ProjectAnalyzer.Class, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ ProjectAnalyzer.PublicField, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ ProjectAnalyzer.PrivateField, s => char.ToLower(s[0]) + s.Substring(1) },
			{ ProjectAnalyzer.ProtectedField, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ ProjectAnalyzer.PublicProperty, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ ProjectAnalyzer.PrivateProperty, s => char.ToLower(s[0]) + s.Substring(1) },
			{ ProjectAnalyzer.ProtectedProperty, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ ProjectAnalyzer.Method, s => char.ToUpper(s[0]) + s.Substring(1) },
		};
	}
}