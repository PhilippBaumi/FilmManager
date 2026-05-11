namespace FilmManager.Interfaces
{
    public interface ILoadFile
    {
        void LoadFromCSV();
        void LoadFromDOCX();
        void LoadFromJSON();
    }
}
