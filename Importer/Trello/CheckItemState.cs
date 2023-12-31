using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class CheckItemState
	{
		public string? idCheckItem { get; set; }
		public CheckItemStateValue state { get; set; }
	}
}
