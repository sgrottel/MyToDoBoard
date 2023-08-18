using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public void Report(YamlObject myToDoYaml)
		{
			HtmlDocument doc = new();

			doc.DocumentNode.AppendChild(HtmlNode.CreateNode("<!DOCTYPE html>"));
			var html = doc.DocumentNode.AppendChild(HtmlNode.CreateNode("<html></html>"));
			head = html.AppendChild(HtmlNode.CreateNode("<head></head>"));
			head.AppendChild(HtmlNode.CreateNode("<meta charset=\"utf-8\">"));
			head.AppendChild(HtmlNode.CreateNode("""<meta name="viewport" content="width=device-width, initial-scale=1">"""));
			var body = html.AppendChild(HtmlNode.CreateNode("<body></body>"));

			var TimeStamp = DateTime.Now;

			head.AppendChild(HtmlNode.CreateNode($"<title>{Path.GetFileName(InputPath)} Report</title>"));
			head.AppendChild(HtmlNode.CreateNode("<meta name=\"generator\">")).Attributes.Add("content", System.Reflection.Assembly.GetExecutingAssembly().FullName);
			head.AppendChild(HtmlNode.CreateNode($"""<meta name="date" content="{TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}">"""));

			summary = body.AppendChild(HtmlNode.CreateNode("<div class=\"summary\">"));
			summary.AppendChild(HtmlNode.CreateNode($"""<div class="title">MyToDo Report of <span class="filename">{Path.GetFileName(InputPath)}</span></div>"""));
			summary.AppendChild(HtmlNode.CreateNode($"""<div class="generated">Generated <span class="date">{TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}</span></div>"""));

			content = body.AppendChild(HtmlNode.CreateNode("<div class=\"content\">"));

			prettyPrint(doc.DocumentNode);
			doc.Save(OutputPath, new UTF8Encoding(false));
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
