using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FilmManager.Helpers
{
    public class ListToObservableCollection
    {
        public static ObservableCollection<object> GetObservableCollection(List<object>list)
        {
            ObservableCollection<object> collection = new();
            foreach (object item in list)
            {
                collection.Add(item);
            }
            return collection;
        }
    }
}
