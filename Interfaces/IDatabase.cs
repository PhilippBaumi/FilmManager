using System.Collections.ObjectModel;

namespace FilmManager.Interfaces
{
    public interface IDatabase
    {
        void DeleteTable(string tableName);
        void CreateTable(string tableName);
        void DeleteEntry(object entry, string tableName);
        void InsertEntry(object entry, string tableName);
        ObservableCollection<object> SelectAllEntries(string tableName);
    }
}
