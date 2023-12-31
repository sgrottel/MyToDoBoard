using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class Card
	{
		public string? id { get; set; }
		public object? address { get; set; }
		public object? badges { get; set; }
		public CheckItemState[]? checkItemStates { get; set; }
		public bool closed { get; set; }
		public object? coordinates { get; set; }
		public object? creationMethod { get; set; }
		public bool dueComplete { get; set; }
		public DateTime? dateLastActivity { get; set; }
		public string? desc { get; set; }
		public object? descData { get; set; }
		public DateTime? due { get; set; }
		public double? dueReminder { get; set; }
		public string? email { get; set; }
		// public string? idBoard { get; set; } always id of owning board
		public string[]? idChecklists { get; set; }
		public string[]? idLabels { get; set; }
		public string? idList { get; set; }
		public object?[]? idMembers { get; set; }
		public object?[]? idMembersVoted { get; set; }
		// public string? idOrganization { get; set; } always id of the organization of the owning board
		public double idShort { get; set; }
		public string? idAttachmentCover { get; set; }
		// public object?[]? labels { get; set; } other view of idLabels
		public object? limits { get; set; }
		public object? locationName { get; set; }
		public bool manualCoverAttachment { get; set; }
		public string? name { get; set; }
		public string? nodeId { get; set; }
		public double pos { get; set; }
		public string? shortLink { get; set; }
		public string? shortUrl { get; set; }
		public object? staticMapUrl { get; set; }
		public DateTime? start { get; set; }
		public bool subscribed { get; set; }
		public string? url { get; set; }
		public object? cover { get; set; }
		public bool isTemplate { get; set; }
		public string? cardRole { get; set; }
		public Attachment[]? attachments { get; set; }
		public object?[]? pluginData { get; set; }
		public object?[]? customFieldItems { get; set; }

		public override string ToString()
		{
			return name ?? nameof(Card);
		}
	}
}
