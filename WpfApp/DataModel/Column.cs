﻿using System.ComponentModel;
using System.Windows.Media;

namespace MyToDoBoard.DataModel
{
	public class Column : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;
		private Card[]? cards = null;
		private Color? backgroundColor = null;
		private Color? defaultCardColor = null;

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
					PropertyChanged?.Invoke(this, new(nameof(Background)));
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

		#region TODO Move to ViewModel

		public Brush Background
		{
			get => (backgroundColor != null) ? new SolidColorBrush(backgroundColor.Value) : Brushes.Transparent;
		}

		#endregion

	}

}
