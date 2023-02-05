using MyToDoBoard.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDoBoard.ViewModel
{
	internal class BoardView : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler? PropertyChanged;

		private Data.Board? data = null;
		public Data.Board? Data
		{
			get => data; set
			{
				if (data != value)
				{
					data = value;
					PropertyChanged?.Invoke(this, new(nameof(Data)));
					PropertyChanged?.Invoke(this, new(nameof(Columns)));
					PropertyChanged?.Invoke(this, new(nameof(Labels)));
				}
			}
		}

		public Column[]? Columns { get => data?.Columns; }

		public Label[]? Labels { get => data?.Labels; }

	}
}
