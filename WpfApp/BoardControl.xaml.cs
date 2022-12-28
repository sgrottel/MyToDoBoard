using MyToDoBoard.Board;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyToDoBoard
{
	/// <summary>
	/// Interaction logic for BoardControl.xaml
	/// </summary>
	public partial class BoardControl : UserControl
	{

		public ObservableCollection<Board.Column> Columns
		{
			get {
				ObservableCollection<Board.Column>? c = DataContext as ObservableCollection<Board.Column>;
				if (c == null)
				{
					DataContext = c = new ObservableCollection<Board.Column>();
				}
				return c;
			}
			set {
				if (DataContext != value)
				{
					DataContext = value ?? new ObservableCollection<Board.Column>();
				}
			}
		}

		public BoardControl()
		{
			InitializeComponent();
			DataContext = new ObservableCollection<Board.Column>();

			draggingCardControl.Visibility = Visibility.Collapsed;
			draggingCardControl.RenderTransform = new TranslateTransform();
		}

		Board.Card? draggingCard = null;
		Point draggingCardStart;

		private void CardControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				FrameworkElement? uiCard = sender as FrameworkElement;
				if (uiCard == null) return;
				Board.Card? card = uiCard.DataContext as Board.Card;
				if (card == null) return;

				if (draggingCard == null)
				{
					draggingCardControl.DataContext = card;
					draggingCardControl.Visibility = Visibility.Visible;

					if (draggingCardControl.CaptureMouse())
					{
						draggingCard = card;
						draggingCard.DraggingVisibility = Visibility.Hidden; // not collapsed, to keep the layout space where the card will drop

						draggingCardControl.Width = uiCard.ActualWidth;
						draggingCardControl.Height = uiCard.ActualHeight;
						TranslateTransform move = (TranslateTransform)draggingCardControl.RenderTransform;

						GeneralTransform t = uiCard.TransformToAncestor(mainView);
						Point o = t.Transform(new Point(0, 0));

						move.X = o.X;
						move.Y = o.Y;

						Point p = Mouse.GetPosition(mainView);
						draggingCardStart.X = p.X - move.X;
						draggingCardStart.Y = p.Y - move.Y;
					}
					else
					{
						draggingCardControl.DataContext = null;
						draggingCardControl.Visibility = Visibility.Collapsed;
					}
				}
			}
		}

		private void CardControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (draggingCard != null)
			{
				Point p = Mouse.GetPosition(mainView);

				FrameworkElement? fe = mainView.InputHitTest(p) as FrameworkElement;
				while (fe != null && ((fe.DataContext as Board.Card) == null)) fe = fe.Parent as FrameworkElement;
				Board.Card? overCard = fe?.DataContext as Board.Card;
				if (overCard == draggingCard)
				{
					overCard = null;
				}

				if (overCard != null)
				{
					moveCardTo(draggingCard, overCard);
				}
				else
				{
					// check if we are dragging into a different column (potentionally an empty one, without cards)
					fe = mainView.InputHitTest(p) as FrameworkElement;
					while (fe != null && ((fe.DataContext as Board.Column) == null)) fe = fe.Parent as FrameworkElement;
					Board.Column? overColumn = fe?.DataContext as Board.Column;

					if (overColumn != null && !overColumn.Cards.Contains(draggingCard))
					{
						moveCardTo(draggingCard, overColumn);
					}
				}

				TranslateTransform move = (TranslateTransform)draggingCardControl.RenderTransform;

				move.X = p.X - draggingCardStart.X;
				move.Y = p.Y - draggingCardStart.Y;
			}
		}

		private void CardControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (draggingCard != null)
			{
				draggingCardControl.ReleaseMouseCapture();

				draggingCardControl.DataContext = null;
				draggingCardControl.Visibility = Visibility.Collapsed;

				draggingCard.DraggingVisibility = Visibility.Visible;
				draggingCard = null;
			}
		}

		private void moveCardTo(Card card, Card dest)
		{
			Column? sc = null;
			Column? dc = null;
			foreach (Column c in Columns)
			{
				if (c.Cards.Contains(card)) sc = c;
				if (c.Cards.Contains(dest)) dc = c;
			}
			if (sc == null || dc == null) return;

			int si = sc.Cards.IndexOf(card);
			int di = dc.Cards.IndexOf(dest);

			if (sc == dc)
			{
				sc.Cards.Move(si, di);
			}
			else
			{
				dc.Cards.Insert(di, card);
				sc.Cards.Remove(card);
			}
		}

		private void moveCardTo(Card card, Column dest)
		{
			Column? sc = null;
			foreach (Column c in Columns)
			{
				if (c.Cards.Contains(card)) sc = c;
			}
			if (sc == null) return;

			dest.Cards.Insert(0, card);
			sc.Cards.Remove(card);
		}

	}
}
