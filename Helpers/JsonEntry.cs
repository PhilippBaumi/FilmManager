namespace FilmManager.Helpers
{
    public class JsonEntry<T>
    {
        public string Type { get; set; }
        public T Data { get; set; }
    }
}
