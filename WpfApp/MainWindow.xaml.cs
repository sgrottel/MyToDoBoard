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

		private Data.Board board = new Data.Board();

		public MainWindow()
		{
			InitializeComponent();

			board.Columns = new[]
			{
				new Data.Column()
				{
					Title = "Todo",
					BackgroundColor = Color.FromArgb(128, 200, 200, 200),
					Cards = new[]
					{
						new Data.Card()
						{
							Title = "A1 Card with a long Title to Test Text Wrapping",
							Labels = new Data.Label[]
							{
								new Data.Label() { Name = "1", Color = Colors.PowderBlue },
								new Data.Label() { Name = "2", Color = Colors.Lime }
							}
						},
						new Data.Card() { Title = "A2" },
						new Data.Card() { Title = "A3" },
						new Data.Card() { Title = "A4" },
						new Data.Card() { Title = "A5" },
						new Data.Card() { Title = "A6" },
						new Data.Card() { Title = "A7" },
						new Data.Card() { Title = "A8" },
					}
				},
				new Data.Column()
				{
					Title = "Ready",
					BackgroundColor = Color.FromArgb(128, 150, 255, 150),
					Cards = new[]
					{
						new Data.Card() { Title = "B1" },
						new Data.Card() { Title = "B2" },
						new Data.Card() { Title = "B3" },
						new Data.Card() { Title = "B4" },
					}
				},
				new Data.Column()
				{
					Title = "Doing",
					BackgroundColor = Color.FromArgb(128, 150, 150, 255)
				},
				new Data.Column()
				{
					Title = "Done",
					BackgroundColor = Color.FromArgb(128, 150, 150, 150),
					Cards = new[]
					{
						new Data.Card() { Title = "C1" },
						new Data.Card() { Title = "C2" },
						new Data.Card() { Title = "C3" },
						new Data.Card() { Title = "C4" },
						new Data.Card() { Title = "C5" },
						new Data.Card() { Title = "C6" },
						new Data.Card() { Title = "C7" },
						new Data.Card() { Title = "C8" },
					}
				}
			};

			boardView.BoardView.Data = board;

		}
	}
}
