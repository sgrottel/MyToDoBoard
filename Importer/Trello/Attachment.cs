using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class Attachment
	{
		public string? id { get; set; }
		public object? bytes { get; set; }
		public DateTime? date { get; set; }
		public object? edgeColor { get; set; }
		public string? idMember { get; set; }
		public bool isUpload { get; set; }
		public string? mimeType { get; set; }
		public string? name { get; set; }
		public object?[]? previews { get; set; }
		public string? url { get; set; }
		public double pos { get; set; }
		public string? fileName { get; set; }
	}
}
