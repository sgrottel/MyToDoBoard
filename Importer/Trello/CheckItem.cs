using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Trello
{
	internal class CheckItem
	{
		public string? id { get; set; }
		public string? name { get; set; }
		public object? nameData { get; set; }
		public double pos { get; set; }
		public CheckItemStateValue state { get; set; }
		public object? due { get; set; }
		public object? dueReminder { get; set; }
		public object? idMember { get; set; }
		// public string? idChecklist { get; set; } always id of owning checklist

		public override string ToString()
		{
			return $"{name}  [{state}]";
		}
	}
}
