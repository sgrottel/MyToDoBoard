using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace MyToDo.Report
{
	using YamlObject = Dictionary<object, object>;
	using YamlList = List<object>;

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

		public void Report(YamlObject myToDoYaml)
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
				AddInfoToHead(doc, myToDoYaml);
				AddInfoToSummary(doc, myToDoYaml);
				BuildColumns(doc, myToDoYaml);
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

		private void AddInfoToHead(HtmlDocument doc, YamlObject myToDoYaml)
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

		private void AddInfoToSummary(HtmlDocument doc, YamlObject _)
		{
			var summaryNode = doc.DocumentNode.SelectSingleNode("/html/body//div[@id=\"summary\"]");
			if (summaryNode == null) throw new Exception("summary node not found");

			var headerNode = summaryNode.AppendHtml("<div class=\"header\">");

			headerNode.AppendHtml($"""<div class="title">MyToDo Report of <span class="filename">{HtmlEncode(Path.GetFileName(InputPath))}</span></div>""");
			headerNode.AppendHtml($"""<div class="generated">Generated <span class="date">{HtmlEncode(Timestamp.ToString("yyyy-MM-dd HH:mm:ss"))}</span></div>""");
		}

		private void BuildColumns(HtmlDocument doc, YamlObject myToDoYaml)
		{
			var columnsNode = doc.DocumentNode.SelectSingleNode("/html/body//div[@id=\"columns\"]");
			if (columnsNode == null) throw new Exception("columns node not found");
			columnsNode.RemoveAllChildren();

			YamlList columns = myToDoYaml.TryGetYamlProperty("columns")
				.NotNull("Columns value is unexpectitly null")
				.AsYamlList("Columns property of unexpeced type");

			foreach (object columnObj in columns)
			{
				YamlObject column = columnObj.AsYamlObject("Column of unexpected type");
				string? viewType = column.TryGetYamlProperty("view")?.ToString();
				if (viewType != null)
				{
					viewType = "view-" + viewType.ToLower();
				}

				var columnNode = columnsNode.AppendHtml("<div class=\"column\">");
				if (viewType != null)
				{
					columnNode.Attributes["class"].Value += " " + viewType;
				}

				string title = (column.GetYamlProperty("title") as string) ?? "Noname";
				var cards = column.TryGetYamlProperty("cards").TryAsYamlList();
				int cardsCount = cards?.Count ?? 0;

				var columnHeader = columnNode.AppendHtml("<div class=\"header\">");

				columnHeader.AppendHtml($"<div class=\"info\">{cardsCount} Card{((cardsCount == 1) ? "" : "s")}</div>");
				columnHeader.AppendHtml($"<div class=\"title\">{HtmlEncode(title)}</div>");

				if (cards != null)
				{
					foreach (object cardObj in cards)
					{
						YamlObject card = cardObj.AsYamlObject("Card of unexpected type");
						title = (card.GetYamlProperty("title") as string) ?? "Noname";

						var cardNode = columnNode.AppendHtml("<div class=\"card\">");
						if (viewType != null)
						{
							cardNode.Attributes["class"].Value += " " + viewType;
						}

						var cardHeader = cardNode.AppendHtml("<div class=\"header\">");

						var cardDate = card.TryGetYamlProperty("date");
						HtmlNode? cardDateNode = null;
						if (cardDate != null)
						{
							cardDateNode = cardHeader.AppendHtml($"<div class=\"info\">📅 <span class=\"date\">{cardDate}</span></div>");
						}

						cardDate = card.TryGetYamlProperty("modDate");
						if (cardDate != null)
						{
							if (cardDateNode != null)
							{
								cardDateNode.AppendHtml("<br>");
								cardDateNode.AppendHtml("✏️ ");
								cardDateNode.AppendHtml($"<span class=\"date moddate\">{cardDate}</span>");
							}
							else
							{
								cardHeader.AppendHtml($"<div class=\"info\">✏️ <span class=\"date moddate\">{cardDate}</span></div>");
							}
						}

						cardDate = card.TryGetYamlProperty("dueDate");
						if (cardDate != null)
						{
							if (cardDateNode != null)
							{
								cardDateNode.AppendHtml("<br>");
								cardDateNode.AppendHtml("⏰ ");
								cardDateNode.AppendHtml($"<span class=\"date duedate\">{cardDate}</span>");
							}
							else
							{
								cardHeader.AppendHtml($"<div class=\"info\">⏰ <span class=\"date duedate\">{cardDate}</span></div>");
							}
						}

						cardHeader.AppendHtml($"<div class=\"title\">{HtmlEncode(title)}</div>");

						var cardDesc = card.TryGetYamlProperty("desc");
						if (cardDesc != null)
						{
							cardNode.AppendHtml($"<div class=\"text\">{HtmlEncode(cardDesc)}</div>");
						}

						var cardLinks = card.TryGetYamlProperty("links").TryAsYamlList();
						if (cardLinks != null && cardLinks.Count > 0)
						{
							foreach (object linkObj in cardLinks)
							{
								string? link = linkObj?.ToString();
								if (string.IsNullOrWhiteSpace(link)) continue;
								cardNode.AppendHtml($"<div class=\"link\">{HtmlEncode(link)}</div>");
							}
						}
					}
				}
			}

		}

	}
}
