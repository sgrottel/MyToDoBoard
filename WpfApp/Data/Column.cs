using System.ComponentModel;
using System.Windows.Media;

namespace MyToDoBoard.Data
{
	public class Column : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;
		private Card[]? cards = null;
		private Color? backgroundColor = null;
		private Color? defaultCardColor = null;
		private double width = 0.0;

		public const double DefaultWidth = 450.0;

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

		public Card[]? Cards
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

		public Color? BackgroundColor
		{
			get => backgroundColor;
			set
			{
				if (backgroundColor != value)
				{
					backgroundColor = value;
					PropertyChanged?.Invoke(this, new(nameof(BackgroundColor)));
				}
			}
		}

		public Color? DefaultCardColor
		{
			get => defaultCardColor;
			set
			{
				if (defaultCardColor != value)
				{
					defaultCardColor = value;
					PropertyChanged?.Invoke(this, new(nameof(DefaultCardColor)));
				}
			}
		}

		public double Width
		{
			get => width;
			set
			{
				if (System.Math.Abs(width - value) > 0.01)
				{
					width = value;
					PropertyChanged?.Invoke(this, new(nameof(Width)));
				}
			}
		}

	}

}
