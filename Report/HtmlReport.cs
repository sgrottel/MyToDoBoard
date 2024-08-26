using HtmlAgilityPack;
using MyToDo.StaticDataModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyToDo.Report
{

	internal class HtmlReport : IReport
	{
		public string InputPath { get; set; } = string.Empty;
		public string OutputPath { get; set; } = string.Empty;
		public bool? DarkMode { get; set; }
		public bool? NoColumnWrapMode { get; set; }

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
				InsertModeClasses(doc);
				InsertInputFileHash(doc);
				UpdateStyleTag(doc, todoDoc, embedStyle);
				AddInfoToHead(doc, todoDoc);
				AddInfoToSummary(doc, todoDoc);
				AddLocalHtmlInterop(doc, InputPath, OutputPath, embedStyle);
				BuildColumns(doc, todoDoc);
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

		private void InsertModeClasses(HtmlDocument doc)
		{
			bool darkMode = DarkMode.GetValueOrDefault(false);
			bool noColumnWrapMode = NoColumnWrapMode.GetValueOrDefault(false);
			if (!DarkMode.HasValue || !NoColumnWrapMode.HasValue)
			{
				if (File.Exists(OutputPath))
				{
					HtmlDocument prevDoc = new();
					prevDoc.Load(OutputPath);
					var htmlNode = prevDoc.DocumentNode.SelectSingleNode("/html");
					if (htmlNode != null)
					{
						if (!DarkMode.HasValue)
						{
							darkMode = htmlNode.HasClass("dark");
						}
						if (!NoColumnWrapMode.HasValue)
						{
							noColumnWrapMode = htmlNode.HasClass("noColumnWrap");
						}
					}
				}
			}

			if (darkMode || noColumnWrapMode)
			{
				var htmlNode = doc.DocumentNode.SelectSingleNode("/html") ?? throw new Exception("html root node not found");
				if (darkMode)
				{
					htmlNode.AddClass("dark");
				}
				if (noColumnWrapMode)
				{
					htmlNode.AddClass("noColumnWrap");
				}
			}
		}

		private void InsertInputFileHash(HtmlDocument doc)
		{
			var inputFileHash = SHA256.Create();
			inputFileHash.ComputeHash(File.Open(InputPath, FileMode.Open, FileAccess.Read, FileShare.Read));
			doc.DocumentNode
				.SelectSingleNode("/html/head")
				.PrependChild(doc.CreateComment(
					"INPUTFILEHASH: " + Convert.ToBase64String(inputFileHash.Hash ?? Array.Empty<byte>()
					)));
		}

		private bool AddLocalHtmlInterop(HtmlDocument doc, string mytodoFile, string reportFile, bool embedScript)
		{
			string? lHtmlIopExe = FindExecutable.FindExecutable.FullPath("LocalHtmlInterop.exe");
			if (string.IsNullOrWhiteSpace(lHtmlIopExe)
				|| !File.Exists(lHtmlIopExe)
				|| !FindExecutable.FindExecutable.IsExecutable(lHtmlIopExe))
			{
				return false;
			}

			int lHtmlIopPort;
			string lHtmlIopJS;
			{
				Process lHtmlIop = new();
				lHtmlIop.StartInfo = new()
				{
					FileName = lHtmlIopExe,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
				};
				lHtmlIop.StartInfo.ArgumentList.Add("getport");
				lHtmlIop.StartInfo.ArgumentList.Add("--nologo");
				lHtmlIop.Start();
				lHtmlIop.WaitForExit();
				string stdout = lHtmlIop.StandardOutput.ReadToEnd();
				if (!int.TryParse(stdout.Trim([' ', '\t', '\n', '\r']), out lHtmlIopPort))
				{
					throw new InvalidOperationException("Failed to read port of LocalHtmlInterop");
				}

				lHtmlIop = new();
				lHtmlIop.StartInfo = new()
				{
					FileName = lHtmlIopExe,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
				};
				lHtmlIop.StartInfo.ArgumentList.Add("getjscode");
				lHtmlIop.StartInfo.ArgumentList.Add("--nologo");
				// lHtmlIop.StartInfo.ArgumentList.Add("--mini");
				lHtmlIop.Start();
				lHtmlIop.WaitForExit();
				stdout = lHtmlIop.StandardOutput.ReadToEnd();
				lHtmlIopJS = stdout.Trim([' ', '\t', '\n', '\r']);
				if (string.IsNullOrWhiteSpace(lHtmlIopJS))
				{
					throw new InvalidOperationException("Failed to read client js code of LocalHtmlInterop");
				}
			}

			var headNode = doc.DocumentNode.SelectSingleNode("/html/head");
			if (headNode == null) throw new Exception("head tag not found");

			headNode.AppendChild(doc.CreateElement("script")).AppendChild(doc.CreateTextNode(@$"
const localHtmlInteropPort = {lHtmlIopPort};
const localHtmlInteropF = '{mytodoFile.Replace("\\", "\\\\")}';
const localHtmlInteropT = '{reportFile.Replace("\\", "\\\\")}';
"));
			headNode.AppendChild(doc.CreateElement("script")).AppendChild(doc.CreateTextNode(lHtmlIopJS));

			var bodyNode = doc.DocumentNode.SelectSingleNode("/html/body");
			if (bodyNode == null) throw new Exception("body tag not found");
			var invokerNode = bodyNode.PrependChild(doc.CreateElement("iframe"));
			invokerNode.Id = "invoker";
			invokerNode.Attributes.Add("name", "invoker");
			invokerNode.Attributes.Add("title", "interop invoker");
			invokerNode.Attributes.Add("src", "about:blank");
			invokerNode.AddClass("hiddenInvoker");

			var sumTitleNode = doc.DocumentNode.SelectSingleNode("/html/body//div[@id=\"summary\"]//div[contains(@class,\"title\")]");

			var sym = sumTitleNode.AppendChild(doc.CreateElement("span")); // up-to-date indicator
			sym.Id = "lHtmlIopState";
			sym.AddClass("lHtmlIopSymbol");
			sym = sym.AppendChild(doc.CreateElement("a"));
			sym.InnerHtml = "❔";

			sym = sumTitleNode.AppendChild(doc.CreateElement("span")); // re-generate report
			sym.Id = "lHtmlIopUpdate";
			sym.AddClass("lHtmlIopSymbol");
			sym.InnerHtml = "🔁";

			sym = sumTitleNode.AppendChild(doc.CreateElement("span")); // edit mytodofile
			sym.Id = "lHtmlIopEdit";
			sym.AddClass("lHtmlIopSymbol");
			sym.Attributes.Add("title", $"Edit {InputPath}");
			sym.InnerHtml = "📝";
			sym.Attributes.Add("click", "startEdit");

			sym = sumTitleNode.AppendChild(doc.CreateElement("span")); // open file's folder
			sym.Id = "lHtmlIopExplore";
			sym.AddClass("lHtmlIopSymbol");
			sym.Attributes.Add("title", $"Show {InputPath} in File Explorer");
			sym.InnerHtml = "📂";

			if (embedScript)
			{
				var assembly = Assembly.GetExecutingAssembly();
				string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("lHtmlIopApp.js"));
				using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
				{
					if (stream == null) throw new Exception($"Embedded resource stream \"{resourceName}\" missing");
					using (StreamReader reader = new StreamReader(stream))
					{
						string js = reader.ReadToEnd();
						headNode.AppendChild(doc.CreateElement("script")).AppendChild(doc.CreateTextNode(js));
					}
				}
			}
			else
			{
				string? p = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				while (p != null)
				{
					string sf = Path.Combine(p, "lHtmlIopApp.js");
					if (File.Exists(sf))
					{
						headNode.AppendChild(doc.CreateElement("script")).Attributes.Add("src", sf);
						break;
					}
					p = Path.GetDirectoryName(p);
				}
			}

			bodyNode.AppendChild(doc.CreateElement("script")).AppendChild(doc.CreateTextNode("startCheck();"));

			return true;
		}

		private void UpdateStyleTag(HtmlDocument doc, ToDoDocument todoDoc, bool embedStyle)
		{
			var headNode = doc.DocumentNode.SelectSingleNode("/html/head");
			if (headNode == null) throw new Exception("head tag not found");

			var styleNode = headNode.SelectSingleNode("style[@id=\"labelColors\"]");
			if (styleNode != null)
			{
				if ((todoDoc.Labels?.Count ?? 0) > 0)
				{
					StringBuilder css = new();
					HashSet<string> labels = new();

					css.Append("\n");
					foreach (var l in todoDoc.Labels!)
					{
						string n = Regex.Replace(l.Id ?? string.Empty, "[^a-zA-Z0-9]", "");
						if (string.IsNullOrEmpty(n)) continue;
						if (labels.Contains(n)) continue;

						string textColor = "black";
						try
						{
							Color c = Color.FromName(l.Color ?? Color.Transparent.Name);
							if (c.A < 200) continue;

							// https://www.w3.org/WAI/GL/wiki/Relative_luminance#:~:text=in%20WCAG%202.-,x,%2B0.055)%2F1.055)%20%5E%202.4
							float RsRGB = c.R / 255.0f;
							float GsRGB = c.G / 255.0f;
							float BsRGB = c.B / 255.0f;

							float R = (RsRGB <= 0.03928) ? (RsRGB / 12.92f) : (float)Math.Pow((RsRGB + 0.055) / 1.055, 2.4);
							float G = (GsRGB <= 0.03928) ? (GsRGB / 12.92f) : (float)Math.Pow((GsRGB + 0.055) / 1.055, 2.4);
							float B = (BsRGB <= 0.03928) ? (BsRGB / 12.92f) : (float)Math.Pow((BsRGB + 0.055) / 1.055, 2.4);

							float lum = 0.2126f * R + 0.7152f * G + 0.0722f * B;

							double linLum = Math.Pow(lum, 1.0 / 2.4);

							if (linLum <= 0.61) textColor = "white";
						}
						catch
						{
							continue;
						}

						labels.Add(n);
						css.AppendLine($".label{n}Colors {{");
						css.AppendLine($"\tbackground-color: {l.Color};");
						css.AppendLine($"\tcolor: {textColor};");
						css.AppendLine("}");
					}
					css.Append("\t");

					styleNode.RemoveAllChildren();
					styleNode.AppendChild(doc.CreateTextNode(css.ToString()));
				}
				else
				{
					headNode.RemoveChild(styleNode);
				}
			}

			styleNode = headNode.SelectSingleNode("link[@rel=\"stylesheet\"]");
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
						cardDateNode = cardHeader.AppendHtml($"<div class=\"info\"><span class=\"date\">{card.Date:dd.MM.yyyy}</span></div>");
					}

					if (card.ModifiedDate != null)
					{
						if (cardDateNode != null)
						{
							cardDateNode.AppendHtml("<br>");
							cardDateNode.AppendHtml($"<span class=\"date moddate\">{card.ModifiedDate:dd.MM.yyyy}</span>");
						}
						else
						{
							cardDateNode = cardHeader.AppendHtml($"<div class=\"info\"><span class=\"date moddate\">{card.ModifiedDate:dd.MM.yyyy}</span></div>");
						}
					}

					if (card.DueDate != null)
					{
						if (cardDateNode != null)
						{
							cardDateNode.AppendHtml("<br>");
							cardDateNode.AppendHtml($"<span class=\"date duedate\">{card.DueDate:dd.MM.yyyy}</span>");
						}
						else
						{
							cardHeader.AppendHtml($"<div class=\"info\"><span class=\"date duedate\">{card.DueDate:dd.MM.yyyy}</span></div>");
						}
					}

					cardHeader.AppendHtml($"<div class=\"title\">{HtmlEncode(card.Title)}</div>");

					if (card.LabelIds != null && card.LabelIds.Count > 0 && todoDoc.Labels != null && todoDoc.Labels.Count > 0)
					{
						HtmlNode cardLabels = cardNode.AppendHtml("<ul class=\"labelflags\">");
						foreach (string lid in card.LabelIds)
						{
							Label? l = null;
							foreach (Label? rl in todoDoc.Labels)
							{
								if (rl.Id != null && rl.Id.Equals(lid, StringComparison.InvariantCultureIgnoreCase))
								{
									l = rl;
									break;
								}
							}
							if (l == null) continue;
							string n = Regex.Replace(l.Id ?? string.Empty, "[^a-zA-Z0-9]", "");
							if (string.IsNullOrEmpty(n)) continue;

							HtmlNode label = cardLabels.AppendHtml($"<li class=\"label{n}Colors\" title=\"{l.Title}\">{l.Title}</li>");
						}
					}

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
