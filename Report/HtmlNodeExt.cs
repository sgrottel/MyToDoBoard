using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Report
{
	internal static class HtmlNodeExt
	{
		
		internal static HtmlNode AppendHtml(this HtmlNode node, string html)
		{
			return node.AppendChild(HtmlNode.CreateNode(html));
		}

	}
}
