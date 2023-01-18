using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private DataModel.Board board = new DataModel.Board();

		public MainWindow()
		{
			InitializeComponent();

			DataContext = board.Columns;

			var c = new DataModel.Column() { Title = "Todo", Background = new SolidColorBrush(Color.FromArgb(128, 200, 200, 200)) };
			c.Cards.Add(new DataModel.Card()
			{
				Title = "A1 Card with a long Title to Test Text Wrapping",
				Labels = new DataModel.Label[]
				{
					new DataModel.Label() { Name = "1", Color = Colors.PowderBlue },
					new DataModel.Label() { Name = "2", Color = Colors.Lime }
				}
			});
			c.Cards.Add(new DataModel.Card() { Title = "A2" });
			c.Cards.Add(new DataModel.Card() { Title = "A3" });
			c.Cards.Add(new DataModel.Card() { Title = "A4" });
			c.Cards.Add(new DataModel.Card() { Title = "A5" });
			c.Cards.Add(new DataModel.Card() { Title = "A6" });
			c.Cards.Add(new DataModel.Card() { Title = "A7" });
			c.Cards.Add(new DataModel.Card() { Title = "A8" });
			board.Columns.Add(c);

			c = new DataModel.Column() { Title = "Ready", Background = new SolidColorBrush(Color.FromArgb(128, 150, 255, 150)) };
			c.Cards.Add(new DataModel.Card() { Title = "B1" });
			c.Cards.Add(new DataModel.Card() { Title = "B2" });
			c.Cards.Add(new DataModel.Card() { Title = "B3" });
			c.Cards.Add(new DataModel.Card() { Title = "B4" });
			board.Columns.Add(c);

			c = new DataModel.Column() { Title = "Doing", Background = new SolidColorBrush(Color.FromArgb(128, 150, 150, 255)) };
			board.Columns.Add(c);

			c = new DataModel.Column() { Title = "Done", Background = new SolidColorBrush(Color.FromArgb(128, 150, 150, 150)) };
			c.Cards.Add(new DataModel.Card() { Title = "C1" });
			c.Cards.Add(new DataModel.Card() { Title = "C2" });
			c.Cards.Add(new DataModel.Card() { Title = "C3" });
			c.Cards.Add(new DataModel.Card() { Title = "C4" });
			c.Cards.Add(new DataModel.Card() { Title = "C5" });
			c.Cards.Add(new DataModel.Card() { Title = "C6" });
			c.Cards.Add(new DataModel.Card() { Title = "C7" });
			c.Cards.Add(new DataModel.Card() { Title = "C8" });
			board.Columns.Add(c);

			boardView.BoardView.Data = board;

		}
	}
}
