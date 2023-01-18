using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyToDoBoard.DataModel
{

	public class Card : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;
		private Label[]? labels = null;

		/*
			- Title & Icon
			- Labels
			- Due Date (for displaying purpose only)
			- Description
			- Checklist
			- List of Links
			- List of other Cards this one depends on
			- Comments
		 */

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

		public Label[]? Labels
		{
			get => labels;
			set
			{
				if (labels != value)
				{
					labels = value;
					PropertyChanged?.Invoke(this, new(nameof(Labels)));
					PropertyChanged?.Invoke(this, new(nameof(Margin)));
				}
			}
		}

		/// <summary>
		/// Visibility of this card changes to hidden when being dragged.
		/// This is not a Model attribute. It is a ViewModel attribute.
		/// </summary>
		public Visibility DraggingVisibility {
			get => draggingVisibility;
			set
			{
				if (draggingVisibility != value)
				{
					draggingVisibility = value;
					PropertyChanged?.Invoke(this, new(nameof(DraggingVisibility)));
				}
			}
		}
		private Visibility draggingVisibility = Visibility.Visible;

		public Thickness Margin
		{
			get
			{
				if (labels == null || labels.Length <= 0)
					return new Thickness(4);
				return new Thickness(4, 8, 4, 4);
			}
		}

	}

}
