using System.IO;
using System.Linq;

namespace GoogleParse.BalanceParse
{
	public static class CSVReader
	{
		public static Workbook ParseCSV(string workbookName, string[] files, char valuesSeparator)
		{
			var workbook = new Workbook {Name = workbookName};

			foreach (var filePath in files)
			{
				var fileName = Path.GetFileNameWithoutExtension(filePath);
				var table = new DataTable()
				{
					Name = fileName
				};

				foreach (var row in File.ReadLines(filePath))
				{
					var dataRow = new DataRow(row.Split(valuesSeparator)
					                             .Select(data => new DataCell(data))
					                             .ToList());

					table.AddRow(dataRow);
				}

				table.ClearCommentRows();
				table.ClearCommentColumns();
				table.Workbook = workbook;
				workbook.Tables.Add(table);
			}

			return workbook;
		}
	}
}