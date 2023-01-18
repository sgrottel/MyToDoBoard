using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyToDoBoard.DataModel
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

		private Brush background = Brushes.WhiteSmoke;
		public Brush Background
		{
			get => background;
			set
			{
				if (background != value)
				{
					background = value;
					PropertyChanged?.Invoke(this, new(nameof(Background)));
				}
			}
		}

		public CardCollection Cards { get; } = new CardCollection();

	}

}
