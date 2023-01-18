using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDoBoard.DataModel
{
	public class Board
	{
		public ColumnCollection Columns { get; } = new ColumnCollection();
		public LabelCollection Labels { get; } = new LabelCollection();
	}
}
