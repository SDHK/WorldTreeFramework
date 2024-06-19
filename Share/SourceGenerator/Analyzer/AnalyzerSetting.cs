/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:42

* 描述：

*/

using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 分析器设置
	/// </summary>
	public static class AnalyzerSetting
	{
		/// <summary>
		/// 项目分析器配置
		/// </summary>
		/// <remarks>启用项目名称，枚举检测开关</remarks>
		public static Dictionary<string, DiagnosticKey> ProjectAnalyzers = new Dictionary<string, DiagnosticKey>()
		{
			{ "App", (DiagnosticKey) ~0},
			{ "DotNet.Core", (DiagnosticKey) ~0},
		};

		//!Regex.IsMatch(fieldName, "^[A-Z][a-zA-Z]*$")

		/// <summary>
		/// 分析器检查委托
		/// </summary>
		public static Dictionary<DiagnosticKey, Func<string, bool>> AnalyzerChecks = new Dictionary<DiagnosticKey, Func<string, bool>>()
		{
			{ DiagnosticKey.ClassNaming, s => Regex.IsMatch(s, "^[A-Z].*$") },
			{ DiagnosticKey.PublicFieldNaming, s => Regex.IsMatch(s, "^[A-Z].*$") },
			{ DiagnosticKey.PrivateFieldNaming, s => Regex.IsMatch(s, "^[a-z].*$") },
			{ DiagnosticKey.ProtectedFieldNaming, s => Regex.IsMatch(s, "^[a-z].*$") },
			{ DiagnosticKey.PublicPropertyNaming, s => Regex.IsMatch(s, "^[A-Z].*$") },
			{ DiagnosticKey.PrivatePropertyNaming, s => Regex.IsMatch(s, "^[A-Z].*$") },
			{ DiagnosticKey.ProtectedPropertyNaming, s => Regex.IsMatch(s, "^[A-Z].*$") },
			{ DiagnosticKey.MethodNaming, s => Regex.IsMatch(s, "^[A-Z].*$") },
		};

		/// <summary>
		/// 分析器代码修复委托
		/// </summary>
		public static Dictionary<DiagnosticKey, Func<string, string>> AnalyzerCodeFix = new Dictionary<DiagnosticKey, Func<string, string>>()
		{
			{ DiagnosticKey.ClassNaming, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ DiagnosticKey.PublicFieldNaming, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ DiagnosticKey.PrivateFieldNaming, s => char.ToLower(s[0]) + s.Substring(1) },
			{ DiagnosticKey.ProtectedFieldNaming, s => char.ToLower(s[0]) + s.Substring(1) },
			{ DiagnosticKey.PublicPropertyNaming, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ DiagnosticKey.PrivatePropertyNaming, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ DiagnosticKey.ProtectedPropertyNaming, s => char.ToUpper(s[0]) + s.Substring(1) },
			{ DiagnosticKey.MethodNaming, s => char.ToUpper(s[0]) + s.Substring(1) },
		};
	}
}