using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class Board
	{
		public string? id { get; set; }
		public string? nodeId { get; set; }
		public string? name { get; set; }
		public string? desc { get; set; }
		public object? descData { get; set; }
		public bool closed { get; set; }
		public DateTime? dateClosed { get; set; }
		public string? idOrganization { get; set; }
		public object? idEnterprise { get; set; }
		public object? limits { get; set; }
		public bool pinned { get; set; }
		public bool starred { get; set; }
		public string? url { get; set; }
		public object? prefs { get; set; }
		public string? shortLink { get; set; }
		public bool subscribed { get; set; }
		// public LabelNames? labelNames { get; set; } // other view of 'labels'
		public object?[]? powerUps { get; set; }
		public DateTime? dateLastActivity { get; set; }
		public DateTime? dateLastView { get; set; }
		public string? shortUrl { get; set; }
		public object?[]? idTags { get; set; }
		public DateTime? datePluginDisable { get; set; }
		public object? creationMethod { get; set; }
		public string? ixUpdate { get; set; }
		public object? templateGallery { get; set; }
		public bool enterpriseOwned { get; set; }
		public object? idBoardSource { get; set; }
		public object?[]? premiumFeatures { get; set; }
		public string? idMemberCreator { get; set; }
		public CardAction[]? actions { get; set; }
		public Card[]? cards { get; set; }
		public LabelName[]? labels { get; set; }
		public CardList[]? lists { get; set; }
		public object?[]? members { get; set; }
		public CheckList[]? checklists { get; set; }
		public object?[]? customFields { get; set; }
		public object?[]? memberships { get; set; }
		public object?[]? pluginData { get; set; }
	}
}
