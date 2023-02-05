using System;
using System.ComponentModel;

namespace MyToDoBoard.Data
{
	public class Comment : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string text = string.Empty;
		private DateTime? date = null;

		public string Text
		{
			get => text;
			set
			{
				if (text != value)
				{
					text = value;
					PropertyChanged?.Invoke(this, new(nameof(Text)));
				}
			}
		}

		public DateTime? Date
		{
			get => date;
			set
			{
				if (date != value)
				{
					date = value;
					PropertyChanged?.Invoke(this, new(nameof(Date)));
				}
			}
		}

	}
}
