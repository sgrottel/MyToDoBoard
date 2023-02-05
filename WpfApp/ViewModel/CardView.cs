using MyToDoBoard.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyToDoBoard.ViewModel
{
	internal class CardView : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private Card card;
		private ColumnView columnView;
		private BoardView boardView;
		public Board? board = null;

		public CardView(Card card, ColumnView columnView, BoardView boardView)
		{
			this.card = card;
			this.card.PropertyChanged += Card_PropertyChanged;
			this.columnView = columnView;
			this.columnView.Data.PropertyChanged += ColumnViewData_PropertyChanged;
			this.boardView = boardView;
			this.boardView.PropertyChanged += BoardView_PropertyChanged;
			board = this.boardView.Data;
			if (board != null)
			{
				board.PropertyChanged += Board_PropertyChanged;
			}
			UpdateMargin();
			UpdateBackgroundBrush();
		}

		private void Card_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender != card) return;
			switch (e.PropertyName)
			{
				case nameof(Card.Title):
					PropertyChanged?.Invoke(this, new(nameof(Title)));
					break;
				case nameof(Card.Labels):
					PropertyChanged?.Invoke(this, new(nameof(Labels)));
					UpdateMargin();
					break;
				case nameof(Card.Color):
					UpdateBackgroundBrush();
					break;
			}
		}

		private void ColumnViewData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender != columnView.Data) return;
			switch (e.PropertyName)
			{
				case nameof(Column.DefaultCardColor):
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
					board = boardView.Data;
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
				case nameof(Board.DefaultCardColor):
					UpdateBackgroundBrush();
					break;
			}
		}

		public Card Data { get => card; }

		public string Title { get => card.Title; }

		public Label[]? Labels { get => card.Labels; }

		/// <summary>
		/// Visibility of this card changes to hidden when being dragged.
		/// This is not a Model attribute. It is a ViewModel attribute.
		/// </summary>
		public Visibility DraggingVisibility
		{
			get => draggingVisibility;
			set
			{
				if (draggingVisibility != value)
				{
					draggingVisibility = value;
					PropertyChanged?.Invoke(this, new(nameof(DraggingVisibility)));
				}
			}
		}
		private Visibility draggingVisibility = Visibility.Visible;

		private static readonly Thickness marginDefault = new Thickness(4);
		private static readonly Thickness marginWithLabels = new Thickness(4, 8, 4, 4);

		public Thickness Margin { get; private set; } = marginDefault;

		private void UpdateMargin()
		{
			Thickness next = marginDefault;
			if (card.Labels != null && card.Labels.Length > 0)
			{
				next = marginWithLabels;
			}
			if (Margin != next)
			{
				Margin = next;
				PropertyChanged?.Invoke(this, new(nameof(Margin)));
			}
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

		public void UpdateBackgroundBrush()
		{
			if (card.Color != null)
			{
				UpdateBackgroundBrush(card.Color.Value);
				return;
			}

			if (columnView.Data.DefaultCardColor != null)
			{
				UpdateBackgroundBrush(columnView.Data.DefaultCardColor.Value);
				return;
			}

			if (board?.DefaultCardColor != null)
			{
				UpdateBackgroundBrush(board.DefaultCardColor.Value);
				return;
			}

			UpdateBackgroundBrush(Colors.Transparent);
		}

	}
}
