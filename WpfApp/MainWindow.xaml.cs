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

			board.Columns = new[]
			{
				new DataModel.Column()
				{
					Title = "Todo",
					BackgroundColor = Color.FromArgb(128, 200, 200, 200),
					Cards = new[]
					{
						new DataModel.Card()
						{
							Title = "A1 Card with a long Title to Test Text Wrapping",
							Labels = new DataModel.Label[]
							{
								new DataModel.Label() { Name = "1", Color = Colors.PowderBlue },
								new DataModel.Label() { Name = "2", Color = Colors.Lime }
							}
						},
						new DataModel.Card() { Title = "A2" },
						new DataModel.Card() { Title = "A3" },
						new DataModel.Card() { Title = "A4" },
						new DataModel.Card() { Title = "A5" },
						new DataModel.Card() { Title = "A6" },
						new DataModel.Card() { Title = "A7" },
						new DataModel.Card() { Title = "A8" },
					}
				},
				new DataModel.Column()
				{
					Title = "Ready",
					BackgroundColor = Color.FromArgb(128, 150, 255, 150),
					Cards = new[]
					{
						new DataModel.Card() { Title = "B1" },
						new DataModel.Card() { Title = "B2" },
						new DataModel.Card() { Title = "B3" },
						new DataModel.Card() { Title = "B4" },
					}
				},
				new DataModel.Column()
				{
					Title = "Doing",
					BackgroundColor = Color.FromArgb(128, 150, 150, 255)
				},
				new DataModel.Column()
				{
					Title = "Done",
					BackgroundColor = Color.FromArgb(128, 150, 150, 150),
					Cards = new[]
					{
						new DataModel.Card() { Title = "C1" },
						new DataModel.Card() { Title = "C2" },
						new DataModel.Card() { Title = "C3" },
						new DataModel.Card() { Title = "C4" },
						new DataModel.Card() { Title = "C5" },
						new DataModel.Card() { Title = "C6" },
						new DataModel.Card() { Title = "C7" },
						new DataModel.Card() { Title = "C8" },
					}
				}
			};

			boardView.BoardView.Data = board;

		}
	}
}
