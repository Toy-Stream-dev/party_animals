using System.Collections.Generic;

namespace GoogleParse.BalanceParse
{
	public class DataRow
	{
		public int Index { get; set; }
		public DataTable Table { get; set; }

		private List<DataCell> Cells { get; } = new List<DataCell>();
		public int CellsCount => Cells.Count;

		public DataRow(List<DataCell> cells)
		{
			foreach (var cell in cells)
			{
				Cells.Add(cell);
				cell.Index = Cells.Count - 1;
				cell.Row = this;
			}
		}

		public DataCell GetCell(int index)
		{
			return index < 0 || index >= Cells.Count ? null : Cells[index];
		}

		public override string ToString() => $"({Table.Workbook.Name}:{Table.Name}:{Index})";

		public int GetCommentCellIndex()
		{
			var commentCell = Cells.Find(c => c.IsComment);
			return commentCell != null ? Cells.IndexOf(commentCell) : -1;
		}

		public void RemoveCellAt(int cellIndex)
		{
			if (cellIndex < 0 || cellIndex >= Cells.Count) return;

			Cells.RemoveAt(cellIndex);
		}
	}
}