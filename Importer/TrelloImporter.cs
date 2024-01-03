using Importer.DataModel;
using Importer.Trello;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
					Color = ln.color
				};

				l.GenerateId(todoDoc.Labels);

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
						c.Checklist.AddRange(
							cl.checkItems?.Select((i) => new CheckListItem() { Text = i.name, Checked = i.state == CheckItemStateValue.complete })
							?? Array.Empty<CheckListItem>()
							);
					}
				}

			}

			foreach (var ta in board.actions ?? Array.Empty<CardAction>())
			{
				switch (ta.ActionType)
				{
					case CardActionType.addAttachmentToCard:
						break;
					case CardActionType.commentCard:
						break;
					case CardActionType.convertToCardFromCheckItem:
						break;
					case CardActionType.copyCard:
						break;
					case CardActionType.createCard:
						break;
					case CardActionType.moveCardToBoard:
						break;
					case CardActionType.updateCard:
						break;
					case CardActionType.updateCheckItemStateOnCard:
						break;
				}

				// TODO: Implement Comments & Dates

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
