using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Report
{
	using YamlObject = Dictionary<object, object>;
	using YamlList = List<object>;

	internal class HtmlReport : IReport
	{
		public string InputPath { get; set; } = string.Empty;
		public string OutputPath { get; set; } = string.Empty;

		private HtmlNode? head;

		private HtmlNode? summary;

		private HtmlNode? content;

		private const bool embedStyle
#if DEBUG
			= false;
#else
			= true;
#endif

		public void Report(YamlObject myToDoYaml)
		{
			HtmlDocument doc = new();

			doc.DocumentNode.AppendChild(HtmlNode.CreateNode("<!DOCTYPE html>"));
			var html = doc.DocumentNode.AppendChild(HtmlNode.CreateNode("<html lang=\"en\"/>"));
			head = html.AppendChild(HtmlNode.CreateNode("<head/>"));
			head.AppendChild(HtmlNode.CreateNode("<meta charset=\"utf-8\">"));
			head.AppendChild(HtmlNode.CreateNode("""<meta name="viewport" content="width=device-width, initial-scale=1">"""));
			AddStyleTag();
			var body = html.AppendChild(HtmlNode.CreateNode("<body/>"));

			var TimeStamp = DateTime.Now;

			head.AppendChild(HtmlNode.CreateNode($"<title>{Path.GetFileName(InputPath)} Report</title>"));
			head.AppendChild(HtmlNode.CreateNode("<meta name=\"generator\">")).Attributes.Add("content", Assembly.GetExecutingAssembly().FullName);
			head.AppendChild(HtmlNode.CreateNode($"""<meta name="date" content="{TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}">"""));

			summary = body.AppendChild(HtmlNode.CreateNode("<div class=\"summary\">"));
			summary.AppendChild(HtmlNode.CreateNode($"""<div class="title">MyToDo Report of <span class="filename">{Path.GetFileName(InputPath)}</span></div>"""));
			summary.AppendChild(HtmlNode.CreateNode($"""<div class="generated">Generated <span class="date">{TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}</span></div>"""));

			content = body.AppendChild(HtmlNode.CreateNode("<div class=\"content\">"));

			try
			{
				BuildContent(myToDoYaml);
			}
			catch (Exception ex)
			{
				content.ChildNodes.Clear();
				content.AppendChild(HtmlNode.CreateNode($"""<div class="exception">Failed: {ex}</div>"""));
			}

			prettyPrint(doc.DocumentNode);
			doc.Save(OutputPath, new UTF8Encoding(false));
		}

		private void BuildContent(YamlObject myToDoYaml)
		{
			if (content == null) throw new InvalidOperationException();
			YamlList columns = myToDoYaml.TryGetYamlProperty("columns")
				.NotNull("Columns value is unexpectitly null")
				.AsYamlList("Columns property of unexpeced type");
			foreach (object columnObj in columns)
			{
				YamlObject column = columnObj.AsYamlObject("Column of unexpected type");
				var columnNode = content.AppendChild(HtmlNode.CreateNode("<div class=\"column\">"));
				string title = (column.GetYamlProperty("title") as string) ?? "Noname";

				var cards = column.TryGetYamlProperty("cards").TryAsYamlList();
				int cardsCount = cards?.Count ?? 0;

				var columnHeader = columnNode.AppendChild(HtmlNode.CreateNode("<div class=\"header\">"));

				columnHeader.AppendChild(HtmlNode.CreateNode($"<div class=\"title\">{title}</div>"));
				columnHeader.AppendChild(HtmlNode.CreateNode($"<div class=\"info\">{cardsCount} Card{((cardsCount == 1) ? "" : "s")}</div>"));

				if (cards != null)
				{
					foreach (object cardObj in cards)
					{
						YamlObject card = cardObj.AsYamlObject("Card of unexpected type");
						title = (card.GetYamlProperty("title") as string) ?? "Noname";

						var cardNode = columnNode.AppendChild(HtmlNode.CreateNode("<div class=\"card\">"));
						var cardHeader = cardNode.AppendChild(HtmlNode.CreateNode("<div class=\"header\">"));
						cardHeader.AppendChild(HtmlNode.CreateNode($"<div class=\"title\">{title}</div>"));

						var cardDate = card.TryGetYamlProperty("date");
						HtmlNode? cardDateNode = null;
						if (cardDate != null)
						{
							cardDateNode = cardHeader.AppendChild(HtmlNode.CreateNode($"<div class=\"info\">📅 {cardDate}</div>"));
						}

						cardDate = card.TryGetYamlProperty("modDate");
						if (cardDate != null)
						{
							if (cardDateNode != null)
							{
								cardDateNode.AppendChild(HtmlNode.CreateNode("<br>"));
								cardDateNode.AppendChild(HtmlNode.CreateNode($"✏️ {cardDate}"));
							}
							else
							{
								cardHeader.AppendChild(HtmlNode.CreateNode($"<div class=\"info\">✏️ {cardDate}</div>"));
							}
						}

						var cardDesc = card.TryGetYamlProperty("desc");
						if (cardDesc != null)
						{
							cardNode.AppendChild(HtmlNode.CreateNode($"<div class=\"text\">{cardDesc}</div>"));
						}

						var cardLinks = card.TryGetYamlProperty("links").TryAsYamlList();
						if (cardLinks != null && cardLinks.Count > 0)
						{
							foreach (object linkObj in cardLinks)
							{
								if (linkObj == null) continue;
								string link = linkObj as string;
								cardNode.AppendChild(HtmlNode.CreateNode($"<div class=\"link\">{link}</div>"));
							}
						}
					}
				}
			}
		}

		private void AddStyleTag()
		{
			if (head == null) throw new InvalidOperationException();
			if (!embedStyle)
			{
#pragma warning disable CS0162 // Unreachable code detected
				string? p = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				while (p != null)
				{
					string sf = Path.Combine(p, "HtmlReportStyle.css");
					if (File.Exists(sf))
					{
						Uri sfu = new Uri(sf);
						head.AppendChild(HtmlNode.CreateNode("<link rel=\"stylesheet\">")).Attributes.Add("href", sfu.AbsoluteUri);
						return;
					}
					p = Path.GetDirectoryName(p);
				}
#pragma warning restore CS0162 // Unreachable code detected
			}

			var assembly = Assembly.GetExecutingAssembly();
			string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("HtmlReportStyle.css"));
			using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null) throw new Exception($"Embedded resource stream \"{resourceName}\" missing");
				using (StreamReader reader = new StreamReader(stream))
				{
					string css = reader.ReadToEnd();
					head.AppendChild(HtmlNode.CreateNode($"<style>\n{css}  </style>"));
				}
			}
		}

		private void prettyPrint(HtmlNode node, int indent = -1)
		{
			var cns = node.ChildNodes.ToArray();

			if (cns.Length == 1)
			{
				if (cns[0].NodeType == HtmlNodeType.Text)
				{
					cns = Array.Empty<HtmlNode>();
				}
			}

			foreach (HtmlNode n in cns)
			{
				if (n.NodeType == HtmlNodeType.Text)
				{
					n.InnerHtml = n.InnerHtml.Trim();
				}

				if (indent > 0)
				{
					node.InsertBefore(HtmlNode.CreateNode(new string(' ', indent * 2)), n);
				}
				node.InsertAfter(HtmlNode.CreateNode("\n"), n);

				if (n.NodeType == HtmlNodeType.Element)
				{
					prettyPrint(n, indent + 1);
				}
			}

			if (indent >= 0 && cns.Length > 0)
			{
				node.PrependChild(HtmlNode.CreateNode("\n"));
				if (indent > 1)
				{
					node.AppendChild(HtmlNode.CreateNode(new string(' ', (indent - 1) * 2)));
				}
			}
		}
	}
}
