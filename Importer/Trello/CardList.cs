using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class CardList
	{
		public string? id { get; set; }
		public string? name { get; set; }
		public bool closed { get; set; }
		public object? color { get; set; }
		// public string? idBoard { get; set; } always id of owning board
		public double pos { get; set; }
		public bool subscribed { get; set; }
		public object? softLimit { get; set; }
		public object? creationMethod { get; set; }
		// public string? idOrganization { get; set; } always if of organization of owning board
		public object? limits { get; set; }
		public string? nodeId { get; set; }

		public override string ToString()
		{
			return (name ?? nameof(CardList)) + (closed ? " [closed]" : "");
		}
	}
}
