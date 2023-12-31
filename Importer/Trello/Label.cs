using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class Label
	{
		public string? id { get; set; }
		//public string? idBoard { get; set; }
		public string? name { get; set; }
		public string? color { get; set; }
		public ulong uses { get; set; }
	}
}
