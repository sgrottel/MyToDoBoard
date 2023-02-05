﻿using System.ComponentModel;
using System.Windows.Media;

namespace MyToDoBoard.Data
{
	public class Board : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private Column[]? columns = null;
		private Label[]? labels = null;
		private Color? backgroundColor = null;
		private string? backgroundImage = null;
		private Color? defaultColumnColor = null;
		private Color? defaultCardColor = null;
		private double defaultColumnWidth = Column.DefaultWidth;
		/// <summary>
		/// Mirrows all `columns` on demand, for archived cards
		/// </summary>
		private Column[]? archive = null;

		public Column[]? Columns
		{
			get => columns;
			set
			{
				if (columns != value)
				{
					columns = value;
					PropertyChanged?.Invoke(this, new(nameof(Columns)));
				}
			}
		}

		public Label[]? Labels
		{
			get => labels;
			set
			{
				if (labels != value)
				{
					labels = value;
					PropertyChanged?.Invoke(this, new(nameof(Labels)));
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

		public string? BackgroundImage
		{
			get => backgroundImage;
			set
			{
				if (backgroundImage != value)
				{
					backgroundImage = value;
					PropertyChanged?.Invoke(this, new(nameof(BackgroundImage)));
				}
			}
		}

		public Color? DefaultColumnColor
		{
			get => defaultColumnColor;
			set
			{
				if (defaultColumnColor != value)
				{
					defaultColumnColor = value;
					PropertyChanged?.Invoke(this, new(nameof(DefaultColumnColor)));
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

		public double DefaultColumnWidth
		{
			get => defaultColumnWidth;
			set
			{
				if (System.Math.Abs(defaultColumnWidth - value) > 0.01)
				{
					defaultColumnWidth = value;
					PropertyChanged?.Invoke(this, new(nameof(DefaultColumnWidth)));
				}
			}
		}

		public Column[]? Archive
		{
			get => archive;
			set
			{
				if (archive != value)
				{
					archive = value;
					PropertyChanged?.Invoke(this, new(nameof(Archive)));
				}
			}
		}

	}
}