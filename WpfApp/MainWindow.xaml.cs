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
		public MainWindow()
		{
			InitializeComponent();

			var co = new ObservableCollection<Board.Column>();
			DataContext = co;
			var c = new Board.Column() { Title = "Todo", Background = new SolidColorBrush(Color.FromArgb(128,200,200,200)) };
			c.Cards.Add(new Board.Card() { Title = "A1 Card with a long Title to Test Text Wrapping" });
			c.Cards.Add(new Board.Card() { Title = "A2" });
			c.Cards.Add(new Board.Card() { Title = "A3" });
			c.Cards.Add(new Board.Card() { Title = "A4" });
			c.Cards.Add(new Board.Card() { Title = "A5" });
			c.Cards.Add(new Board.Card() { Title = "A6" });
			c.Cards.Add(new Board.Card() { Title = "A7" });
			c.Cards.Add(new Board.Card() { Title = "A8" });
			co.Add(c);
			c = new Board.Column() { Title = "Ready", Background = new SolidColorBrush(Color.FromArgb(128, 150, 255, 150)) };
			c.Cards.Add(new Board.Card() { Title = "B1" });
			c.Cards.Add(new Board.Card() { Title = "B2" });
			c.Cards.Add(new Board.Card() { Title = "B3" });
			c.Cards.Add(new Board.Card() { Title = "B4" });
			co.Add(c);
			c = new Board.Column() { Title = "Doing", Background = new SolidColorBrush(Color.FromArgb(128, 150, 150, 255)) };
			co.Add(c);
			c = new Board.Column() { Title = "Done", Background = new SolidColorBrush(Color.FromArgb(128, 150, 150, 150)) };
			c.Cards.Add(new Board.Card() { Title = "C1" });
			c.Cards.Add(new Board.Card() { Title = "C2" });
			c.Cards.Add(new Board.Card() { Title = "C3" });
			c.Cards.Add(new Board.Card() { Title = "C4" });
			c.Cards.Add(new Board.Card() { Title = "C5" });
			c.Cards.Add(new Board.Card() { Title = "C6" });
			c.Cards.Add(new Board.Card() { Title = "C7" });
			c.Cards.Add(new Board.Card() { Title = "C8" });
			co.Add(c);

			board.Columns = co;
		}
	}
}
