using MyToDoBoard.DataModel;
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

		private DataModel.Board? data = null;
		public DataModel.Board? Data
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

		public ColumnCollection? Columns { get => data?.Columns; }

		public LabelCollection? Labels { get => data?.Labels; }

	}
}
