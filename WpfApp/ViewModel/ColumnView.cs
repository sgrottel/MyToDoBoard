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
			UpdateCards();
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
					UpdateCards();
					break;
				case nameof(Column.BackgroundColor):
					UpdateBackgroundBrush();
					break;
				case nameof(Column.Width):
					PropertyChanged?.Invoke(this, new(nameof(Width)));
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
					UpdateBackgroundBrush();
					PropertyChanged?.Invoke(this, new(nameof(Width)));
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
				case nameof(Board.DefaultColumnWidth):
					PropertyChanged?.Invoke(this, new(nameof(Width)));
					break;
			}
		}

		public Column Data { get => data; }
		public string Title { get => data.Title; }
		public CardView[] Cards { get; private set; } = Array.Empty<CardView>();

		private void UpdateCards()
		{
			if (data == null || data.Cards == null || data.Cards.Length == 0)
			{
				if (Cards.Length != 0)
				{
					Cards = Array.Empty<CardView>();
					PropertyChanged?.Invoke(this, new(nameof(Cards)));
				}
				return;
			}

			if (data.Cards.Length == Cards.Length)
			{
				bool match = true;
				for (int idx = 0; idx < Cards.Length; ++idx)
				{
					if (Cards[idx] == null)
					{
						match = false;
						break;
					}
					if (Cards[idx].Data != data.Cards[idx])
					{
						match = false;
						break;
					}
				}
				if (match)
				{
					return;
				}
			}

			Cards = new CardView[data.Cards.Length];
			for (int idx = 0; idx < Cards.Length; ++idx)
			{
				Cards[idx] = new CardView(data.Cards[idx], this, boardView);
			}
			PropertyChanged?.Invoke(this, new(nameof(Cards)));
		}

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

		public double Width
		{
			get
			{
				if (data.Width > 0.1) return data.Width;
				return board?.DefaultColumnWidth ?? Column.DefaultWidth;
			}
		}

	}
}
