using System.Collections.Generic;

namespace GoogleParse.BalanceParse
{
	public class DataTable
	{
		public string Name { get; set; }
		public Workbook Workbook { get; set; }

		private List<DataRow> Rows { get; } = new List<DataRow>();
		public int RowsCount => Rows.Count;

		public void AddRow(DataRow row)
		{
			Rows.Add(row);
			row.Index = Rows.Count - 1;
			row.Table = this;
		}

		public DataRow GetRow(int index)
		{
			return index < 0 || index >= Rows.Count ? null : Rows[index];
		}

		public override string ToString() => $"{Name} table";

		public void ClearCommentColumns()
		{
			if (RowsCount <= 1) return;

			var commentColumnIndex = Rows[0].GetCommentCellIndex();

			while (commentColumnIndex != -1)
			{
				foreach (var row in Rows)
				{
					row.RemoveCellAt(commentColumnIndex);
				}

				commentColumnIndex = Rows[0].GetCommentCellIndex();
			}
		}


		public void ClearCommentRows()
		{
			if (RowsCount <= 1) return;

			for (int i = RowsCount - 1; i > 0; i--)
			{
				if (Rows[i].CellsCount == 0 || !Rows[i].GetCell(0).IsComment) continue;
				Rows.RemoveAt(i);
			}
		}
	}
}