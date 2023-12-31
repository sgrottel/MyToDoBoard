using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class CardAction
	{
		public string? id { get; set; }
		public string? idMemberCreator { get; set; }
		public CardActionData? data { get; set; }
		public object? appCreator { get; set; }

		[JsonPropertyName("type")]
		public CardActionType ActionType { get; set; }
		public DateTime? date { get; set; }
		public object? limits { get; set; }
		public object? memberCreator { get; set; }
	}
}
