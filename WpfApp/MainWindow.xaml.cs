using System;
using System.Collections.Generic;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			draggingCardControl.Visibility = Visibility.Collapsed;
			draggingCardControl.RenderTransform = new TranslateTransform();
		}

		FrameworkElement? draggingCardVisual = null;
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

				if (draggingCardVisual == null)
				{
					draggingCardControl.DataContext = card;
					draggingCardControl.Visibility = Visibility.Visible;

					if (draggingCardControl.CaptureMouse())
					{
						draggingCardControl.Width = uiCard.ActualWidth;
						draggingCardControl.Height = uiCard.ActualHeight;
						TranslateTransform move = (TranslateTransform)draggingCardControl.RenderTransform;

						GeneralTransform t = uiCard.TransformToAncestor(BoardControl);
						Point o = t.Transform(new Point(0, 0));

						move.X = o.X;
						move.Y = o.Y;

						Point p = Mouse.GetPosition(BoardControl);
						draggingCardStart.X = p.X - move.X;
						draggingCardStart.Y = p.Y - move.Y;

						draggingCardVisual = uiCard;
						draggingCardVisual.Visibility = Visibility.Hidden;
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
			if (draggingCardVisual != null)
			{
				Point p = Mouse.GetPosition(BoardControl);

				TranslateTransform move = (TranslateTransform)draggingCardControl.RenderTransform;

				move.X = p.X - draggingCardStart.X;
				move.Y = p.Y - draggingCardStart.Y;
			}
		}

		private void CardControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (draggingCardVisual != null)
			{
				draggingCardVisual.ReleaseMouseCapture();
				draggingCardVisual.Visibility = Visibility.Visible;
				draggingCardVisual.RenderTransform = null;
				draggingCardVisual = null;
				draggingCardControl.DataContext = null;
				draggingCardControl.Visibility = Visibility.Collapsed;
			}
		}
	}
}
