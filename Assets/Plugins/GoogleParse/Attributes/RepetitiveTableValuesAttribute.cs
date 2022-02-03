using System;

namespace GoogleParse.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
	public class RepetitiveTableValuesAttribute : Attribute
	{
		public int Count { get; }
		public bool InsertDefault { get; }

		public RepetitiveTableValuesAttribute(int count, bool insertDefault = false)
		{
			Count = count;
			InsertDefault = insertDefault;
		}
	}
}