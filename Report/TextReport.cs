using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Report
{
	using YamlObject = Dictionary<object, object>;
	using YamlList = List<object>;

	internal class TextReport : IReport
	{
		public string InputPath { get; set; } = string.Empty;
		public string OutputPath { get; set; } = string.Empty;

		public void Report(YamlObject myToDoYaml)
		{
			StringBuilder sb = new();
			sb.AppendLine("MyToBoard™ Report");
			sb.AppendLine($"From: {InputPath}");
			sb.AppendLine($"Created on: {DateTime.Now}");

			try
			{
				buildReport(sb, myToDoYaml);

				sb.AppendLine();
				sb.AppendLine("End.");
			}
			catch (Exception ex)
			{
				sb.AppendLine();
				sb.AppendLine("Aborted!");
				sb.AppendLine($"Exception: {ex}");
				sb.AppendLine();
			}

			File.WriteAllText(OutputPath, sb.ToString());
		}

		private void buildReport(StringBuilder sb, YamlObject myToDoYaml)
		{
			YamlList columns = myToDoYaml.GetYamlProperty("columns")
				.NotNull("Columns value is unexpectitly null")
				.AsYamlList("Columns property of unexpeced type");
			foreach (object columnObj in columns)
			{
				YamlObject column = columnObj.AsYamlObject("Column of unexpected type");
				object? title = column.GetYamlProperty("title");

				sb.AppendLine();
				sb.AppendLine($"Column: {title ?? "Unnamed"}");

				YamlList? cards = column.GetYamlProperty("cards").TryAsYamlList();
				if (cards == null)
				{
					sb.AppendLine(" No cards");
					continue;
				}

				sb.AppendLine($"Number of cards: {cards.Count}");
			}

		}

	}
}
