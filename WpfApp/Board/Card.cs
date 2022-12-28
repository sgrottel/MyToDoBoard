using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDoBoard.Board
{

	public class Card : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;

		/*
			- Title & Icon
			- Labels
			- Due Date (for displaying purpose only)
			- Description
			- Checklist
			- List of Links
			- List of other Cards this one depends on
			- Comments
		 */

		public string Title
		{
			get => title;
			set
			{
				if (title != value)
				{
					title = value;
					PropertyChanged?.Invoke(this, new(nameof(Title)));
				}
			}
		}

	}

}
