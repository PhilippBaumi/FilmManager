namespace FilmManager.Interfaces
{
    public interface ILoadFile
    {
        void LoadFromCSV();
        void LoadFromPDF();
        void LoadFromDOCX();
        void LoadFromJSON();
    }
}
