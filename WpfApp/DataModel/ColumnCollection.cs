using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDoBoard.DataModel
{
	public class ColumnCollection : Collection<Column>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		public event NotifyCollectionChangedEventHandler? CollectionChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		protected override void ClearItems()
		{
			base.ClearItems();
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		protected override void InsertItem(int index, Column item)
		{
			base.InsertItem(index, item);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		protected override void RemoveItem(int index)
		{
			Column old = Items[index];
			base.RemoveItem(index);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old, index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		protected override void SetItem(int index, Column item)
		{
			Column old = Items[index];
			base.SetItem(index, item);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, old, index));
		}
	}
}
