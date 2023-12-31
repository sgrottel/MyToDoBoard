using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class LabelName
	{
		public string? id { get; set; }
		//public string? idBoard { get; set; } // always id of parent board object
		public string? name { get; set; }
		public string? color { get; set; }
		public ulong uses { get; set; }
	}
}
