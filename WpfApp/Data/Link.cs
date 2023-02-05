using System.ComponentModel;

namespace MyToDoBoard.Data
{
	public class Link : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;
		private string url = string.Empty;

		public string Title
		{
			get => title;
			set
			{
				if (title != value)
				{
					title = value;
					PropertyChanged?.Invoke(this, new(nameof(Title)));
				}
			}
		}

		public string Url
		{
			get => url;
			set
			{
				if (url != value)
				{
					url = value;
					PropertyChanged?.Invoke(this, new(nameof(Url)));
				}
			}
		}

	}
}
