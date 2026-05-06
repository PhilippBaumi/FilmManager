namespace FilmManager.Interfaces
{
    public interface IWriteFile
    {
        void WriteToPDF();
        void WriteToCSV();
        void WriteToDOCX();

        void WriteToJSON();

    }
}
