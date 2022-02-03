using System.Collections.Generic;

namespace GoogleParse.BalanceParse
{
	public class Workbook
	{
		public List<DataTable> Tables { get; } = new List<DataTable>();
		public string Name { get; set; }

		public DataTable GetTable(string name) => Tables.Find(table => table.Name == name);

		public override string ToString() => $"{Name} workbook";
	}
}