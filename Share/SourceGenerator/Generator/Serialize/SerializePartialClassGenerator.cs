/****************************************

* 作者：闪电黑客
* 日期：2024/8/6 16:43

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class SerializePartialClassGenerator
	{
		public static void Execute(GeneratorExecutionContext context, StringBuilder Code, TypeDeclarationSyntax typeDeclaration)
		{
			// 将 ClassDeclarationSyntax 转换为 INamedTypeSymbol
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);

			// 获取字段和属性，过滤掉 TreePackIgnore 标记的字段
			List<ISymbol> fieldSymbols = classSymbol.GetMembers()
				.Where(f =>
					{
						if (f is IFieldSymbol fieldSymbol && !fieldSymbol.IsStatic && !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst && !NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreePackIgnoreAttribute))
						{
							return true;
						}

						else if (f is IPropertySymbol propertySymbol && !propertySymbol.IsStatic && !propertySymbol.IsReadOnly && propertySymbol.GetMethod != null && propertySymbol.SetMethod != null && !NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreePackIgnoreAttribute))
						{
							return true;
						}
						return false;
					}
				)
				.ToList();

			// 获取类的完整名称，包括泛型参数
			string className = TreeSyntaxHelper.GetFullTypeName(typeDeclaration);
			if (typeDeclaration is ClassDeclarationSyntax)
			{
				Code.AppendLine($"	public partial class {className}");
			}
			else if (typeDeclaration is StructDeclarationSyntax)
			{
				Code.AppendLine($"	public partial struct {className}");

			}
			Code.AppendLine("	{");

			Code.AppendLine($"		class Serialize : SerializeRule<ByteSequence, {className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(ByteSequence self, ref {className} value)");
			Code.AppendLine("			{");
			foreach (ISymbol symbol in fieldSymbols)
			{
				if (symbol is IFieldSymbol fieldSymbol)
				{
					//判断是否是属性的后备字段
					if (fieldSymbol.AssociatedSymbol is IPropertySymbol propertySymbol) continue;

					if (fieldSymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
					{
						if (arrayTypeSymbol.ElementType.IsUnmanagedType)
							Code.AppendLine($"				self.WriteUnmanagedArray(value.{symbol.Name});");
						else
							Code.AppendLine($"				self.WriteArray(value.{symbol.Name});");
					}
					else
					{
						if (fieldSymbol.Type.IsUnmanagedType)
							Code.AppendLine($"				self.WriteUnmanaged(value.{symbol.Name});");
						else
							Code.AppendLine($"				self.WriteValue(value.{symbol.Name});");
					}
				}
				else if (symbol is IPropertySymbol propertySymbol)
				{
					if (propertySymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
					{
						if (arrayTypeSymbol.ElementType.IsUnmanagedType)
							Code.AppendLine($"				self.WriteUnmanagedArray(value.{symbol.Name});");
						else
							Code.AppendLine($"				self.WriteArray(value.{symbol.Name});");
					}
					else
					{
						if (propertySymbol.Type.IsUnmanagedType)
							Code.AppendLine($"				self.WriteUnmanaged(value.{symbol.Name});");
						else
							Code.AppendLine($"				self.WriteValue(value.{symbol.Name});");
					}
				}
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");


			Code.AppendLine($"		class Deserialize : DeserializeRule<ByteSequence, {className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(ByteSequence self, ref {className} value)");
			Code.AppendLine("			{");

			if (typeDeclaration is ClassDeclarationSyntax)
				Code.AppendLine($"				if (value == null) value = new();");

			foreach (ISymbol symbol in fieldSymbols)
			{
				if (symbol is IFieldSymbol fieldSymbol)
				{
					//判断是否是属性的后备字段
					if (fieldSymbol.AssociatedSymbol is IPropertySymbol propertySymbol) continue;

					if (fieldSymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
					{
						if (arrayTypeSymbol.ElementType.IsUnmanagedType)
							Code.AppendLine($"				self.ReadUnmanagedArray(ref value.{symbol.Name});");
						else
							Code.AppendLine($"				self.ReadArray(ref value.{symbol.Name});");
					}
					else
					{
						if (fieldSymbol.Type.IsUnmanagedType)
							Code.AppendLine($"				self.ReadUnmanaged(out value.{symbol.Name});");
						else
							Code.AppendLine($"				self.ReadValue(ref value.{symbol.Name});");
					}
				}
				else if (symbol is IPropertySymbol propertySymbol)
				{
					if (propertySymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
					{
						Code.AppendLine($"				{arrayTypeSymbol.ElementType}[] m{symbol.Name} = default;");

						if (arrayTypeSymbol.ElementType.IsUnmanagedType)
							Code.AppendLine($"				self.ReadUnmanagedArray(ref m{symbol.Name});");
						else
							Code.AppendLine($"				self.ReadArray(ref m{symbol.Name});");
					}
					else
					{
						if (propertySymbol.Type.IsUnmanagedType)
						{
							Code.AppendLine($"				self.ReadUnmanaged(out {propertySymbol.Type.Name} m{symbol.Name});");
						}
						else
						{
							Code.AppendLine($"				{propertySymbol.Type.Name} m{symbol.Name} = default;");
							Code.AppendLine($"				self.ReadValue(ref m{symbol.Name});");
						}
					}
					Code.AppendLine($"				value.{propertySymbol.Name} = m{symbol.Name};");
				}
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");

			Code.AppendLine("	}");
		}

	}
}
