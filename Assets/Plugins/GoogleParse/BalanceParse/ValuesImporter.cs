
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoogleParse.Attributes;
using UnityEngine;

namespace GoogleParse.BalanceParse
{
	public static class ValuesImporter
	{
		static List<string> GetFieldNamesFromTableHeader(DataTable sheet)
		{
			var headerRow = sheet.GetRow(0);

			var fieldNames = new List<string>();
			for (var i = 0; i < headerRow.CellsCount; i++)
			{
				var cell = headerRow.GetCell(i);
				if (cell == null || cell.IsBlank || cell.IsComment) break;
				fieldNames.Add(cell.StringValue);
			}

			return fieldNames;
		}

		static object CreateEntityFromRow(DataRow row, List<string> columnNames, Type entityType, string sheetName)
		{
			var entity = Activator.CreateInstance(entityType);

			var column = 0;
			for (var i = 0; i < columnNames.Count; i++)
			{
				var entityField = entityType.GetField(columnNames[i],
				                                      BindingFlags.Instance | BindingFlags.Public |
				                                      BindingFlags.NonPublic);
				if (entityField == null) continue;
//			if (!entityField.IsPublic && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0)
//				continue;

				if (entityField.GetCustomAttributes(typeof(SkipTableValueAttribute), false).Length != 0) continue;

				if (entityField.GetCustomAttributes(typeof(RepetitiveTableValuesAttribute), false)
				               .FirstOrDefault() is RepetitiveTableValuesAttribute repetitive)
				{
					var listInfo = CreateList(entityField);

					var list = listInfo.list;
					var listAddMethod = listInfo.addMethod;
					var valuesType = listInfo.valueType;

					var lastIndex = Math.Min(column + repetitive.Count, row.CellsCount);
					if (repetitive.InsertDefault)
					{
						listAddMethod?.Invoke(list, new[] {EntityConverter.Convert(valuesType, string.Empty)});
					}

					for (; column < lastIndex; column++)
					{
						listAddMethod?.Invoke(list, new[] {GetCellValue(row, column, valuesType)});
					}

					entityField.SetValue(entity, list);
					continue;
				}

				if (entityField.FieldType.IsGenericType &&
				    entityField.FieldType.GetGenericTypeDefinition() == typeof(List<>))
				{
					entityField.SetValue(entity, ParseFieldList(entityField, row, column++));
					continue;
				}

				try
				{
					entityField.SetValue(entity, GetCellValue(row, column++, entityField.FieldType));
				}
				catch (Exception e)
				{
					Debug.Log(e);
					throw;
				}
			}

			return entity;
		}

		static (object list, MethodInfo addMethod, Type valueType) CreateList(FieldInfo field)
		{
			var valuesType = field.FieldType.GetGenericArguments().FirstOrDefault();
			var listType = typeof(List<>).MakeGenericType(valuesType);
			var addMethod = listType.GetMethod("Add", new[] {valuesType});
			var list = Activator.CreateInstance(listType);

			return (list, addMethod, valuesType);
		}

		static object ParseFieldList(FieldInfo field, DataRow row, int cellIndex)
		{
			var listInfo = CreateList(field);

			var list = listInfo.list;
			var listAddMethod = listInfo.addMethod;
			var valuesType = listInfo.valueType;

			var str = row.GetCell(cellIndex)?.StringValue;
			if (string.IsNullOrEmpty(str)) return list;

			var values = str.Contains(';') || str.Contains(',') ? str.Split(';', ',') : new[] {str};

			if (values == null) return list;

			foreach (var strValue in values)
			{
				var s = strValue.StartsWith(" ") ? strValue.Remove(0, 1) : strValue;

				listAddMethod?.Invoke(list, new[] {EntityConverter.Convert(valuesType, s)});
			}

			return list;
		}

		static object GetCellValue(DataRow row, int cellIndex, Type entityType)
		{
			var cell = row.GetCell(cellIndex);
			return EntityConverter.Convert(entityType, cell?.StringValue);
		}

		static object GetEntityListFromSheet(DataTable table, Type entityType)
		{
			var workbookColumnNames = GetFieldNamesFromTableHeader(table);

			var entityFields = entityType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (!entityFields.All(info => workbookColumnNames.Contains(info.Name)))
			{
				workbookColumnNames = entityFields.Select(info => info.Name).ToList();
			}

			var listType = typeof(List<>).MakeGenericType(entityType);
			var listAddMethod = listType.GetMethod("Add", new[] {entityType});
			var list = Activator.CreateInstance(listType);

			// row of index 0 is header
			for (var i = 1; i <= table.RowsCount; i++)
			{
				var row = table.GetRow(i);

				var entryCell = row?.GetCell(0);
				if (entryCell == null || entryCell.IsBlank || entryCell.IsComment) continue;

				var entity = CreateEntityFromRow(row, workbookColumnNames, entityType, table.Name);
				listAddMethod?.Invoke(list, new[] {entity});
			}

			return list;
		}

		public static int Import(object target, Workbook book)
		{
			if (target == null) return 0;

			var sheetCount = 0;

			var targetFields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			// if (targetFields.Length > 0) спросить Сашу
			// {
			// 	var existingFields = targetFields.Count(t => GetTable(t, book) != null);
			// 	if (existingFields == 1)
			// 	{
			// 		return 0; //долго объяснять
			// 	}
			// }

			foreach (var field in targetFields)
			{
				if (field.FieldType.IsValueType) continue;

				if (field.FieldType.GetCustomAttributes(typeof(SkipTableAttribute), false).Length != 0)
				{
					continue;
				}

				var sheet = GetTable(field, book);
				if (sheet == null)
				{
					sheetCount += Import(field.GetValue(target), book);
					continue;
				}

				var fieldType = field.FieldType;

				if (!fieldType.IsGenericType)
				{
					if (!fieldType.IsArray) continue;

					if (fieldType.IsAssignableFrom(typeof(float[])))
					{
						field.SetValue(target, TryGetFloatValuesFromSheet(sheet, true));
						sheetCount++;
					}
					else if (fieldType.IsAssignableFrom(typeof(int[])))
					{
						field.SetValue(target, TryGetIntValuesFromSheet(sheet, true));
						sheetCount++;
					}

					continue;
				}

				if (fieldType.GetGenericTypeDefinition() != typeof(List<>)) continue;

				var types = fieldType.GetGenericArguments();
				var entityType = types[0];

				var entities = entityType == typeof(float)
					               ? TryGetFloatValuesFromSheet(sheet, false)
					               : entityType == typeof(int)
						               ? TryGetIntValuesFromSheet(sheet, false)
						               : GetEntityListFromSheet(sheet, entityType);

				field.SetValue(target, entities);
				sheetCount++;
			}

			if (CheckIfCanParseWholeSheet(target, book))
			{
				sheetCount++;
			}

			if (sheetCount > 1 && target is IParsable balanceBase)
			{
				balanceBase.OnParsed();
			}

			return sheetCount;
		}

		private static bool CheckIfCanParseWholeSheet(object target, Workbook book)
		{
			//if (targetType.Name != book.Name) return false; //не помню зачем это нужно было
			//if (targetType.IsGenericType) return false; //и это

			var sheetName = target.GetType().GetCustomAttributes<TableSheetNameAttribute>().FirstOrDefault();
			var sheet = book.GetTable(sheetName?.TableName);

			if (sheet == null) return false;

			var entities = GetEntityListFromSheet(sheet, typeof(ConstantsPair));

			if (entities is List<ConstantsPair> constants)
			{
				constants.ApplyValuesToObject(target);
				return true;
			}

			return false;
		}

		private static object TryGetFloatValuesFromSheet(DataTable table, bool toArray)
		{
			if (GetEntityListFromSheet(table, typeof(ByteFloatPair)) is List<ByteFloatPair> entities)
			{
				if (entities.Count > 0 && entities[0].key != 0) entities.Insert(0, new ByteFloatPair());
				var values = entities.Select(pair => pair.value);
				return toArray ? (object)values.ToArray() : values.ToList();
			}

			return toArray ? (object)new float[0] : new List<float>();
		}

		private static object TryGetIntValuesFromSheet(DataTable table, bool toArray)
		{
			var floatValues = TryGetFloatValuesFromSheet(table, toArray);
			if (floatValues is IEnumerable<float> floatEnumerable)
			{
				var intEnumerable = floatEnumerable.Select(value => (int) value);
				return toArray ? (object) intEnumerable.ToArray() : intEnumerable.ToList();
			}

			return toArray ? (object) new int[0] : new List<int>();
		}

		static DataTable GetTable(FieldInfo assetField, Workbook book)
		{
			var sheet = book.GetTable(assetField.Name);
			if (sheet != null) return sheet;

			var attribute = assetField.GetCustomAttributes<TableSheetNameAttribute>().FirstOrDefault();
			return attribute == null ? null : book.GetTable(attribute.TableName);
		}
	}
}