using HtmlAgilityPack;
using MyToDo.StaticDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace MyToDo.Report
{

	internal class HtmlReport : IReport
	{
		public string InputPath { get; set; } = string.Empty;
		public string OutputPath { get; set; } = string.Empty;

		private DateTime Timestamp = DateTime.MinValue;

		private string HtmlEncode(object? o)
		{
			if (o == null) return "NULL";
			return HtmlDocument.HtmlEncode(o.ToString());
		}

		public void Report(ToDoDocument todoDoc)
		{
			HtmlDocument doc = new();
			Timestamp = DateTime.Now;

			var assembly = Assembly.GetExecutingAssembly();
			string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("HtmlReportTemplate.html"));
			using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null) throw new Exception("Failed to find HtmlReportTemplate.html resource");
				using (StreamReader reader = new(stream))
					doc.LoadHtml(reader.ReadToEnd());
			}

			try
			{
				bool embedStyle = true;
#if DEBUG
				embedStyle = false;
#endif
				UpdateStyleTag(doc, embedStyle);
				AddInfoToHead(doc, todoDoc);
				AddInfoToSummary(doc, todoDoc);
				BuildColumns(doc, todoDoc);

				// TODO: Labels

			}
			catch (Exception ex)
			{
				var sumNode = doc.DocumentNode.SelectSingleNode("/html/body//div[@id=\"summary\"]");
				if (sumNode != null)
				{
					sumNode.AppendHtml($"""<div class="exception">Failed: {HtmlEncode(ex)}</div>""");
				}
				else
				{
					throw;
				}
			}

			doc.RemoveAllWhitespace();
			doc.MakePrettyPrinted();
			doc.Save(OutputPath, new UTF8Encoding(false));
		}

		private void UpdateStyleTag(HtmlDocument doc, bool embedStyle)
		{
			var headNode = doc.DocumentNode.SelectSingleNode("/html/head");
			if (headNode == null) throw new Exception("head tag not found");

			var styleNode = headNode.SelectSingleNode("link[@rel=\"stylesheet\"]");
			if (styleNode == null)
			{
				styleNode = headNode.AppendHtml("<link rel=\"stylesheet\" href=\"./HtmlReportStyle.css\">");
			}

			if (!embedStyle)
			{
				string? p = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				while (p != null)
				{
					string sf = Path.Combine(p, "HtmlReportStyle.css");
					if (File.Exists(sf))
					{
						Uri sfu = new Uri(sf);
						styleNode.Attributes["href"].Value = sfu.AbsoluteUri;
						return;
					}
					p = Path.GetDirectoryName(p);
				}
			}

			var assembly = Assembly.GetExecutingAssembly();
			string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("HtmlReportStyle.css"));
			using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null) throw new Exception($"Embedded resource stream \"{resourceName}\" missing");
				using (StreamReader reader = new StreamReader(stream))
				{
					string css = reader.ReadToEnd();
					styleNode = headNode.ReplaceChild(HtmlNode.CreateNode("<style>"), styleNode);
					styleNode.AppendHtml($"\n{css}\t");
				}
			}

		}

		private void AddInfoToHead(HtmlDocument doc, ToDoDocument todoDoc)
		{
			var headNode = doc.DocumentNode.SelectSingleNode("/html/head");
			if (headNode == null) throw new Exception("head tag not found");

			var titleNode = headNode.SelectSingleNode("title");
			if (titleNode == null)
			{
				titleNode = headNode.AppendChild(HtmlNode.CreateNode("<title>"));
			}
			titleNode.RemoveAllChildren();
			titleNode.AppendHtml(HtmlEncode(Path.GetFileName(InputPath) + " Report"));

			headNode.AppendHtml("<meta name=\"sourcefile\">").Attributes.Add("content", InputPath);
			headNode.AppendHtml("<meta name=\"generator\">").Attributes.Add("content", Assembly.GetExecutingAssembly().FullName);
			headNode.AppendHtml($"""<meta name="date" content="{HtmlEncode(Timestamp.ToString("yyyy-MM-dd HH:mm:ss"))}">""");
		}

		private void AddInfoToSummary(HtmlDocument doc, ToDoDocument _)
		{
			var summaryNode = doc.DocumentNode.SelectSingleNode("/html/body//div[@id=\"summary\"]");
			if (summaryNode == null) throw new Exception("summary node not found");

			var headerNode = summaryNode.AppendHtml("<div class=\"header\">");

			headerNode.AppendHtml($"""<div class="title">MyToDo Report of <span class="filename">{HtmlEncode(Path.GetFileName(InputPath))}</span></div>""");
			headerNode.AppendHtml($"""<div class="generated">Generated <span class="date">{HtmlEncode(Timestamp.ToString("yyyy-MM-dd HH:mm:ss"))}</span></div>""");
		}

		private class ColumnInfo {
			public int Count { get; set; } = 0;
			public int CountHidden { get; set; } = 0;
			public HtmlNode? ColumnNode { get; set; } = null;
			public HtmlNode? InfoNode { get; set; } = null;
		};

		private void BuildColumns(HtmlDocument doc, ToDoDocument todoDoc)
		{
			var columnsNode = doc.DocumentNode.SelectSingleNode("/html/body//div[@id=\"columns\"]");
			if (columnsNode == null) throw new Exception("columns node not found");
			columnsNode.RemoveAllChildren();

			Dictionary<string, ColumnInfo> columnsInfo = new();

			foreach (Column column in todoDoc.Columns ?? new())
			{
				string viewType = $"view-{column.View}";
				string columnTitle = (column.Title) ?? "Noname";

				if (!columnsInfo.ContainsKey(columnTitle))
				{
					columnsInfo.Add(columnTitle, new() { ColumnNode = columnsNode.AppendHtml("<div class=\"column\">") });
					var columnHeader = (columnsInfo[columnTitle].ColumnNode ?? throw new Exception()).AppendHtml("<div class=\"header\">");
					columnsInfo[columnTitle].InfoNode = columnHeader.AppendHtml("<div class=\"info\" />");
					columnHeader.AppendHtml($"<div class=\"title\">{HtmlEncode(columnTitle)}</div>");
				}

				if (column.Cards == null) continue;

				foreach (Card card in column.Cards)
				{
					ColumnInfo info = columnsInfo[columnTitle] ?? throw new Exception();
					if (viewType == "view-hidden")
					{
						info.CountHidden++;
					}
					else
					{
						info.Count++;
					}

					var cardNode = (info.ColumnNode ?? throw new Exception()).AppendHtml("<div class=\"card\">");
					if (viewType != null)
					{
						cardNode.Attributes["class"].Value += " " + viewType;
					}

					var cardHeader = cardNode.AppendHtml("<div class=\"header\">");

					HtmlNode? cardDateNode = null;
					if (card.Date != null)
					{
						cardDateNode = cardHeader.AppendHtml($"<div class=\"info\">📅 <span class=\"date\">{card.Date:dd.MM.yyyy}</span></div>");
					}

					if (card.ModifiedDate != null)
					{
						if (cardDateNode != null)
						{
							cardDateNode.AppendHtml("<br>");
							cardDateNode.AppendHtml("✏️ ");
							cardDateNode.AppendHtml($"<span class=\"date moddate\">{card.ModifiedDate:dd.MM.yyyy}</span>");
						}
						else
						{
							cardDateNode = cardHeader.AppendHtml($"<div class=\"info\">✏️ <span class=\"date moddate\">{card.ModifiedDate:dd.MM.yyyy}</span></div>");
						}
					}

					if (card.DueDate != null)
					{
						if (cardDateNode != null)
						{
							cardDateNode.AppendHtml("<br>");
							cardDateNode.AppendHtml("⏰ ");
							cardDateNode.AppendHtml($"<span class=\"date duedate\">{card.DueDate:dd.MM.yyyy}</span>");
						}
						else
						{
							cardHeader.AppendHtml($"<div class=\"info\">⏰ <span class=\"date duedate\">{card.DueDate:dd.MM.yyyy}</span></div>");
						}
					}

					cardHeader.AppendHtml($"<div class=\"title\">{HtmlEncode(card.Title)}</div>");

					HtmlNode cardContentNode = cardNode.AppendHtml("<div class=\"text\">"); 
					if (card.Description != null)
					{
						cardContentNode.AppendHtml($"<div>{HtmlEncode(card.Description)}</div>");
					}

					if (card.Checklist != null)
					{
						cardContentNode.AppendHtml("<h3>Checklist</h3>");
						HtmlNode checklistNode = cardContentNode.AppendHtml("<ul>");

						foreach (CheckListItem cli in card.Checklist)
						{
							HtmlNode cliNode = checklistNode.AppendHtml("<li>");

							if (cli.Checked ?? false && cli.Date != null)
							{
								cliNode.AppendHtml($"<div class=\"info date\">{cli.Date:dd.MM.yyyy}</div>");
							}
							cliNode.AppendHtml($"<div>{((cli.Checked ?? false) ? '☑' : '☐')}&nbsp;&nbsp;{HtmlEncode(cli.Text)}</div>");
						}
					}

					if (card.Comments != null)
					{
						cardContentNode.AppendHtml("<h3>Comments</h3>");
						foreach (Comment c in card.Comments)
						{
							if (c.Date != null)
							{
								cardContentNode.AppendHtml($"<div class=\"info date\">{c.Date:dd.MM.yyyy}</div>");
							}
							cardContentNode.AppendHtml($"<div>{HtmlEncode(c.Text)}</div>");
						}
					}

					if (card.Links != null && card.Links.Count > 0)
					{
						foreach (string link in card.Links)
						{
							if (string.IsNullOrWhiteSpace(link)) continue;
							cardNode.AppendHtml($"<div class=\"link\">{HtmlEncode(link)}</div>");
						}
					}

					// TODO: Labels

				}
			}

			foreach (KeyValuePair<string, ColumnInfo> column in columnsInfo)
			{
				if (column.Value.Count == 0 && column.Value.CountHidden > 0)
				{
					HtmlAttribute attr = column.Value.ColumnNode?.Attributes["class"] ?? throw new Exception();
					attr.Value += " view-hidden";
				}
				int cardsCount = column.Value.Count;
				column.Value.InfoNode?.AppendHtml(HtmlEncode($"{cardsCount} Card{((cardsCount == 1) ? "" : "s")}"));
			}
		}

	}
}
