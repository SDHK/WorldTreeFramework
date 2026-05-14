using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;

namespace WorldTree
{
	public static partial class MainWorldRule
	{
		[NodeRule(nameof(AddRule<MainWorld>))]
		private static void OnAddRule(this MainWorld self)
		{
			self.Log($"配置表工具主世界启动！！");

			// 简单测试：读取 xlsx 第一个工作表
			string xlsxPath = Path.Combine(AppContext.BaseDirectory, @"..\Execl\Entry.xlsx");
			if (!File.Exists(xlsxPath))
			{
				self.Log($"未找到测试用工作簿（将 Config.xlsx 放到输出目录）: {xlsxPath}");
				return;
			}

			using var workbook = new XLWorkbook(xlsxPath);
			var sheet = workbook.Worksheets.First();
			var used = sheet.RangeUsed();
			if (used == null)
			{
				self.Log($"第一张表「{sheet.Name}」无已用单元格。");
				return;
			}

			int rowCount = used.RowCount();
			int colCount = used.ColumnCount();
			int firstRow = used.FirstRow().RowNumber();
			int firstCol = used.FirstColumn().ColumnNumber();
			self.Log($"第一张表: 「{sheet.Name}」 行={rowCount} 列={colCount} (起始 R{firstRow}C{firstCol})");

			const int previewRows = 5;
			int rowsToShow = Math.Min(rowCount, previewRows);
			for (int r = 0; r < rowsToShow; r++)
			{
				int rowIndex = firstRow + r;
				var cells = Enumerable.Range(0, colCount)
					.Select(c => sheet.Cell(rowIndex, firstCol + c).GetFormattedString())
					.ToArray();
				self.Log($"  行{rowIndex}: {string.Join(" | ", cells)}");
			}

			if (rowCount > previewRows)
				self.Log($"  … 共 {rowCount} 行，仅预览前 {previewRows} 行");
		}
	}
}