using System.ComponentModel;
using System.Windows.Media;

namespace MyToDoBoard.Data
{
	public class Label : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string id = string.Empty;
		private string name = string.Empty;
		private Color color = Colors.Red;

		public string Id
		{
			get => id;
			set
			{
				if (id != value)
				{
					id = value;
					PropertyChanged?.Invoke(this, new(nameof(Id)));
				}
			}
		}

		public string Name
		{
			get => name;
			set
			{
				if (name != value)
				{
					name = value;
					PropertyChanged?.Invoke(this, new(nameof(Name)));
				}
			}
		}

		public Color Color
		{
			get => color;
			set
			{
				if (color != value)
				{
					color = value;
					PropertyChanged?.Invoke(this, new(nameof(Color)));
					PropertyChanged?.Invoke(this, new(nameof(ColorBrush)));
				}
			}
		}

		#region TODO Move to ViewModel

		public Brush ColorBrush
		{
			get { return new SolidColorBrush(color); }
		}

		#endregion

	}
}
