using System;
using System.ComponentModel;
using System.Windows;

namespace MyToDoBoard.DataModel
{
	public class Card : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler? PropertyChanged;

		private string title = string.Empty;
		private string? icon = null;
		private Label[]? labels = null;
		private DateTime? dueDate = null;
		private string? description = null;
		private ChecklistItem[]? checklist = null;
		private Link[]? links = null;
		private Card[]? relatedCards = null;
		private Comment[]? comments = null;

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

		public string? Icon
		{
			get => icon;
			set
			{
				if (icon != value)
				{
					icon = value;
					PropertyChanged?.Invoke(this, new(nameof(Icon)));
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

		public DateTime? DueDate
		{
			get => dueDate;
			set
			{
				if (dueDate != value)
				{
					dueDate = value;
					PropertyChanged?.Invoke(this, new(nameof(DueDate)));
				}
			}
		}

		public string? Description
		{
			get => description;
			set
			{
				if (description != value)
				{
					description = value;
					PropertyChanged?.Invoke(this, new(nameof(Description)));
				}
			}
		}

		public ChecklistItem[]? Checklists
		{
			get => checklist;
			set
			{
				if (checklist != value)
				{
					checklist = value;
					PropertyChanged?.Invoke(this, new(nameof(Checklists)));
				}
			}
		}

		public Link[]? Links
		{
			get => links;
			set
			{
				if (links != value)
				{
					links = value;
					PropertyChanged?.Invoke(this, new(nameof(Links)));
				}
			}
		}

		public Card[]? RelatedCards
		{
			get => relatedCards;
			set
			{
				if (relatedCards != value)
				{
					relatedCards = value;
					PropertyChanged?.Invoke(this, new(nameof(RelatedCards)));
				}
			}
		}

		public Comment[]? Comments
		{
			get => comments;
			set
			{
				if (comments != value)
				{
					comments = value;
					PropertyChanged?.Invoke(this, new(nameof(Comments)));
				}
			}
		}

		#region TODO Move to ViewModel

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

		#endregion

	}

}
