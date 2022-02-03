using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Debug = UnityEngine.Debug;

namespace GoogleParse
{
	public static class GoogleCSVDownloader
	{
		private const char NEW_LINE = '\n',
		                   SEPARATOR = '\t';

		static readonly string[] Scopes = {SheetsService.Scope.SpreadsheetsReadonly};
		private static string _applicationName, _credentialsPath;

		public static void Init(string applicationName, string credentialsPath)
		{
			_applicationName = applicationName;
			_credentialsPath = credentialsPath;
		}

		public static void DownloadSheetsAsCSV(string spreadsheetId, string savePath)
		{
			UserCredential credential;

			if (!File.Exists(_credentialsPath))
			{
				Debug.LogError($"No credentials file at path {_credentialsPath}");
				return;
			}

			using (var stream =
				new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
			{
				var credPath = _credentialsPath.Replace("credentials.json", "token");
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
				                                                         Scopes,
				                                                         "user",
				                                                         CancellationToken.None,
				                                                         new FileDataStore(credPath, true)).Result;
				//Debug.Log("Credential file saved to: " + credPath);
			}

			var service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = _applicationName,
			});

			var metaData = service.Spreadsheets.Get(spreadsheetId).Execute();
			var sheetNames = metaData.Sheets.Select(sheet => sheet.Properties.Title);

			if (Directory.Exists(savePath)) Directory.Delete(savePath, true);

			Directory.CreateDirectory(savePath);

			foreach (var sheetName in sheetNames)
			{
				if (sheetName.ToLower().Contains("ignore")) continue;

				var request = service.Spreadsheets.Values.Get(spreadsheetId, sheetName);

				var response = request.Execute();
				var values = response.Values;

				if (values == null)
				{
					//Debug.Log($"{sheetName} table is empty");
					continue;
				}

				var csv = ValuesToCSV(values);
				SaveCSVToFile(csv, savePath, sheetName);
			}
		}

		private static string ValuesToCSV(IList<IList<object>> sheetValues)
		{
			var strBuilder = new StringBuilder();
			var rows = sheetValues.ToList();

			for (int i = 0; i < rows.Count; i++)
			{
				var values = rows[i].ToList();

				for (var j = 0; j < values.Count; j++)
				{
					var strValue = values[j].ToString();

					if (strValue.Contains(SEPARATOR))
					{
						strValue = $"\"{strValue}\"";
					}

					strBuilder.Append(strValue);
					if (j != values.Count - 1) strBuilder.Append(SEPARATOR);
				}

				if (i != rows.Count - 1) strBuilder.Append(NEW_LINE);
			}

			return strBuilder.ToString();
		}

		private static void SaveCSVToFile(string csv, string savePath, string sheetName)
		{
			var filePath = $"{savePath}/{sheetName}.csv";

			if (File.Exists(filePath)) File.Delete(filePath);

			if (string.IsNullOrEmpty(csv))
			{
				Debug.Log($"{sheetName} is empty");
				return;
			}

			File.WriteAllText(filePath, csv);
		}
	}
}