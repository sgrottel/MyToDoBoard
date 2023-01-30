using System.ComponentModel;

namespace MyToDoBoard.DataModel
{
	public class ChecklistItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;
		private bool isChecked = false;

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

		public bool IsChecked
		{
			get => isChecked;
			set
			{
				if (isChecked != value)
				{
					isChecked = value;
					PropertyChanged?.Invoke(this, new(nameof(IsChecked)));
				}
			}
		}

	}
}
