using MyToDoBoard.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyToDoBoard.ViewModel
{
	internal class BoardView : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler? PropertyChanged;

		private Board? data = null;
		public Board? Data
		{
			get => data; set
			{
				if (data != value)
				{
					if (data != null)
					{
						data.PropertyChanged -= Data_PropertyChanged;
					}
					data = value;
					if (data != null)
					{
						data.PropertyChanged += Data_PropertyChanged;
					}
					PropertyChanged?.Invoke(this, new(nameof(Data)));
					PropertyChanged?.Invoke(this, new(nameof(Columns)));
					PropertyChanged?.Invoke(this, new(nameof(Labels)));
					UpdateBackgroundBrush();
				}
			}
		}

		private void Data_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender != data) return;
			switch (e.PropertyName)
			{
				case nameof(Board.Columns):
					PropertyChanged?.Invoke(this, new(nameof(Columns)));
					break;
				case nameof(Board.Labels):
					PropertyChanged?.Invoke(this, new(nameof(Labels)));
					break;
				case nameof(Board.BackgroundColor):
				case nameof(Board.BackgroundImage):
					UpdateBackgroundBrush();
					break;
			}
		}

		public Column[]? Columns { get => data?.Columns; }

		public Label[]? Labels { get => data?.Labels; }

		public Brush BackgroundBrush { get; set; } = Brushes.Transparent;

		private void SetTransparentBackgroundBrush()
		{
			if (BackgroundBrush != Brushes.Transparent)
			{
				BackgroundBrush = Brushes.Transparent;
				PropertyChanged?.Invoke(this, new(nameof(BackgroundBrush)));
			}
		}

		private void UpdateBackgroundBrush()
		{
			try
			{
				if (data == null)
				{
					SetTransparentBackgroundBrush();
					return;
				}

				if (!string.IsNullOrEmpty(data.BackgroundImage) && System.IO.File.Exists(data.BackgroundImage))
				{
					ImageBrush? ib = BackgroundBrush as ImageBrush;
					if (ib != null)
					{
						BitmapImage? bi = ib.ImageSource as BitmapImage;
						if (bi != null && bi.UriSource == new Uri(data.BackgroundImage))
						{
							return;
						}
					}
					try
					{
						ib = new ImageBrush(new BitmapImage(new Uri(data.BackgroundImage)))
						{
							Stretch = Stretch.UniformToFill,
							AlignmentX = AlignmentX.Center,
							AlignmentY = AlignmentY.Center
						};
						BackgroundBrush = ib;
						PropertyChanged?.Invoke(this, new(nameof(BackgroundBrush)));
						return;
					}
					catch
					{
					}
				}

				if (data.BackgroundColor != null)
				{
					SolidColorBrush? scb = BackgroundBrush as SolidColorBrush;
					if (scb != null)
					{
						if (scb.Color == data.BackgroundColor)
						{
							return;
						}
					}
					BackgroundBrush = new SolidColorBrush(data.BackgroundColor.Value);
					PropertyChanged?.Invoke(this, new(nameof(BackgroundBrush)));
					return;
				}

			}
			catch { }

			SetTransparentBackgroundBrush();
		}

	}
}
