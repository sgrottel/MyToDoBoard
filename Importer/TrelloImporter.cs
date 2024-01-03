using Importer.DataModel;
using Importer.Trello;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Importer
{
	internal static class TrelloImporter
	{
		public static void Import(this ToDoDocument todoDoc, Board board)
		{
			Dictionary<string, string> labelId = new();
			todoDoc.Labels = new();
			foreach (LabelName ln in board.labels ?? Array.Empty<LabelName>())
			{
				if (ln.uses <= 0) continue;

				Label l = new Label()
				{
					Title = ln.name,
					Color = TrelloColor(ln.color)
				};

				string? knownLabelId = GetKnownLabelId(ln.name);
				if (knownLabelId != null && Label.TestId(knownLabelId, todoDoc.Labels))
				{
					l.Id = knownLabelId;
				}
				else
				{
					l.GenerateId(todoDoc.Labels);
				}

				todoDoc.Labels.Add(l);

				if (ln.id != null)
				{
					labelId.Add(ln.id, l.Id ?? string.Empty);
				}
			}

			Dictionary<string, Column> lists = new();
			Dictionary<string, Column> secondLists = new();
			todoDoc.Columns = new();
			foreach (CardList cl in board.lists ?? Array.Empty<CardList>())
			{
				Column c = new() { Title = cl.name, Order = (long)cl.pos };
				if (cl.closed)
				{
					c.View = ColumnView.hidden;
					c.Order += 10000000L;
				}

				todoDoc.Columns.Add(c);
				if (cl.id != null)
				{
					lists.Add(cl.id, c);

					if (c.View == ColumnView.DefaultView)
					{
						c = new() { Title = cl.name, View = ColumnView.hidden, Order = (long)cl.pos + 10000000L };
						secondLists.Add(cl.id, c);
					}
				}
			}

			Dictionary<string, DataModel.Card> cards = new();
			Dictionary<string, CheckListItem> checkListItems = new();
			foreach (Trello.Card tc in board.cards ?? Array.Empty<Trello.Card>())
			{
				if (tc.idList == null) continue;
				DataModel.Card c = new() { Title = tc.name, Description = CleanDescription(tc.desc), ModifiedDate = CleanDate(tc.dateLastActivity) };

				if (tc.attachments?.Any() ?? false)
				{
					foreach (Attachment at in tc.attachments)
					{
						string? l = at.url ?? at.fileName;
						if (l != null)
						{
							c.Links ??= new();
							c.Links.Add(l);
						}
					}
				}

				Column cc = lists[tc.idList];
				if (cc.View == ColumnView.DefaultView && tc.closed)
				{
					cc = secondLists[tc.idList];
				}

				cc.Cards!.Add(c);
				if (tc.id != null)
				{
					cards.Add(tc.id, c);
				}

				if (tc.idLabels != null && tc.idLabels.Length > 0)
				{
					c.LabelIds = tc.idLabels.Select((l) => labelId[l]).ToList();
				}

				if (tc.idChecklists != null && tc.idChecklists.Length > 0)
				{
					foreach (string tcclid in tc.idChecklists)
					{
						CheckList? cl = board.checklists?.First((cl) => cl.id == tcclid);
						if (cl == null) continue;

						c.Checklist ??= new();
						foreach (CheckItem ci in cl.checkItems ?? Array.Empty<CheckItem>())
						{
							CheckListItem cli = new() { Text = ci.name, Checked = ci.state == CheckItemStateValue.complete };
							c.Checklist.Add(cli);
							if (ci.id != null)
							{
								checkListItems.Add(ci.id, cli);
							}
						}
					}
				}

			}

			foreach (var ta in board.actions ?? Array.Empty<CardAction>())
			{
				switch (ta.ActionType)
				{
					case CardActionType.createCard:
						{
							string? cid = null;
							JsonElement? c = (ta.data?.card as JsonElement?);
							if (c != null && c.Value.ValueKind == JsonValueKind.Object)
							{
								JsonElement prop;
								if (c.Value.TryGetProperty("id", out prop))
								{
									cid = prop.GetString();
								}
							}

							if (cid != null && cards.ContainsKey(cid))
							{
								if (ta.date != null)
								{
									if (cards[cid].Date == null)
									{
										cards[cid].Date = ta.date;
									}
									else if (cards[cid].Date > ta.date)
									{
										cards[cid].Date = ta.date;
									}
								}
							}
						}
						break;

					case CardActionType.convertToCardFromCheckItem:
						goto case CardActionType.createCard;
					case CardActionType.copyCard:
						goto case CardActionType.createCard;
					case CardActionType.moveCardToBoard:
						goto case CardActionType.createCard;

					case CardActionType.updateCheckItemStateOnCard:
						{
							string? cid = null;
							string? cs = null;
							JsonElement? c = (ta.data?.checkItem as JsonElement?);
							if (c != null && c.Value.ValueKind == JsonValueKind.Object)
							{
								JsonElement prop;
								if (c.Value.TryGetProperty("id", out prop))
								{
									cid = prop.GetString();
								}
								if (c.Value.TryGetProperty("state", out prop))
								{
									cs = prop.GetString();
								}
							}
							if (cid != null && cs != null && string.Equals(cs, CheckItemStateValue.complete.ToString()) && checkListItems.ContainsKey(cid))
							{
								if (ta.date != null)
								{
									CheckListItem cli = checkListItems[cid];
									if (cli.Date == null)
									{
										cli.Date = ta.date;
									}
									else if (cli.Date < ta.date)
									{
										cli.Date = ta.date;
									}
								}
							}
						}
						break;

					case CardActionType.commentCard:
						{
							string? cid = null;
							JsonElement? c = (ta.data?.card as JsonElement?);
							if (c != null && c.Value.ValueKind == JsonValueKind.Object)
							{
								JsonElement prop;
								if (c.Value.TryGetProperty("id", out prop))
								{
									cid = prop.GetString();
								}
							}

							if (cid != null && cards.ContainsKey(cid))
							{
								Comment cm = new() {
									Text = ta.data?.text ?? string.Empty,
									Date = ta.date
								};

								cards[cid].Comments ??= new();
								cards[cid].Comments!.Insert(0, cm);
							}

						}
						break;

				}

			}

			// final cleanup
			todoDoc.Columns.RemoveAll((c) => !c.Cards?.Any() ?? true);
			foreach (Column cc in todoDoc.Columns)
			{
				foreach (DataModel.Card c in cc.Cards!)
				{
					if (c.LabelIds != null && !c.LabelIds.Any())
					{
						c.LabelIds = null;
					}
					if (c.Links != null && !c.Links.Any())
					{
						c.Links = null;
					}
					if (c.Checklist != null && !c.Checklist.Any())
					{
						c.Checklist = null;
					}
				}
			}
			todoDoc.Columns.AddRange(secondLists.Where((p) => p.Value.Cards?.Any() ?? false).Select((p) => p.Value));
			todoDoc.Columns.Sort((a, b) => (int)(a.Order - b.Order));
		}

		private static string? TrelloColor(string? color)
		{
			switch (color?.ToLower())
			{
				case "sky_dark": return "LightSkyBlue";
				case "blue_light": return "RoyalBlue";
				case "red_dark": return "FireBrick";
				case "orange": return "Peru";
				case "purple_dark": return "Indigo";
				case "pink": return "HotPink";
				case "lime": return "MediumSeaGreen";
				case "green": return "Green";
			}
			return null;
		}

		private static string? GetKnownLabelId(string? name)
		{
			switch (name?.ToLower())
			{
				case "software": return "SW";
				case "software cloud": return "SWC";
				case "prio": return "P+";
				case "real life": return "RL";
				case "makerly": return "Mk";
				case "smut": return "Sx";
				case "geld & stuff": return "€";
				case "chihaya project": return "Chihaya";
			}
			return null;
		}

		private static DateTime? CleanDate(DateTime? d)
		{
			if (d == null) return null;
			return new DateTime(
				d.Value.Year,
				d.Value.Month,
				d.Value.Day,
				d.Value.Hour,
				d.Value.Minute,
				d.Value.Second);
		}

		private static string? CleanDescription(string? desc)
		{
			if (desc == null) return null;
			return desc.Replace("\u200C", "").Replace("\r", "");
		}

	}
}
