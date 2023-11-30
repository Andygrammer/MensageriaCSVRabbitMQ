namespace Produtor.Handlers
{
    public class CsvModel<T>
    {
        public string FileName { get; set; }

        public string FolderName { get; set; }

        public string DirName { get; set; }

        public List<T> DataToWrite { get; set; }

        public int BatchSize { get; set; }

        public int BufferSize { get; set; }
    }
}
