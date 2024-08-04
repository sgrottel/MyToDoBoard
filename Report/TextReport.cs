using System.Text;

namespace MyToDo.Report
{

	internal class TextReport : IReport
	{
		public string InputPath { get; set; } = string.Empty;
		public string OutputPath { get; set; } = string.Empty;

		public void Report(StaticDataModel.ToDoDocument todoDoc)
		{
			StringBuilder sb = new();
			sb.AppendLine("MyToBoard™ Report");
			sb.AppendLine($"From: {InputPath}");
			sb.AppendLine($"Created on: {DateTime.Now}");

			try
			{
				buildReport(sb, todoDoc);

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

		private void buildReport(StringBuilder sb, StaticDataModel.ToDoDocument todoDoc)
		{
			if (todoDoc.Columns == null) return;
			foreach (var column in todoDoc.Columns)
			{
				sb.AppendLine();
				sb.AppendLine($"Column: {column.Title ?? "Unnamed"}");

				if (column.Cards == null)
				{
					sb.AppendLine(" No cards");
					continue;
				}

				sb.AppendLine($"Number of cards: {column.Cards.Count}");
			}

		}

	}
}
