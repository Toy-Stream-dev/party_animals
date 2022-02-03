namespace GoogleParse.BalanceParse
{
	public class DataCell
	{
		public int Index { get; set; }
		public DataRow Row { get; set; }

		private string RawData { get; }
		public bool IsBlank => string.IsNullOrEmpty(RawData);
		public bool IsComment => RawData.StartsWith("#");
		public string StringValue => RawData;

		public DataCell(object data)
		{
			RawData = data.ToString();
		}

		public override string ToString() => Row != null ? Row.ToString().Replace(")", $":{Index})") : base.ToString();
	}
}