using MyToDoBoard.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyToDoBoard.ViewModel
{
	internal class ColumnView : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler? PropertyChanged;

		private Column data;
		private BoardView boardView;
		private Board? board;

		internal ColumnView(BoardView boardView, Column column)
		{
			this.boardView = boardView;
			data = column;
			data.PropertyChanged += Data_PropertyChanged;
			this.boardView.PropertyChanged += BoardView_PropertyChanged;
			board = this.boardView.Data;
			if (board != null)
			{
				board.PropertyChanged += Board_PropertyChanged;
			}
			UpdateBackgroundBrush();
		}

		private void Data_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender != data) return;
			switch (e.PropertyName)
			{
				case nameof(Column.Title):
					PropertyChanged?.Invoke(this, new(nameof(Title)));
					break;
				case nameof(Column.Cards):
					PropertyChanged?.Invoke(this, new(nameof(Cards)));
					break;
				case nameof(Column.BackgroundColor):
					UpdateBackgroundBrush();
					break;
			}
		}

		private void BoardView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender != boardView) return;
			switch (e.PropertyName)
			{
				case nameof(BoardView.Data):
					if (board != null)
					{
						board.PropertyChanged -= Board_PropertyChanged;
					}
					board = this.boardView.Data;
					if (board != null)
					{
						board.PropertyChanged += Board_PropertyChanged;
					}
					break;
			}
		}

		private void Board_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender != board) return;
			switch (e.PropertyName)
			{
				case nameof(Board.DefaultColumnColor):
					UpdateBackgroundBrush();
					break;
			}
		}

		public Column Data { get => data; }
		public string Title { get => data.Title; }
		public Card[]? Cards { get => data.Cards; }
		public Brush BackgroundBrush { get; private set; } = Brushes.Transparent;

		private void UpdateBackgroundBrush(Color color)
		{
			SolidColorBrush? scb = BackgroundBrush as SolidColorBrush;
			if (scb != null)
			{
				if (scb.Color == color) return;
			}
			BackgroundBrush = new SolidColorBrush(color);
			PropertyChanged?.Invoke(this, new(nameof(BackgroundBrush)));
		}

		private void UpdateBackgroundBrush()
		{
			if (data.BackgroundColor != null)
			{
				UpdateBackgroundBrush(data.BackgroundColor.Value);
				return;
			}

			if (board?.DefaultColumnColor != null)
			{
				UpdateBackgroundBrush(board.DefaultColumnColor.Value);
				return;
			}

			if (BackgroundBrush == Brushes.Transparent) return;

			BackgroundBrush = Brushes.Transparent;
			PropertyChanged?.Invoke(this, new(nameof(BackgroundBrush)));
		}

	}
}
