using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class CardActionData
	{
		public object? card { get; set; }
		public object? old { get; set; }
		public object? board { get; set; }
		public object? list { get; set; }
		public object? listBefore { get; set; }
		public object? listAfter { get; set; }
		public string? text { get; set; }
		public object? textData { get; set; }
		public object? checklist { get; set; }
		public object? checkItem { get; set; }
		public object? attachment { get; set; }
		public object? plugin { get; set; }
		public object? cardSource { get; set; }
		public object? boardSource { get; set; }
	}
}
