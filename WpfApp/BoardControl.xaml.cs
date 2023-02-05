﻿using MyToDoBoard.Data;
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

		internal ViewModel.BoardView BoardView { get; } = new ViewModel.BoardView();

		public BoardControl()
		{
			InitializeComponent();
			DataContext = BoardView;

			draggingCardControl.Visibility = Visibility.Collapsed;
			draggingCardControl.RenderTransform = new TranslateTransform();
		}

		Data.Card? draggingCard = null;
		Point draggingCardStart;

		private void CardControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				FrameworkElement? uiCard = sender as FrameworkElement;
				if (uiCard == null) return;
				Data.Card? card = uiCard.DataContext as Data.Card;
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
				while (fe != null && ((fe.DataContext as Data.Card) == null)) fe = fe.Parent as FrameworkElement;
				Data.Card? overCard = fe?.DataContext as Data.Card;
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
					while (fe != null && ((fe.DataContext as Data.Column) == null)) fe = fe.Parent as FrameworkElement;
					Data.Column? overColumn = fe?.DataContext as Data.Column;

					if (overColumn != null && overColumn.Cards != null && !overColumn.Cards.Contains(draggingCard))
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
			ViewModel.ColumnView? sc = null;
			ViewModel.ColumnView? dc = null;
			if (BoardView != null && BoardView.Columns != null)
			{
				foreach (ViewModel.ColumnView c in BoardView.Columns)
				{
					if (c.Cards == null) continue;
					if (c.Cards.Contains(card)) sc = c;
					if (c.Cards.Contains(dest)) dc = c;
				}
			}
			if (sc == null || dc == null) return;

			//int di = dc.Cards.IndexOf(dest);
			//sc.Cards.Remove(card);
			//dc.Cards.Insert(di, card);
		}

		private void moveCardTo(Card card, Column dest)
		{
			ViewModel.ColumnView? sc = null;
			if (BoardView != null && BoardView.Columns != null)
			{
				foreach (ViewModel.ColumnView c in BoardView.Columns)
				{
					if (c.Cards == null) continue;
					if (c.Cards.Contains(card)) sc = c;
				}
			}
			if (sc == null) return;

			//dest.Cards.Insert(0, card);
			//sc.Cards.Remove(card);
		}

	}
}
