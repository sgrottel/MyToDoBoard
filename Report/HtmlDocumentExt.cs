using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyToDo.Report
{
	internal static class HtmlDocumentExt
	{

		internal static void RemoveAllWhitespace(this HtmlDocument doc)
		{
			ClearWhitespace(doc.DocumentNode);
		}

		internal static void MakePrettyPrinted(this HtmlDocument doc)
		{
			PrettyPrint(doc.DocumentNode);
		}

		private static void ClearWhitespace(HtmlNode node)
		{
			List<HtmlNode> toDelete = new();
			foreach (HtmlNode n in node.ChildNodes)
			{
				if (n.NodeType == HtmlNodeType.Element)
				{
					ClearWhitespace(n);
				}
				else if (n.NodeType == HtmlNodeType.Text)
				{
					if (string.IsNullOrWhiteSpace(n.InnerHtml))
					{
						toDelete.Add(n);
					}
				}
			}
			foreach (HtmlNode n in toDelete)
			{
				node.RemoveChild(n);
			}
		}

		private static void PrettyPrint(HtmlNode node, int indent = -1)
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
					node.InsertBefore(HtmlNode.CreateNode(new string('\t', indent)), n);
				}
				node.InsertAfter(HtmlNode.CreateNode("\n"), n);

				if (n.NodeType == HtmlNodeType.Element)
				{
					PrettyPrint(n, indent + 1);
				}
			}

			if (indent >= 0 && cns.Length > 0)
			{
				node.PrependChild(HtmlNode.CreateNode("\n"));
				if (indent > 1)
				{
					node.AppendChild(HtmlNode.CreateNode(new string('\t', (indent - 1))));
				}
			}
		}
	}
}
