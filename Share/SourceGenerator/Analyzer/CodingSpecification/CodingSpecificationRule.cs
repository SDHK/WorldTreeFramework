/****************************************

* 作者：闪电黑客
* 日期：2024/6/19 11:33

* 描述：

*/

using Microsoft.CodeAnalysis;

namespace WorldTree.Analyzer
{
	public static class ClassNamingDiagnosticRule
	{
		private const string Title = "类命名规范诊断";
		private const string MessageFormat = "类: {0} 命名不规范";
		private const string Description = "类命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.ClassNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}

	public static class PublicFieldNamingDiagnosticRule
	{
		private const string Title = "公开字段命名规范诊断";
		private const string MessageFormat = "公共字段: {0} 命名不规范";
		private const string Description = "公开字段命名命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.PublicFieldNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}

	public static class PrivateFieldNamingDiagnosticRule
	{
		private const string Title = "私有字段命名规范诊断";
		private const string MessageFormat = "私有字段: {0} 命名不规范";
		private const string Description = "私有字段命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.PrivateFieldNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}

	public static class ProtectedFieldNamingDiagnosticRule
	{
		private const string Title = "保护字段命名规范诊断";
		private const string MessageFormat = "保护字段: {0} 命名不规范";
		private const string Description = "保护字段命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.ProtectedFieldNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}

	public static class PublicPropertyNamingDiagnosticRule
	{
		private const string Title = "公共属性命名规范诊断";
		private const string MessageFormat = "公共属性: {0} 命名不规范";
		private const string Description = "公共属性命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.PublicPropertyNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}

	public static class PrivatePropertyNamingDiagnosticRule
	{
		private const string Title = "私有属性命名规范诊断";
		private const string MessageFormat = "私有属性: {0} 命名不规范";
		private const string Description = "私有属性命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.PrivatePropertyNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}

	public static class ProtectedPropertyNamingDiagnosticRule
	{
		private const string Title = "保护属性命名规范诊断";
		private const string MessageFormat = "保护属性: {0} 命名不规范";
		private const string Description = "保护属性命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.ProtectedPropertyNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}

	public static class MethodNamingDiagnosticRule
	{
		private const string Title = "方法命名规范诊断";
		private const string MessageFormat = "方法: {0} 命名不规范";
		private const string Description = "方法命名不规范.";
		public const string CodeFixTitle = "[修复为规范命名]";
		public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticKey.MethodNaming.ToString(), Title, MessageFormat, DiagnosticCategories.CodingSpecification, DiagnosticSeverity.Error, true, Description);
	}
}