using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDoBoard.Board
{

	public class Column : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;

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

		private ObservableCollection<Card> cards = new();
		public ObservableCollection<Card> Cards
		{
			get => cards;
			set
			{
				if (cards != value)
				{
					cards = value;
					PropertyChanged?.Invoke(this, new(nameof(Cards)));
				}
			}
		}

	}

}
