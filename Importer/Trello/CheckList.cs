using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class CheckList
	{
		public string? id { get; set; }
		public string? name { get; set; }
		// public string? idBoard { get; set; } always same as id of owning board
		public string? idCard { get; set; }	// backref to card
		public double pos { get; set; }
		public object? limits { get; set; }
		public CheckItem[]? checkItems { get; set; }
		public object? creationMethod { get; set; }
	}
}
