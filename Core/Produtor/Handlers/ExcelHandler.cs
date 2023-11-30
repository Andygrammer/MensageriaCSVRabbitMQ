using System.Text;

namespace Produtor.Handlers
{
    public static class ExcelHandler
    {
        public static async Task<MemoryStream> WriteCsv<T>(CsvModel<T> csvModel)
        {
            var memory = new MemoryStream();
            var fileStream = new FileStream(Path.Combine(csvModel.DirName, csvModel.FolderName, csvModel.FileName), FileMode.Create, FileAccess.Write);

            int batchSize = csvModel.BatchSize;
            int batchTotal = (int)Math.Ceiling((double)csvModel.DataToWrite.Count / batchSize);
            int dataIndex = 0;

            using(var csvWriter = new StreamWriter(fileStream, Encoding.UTF8, bufferSize: csvModel.BufferSize))
            {
                for (int batchIndex = 0; batchIndex < batchTotal; batchIndex++)
                {
                    int initialBatchIndex = batchIndex * batchSize;
                    int finalBatchIndex = Math.Min(initialBatchIndex + batchSize, csvModel.DataToWrite.Count);

                    for (int i = initialBatchIndex; i < finalBatchIndex; i++)
                    {
                        var dataToWrite = csvModel.DataToWrite[dataIndex];
                        csvWriter.WriteLine(dataToWrite);
                        dataIndex++;
                    }
                }
            }

            using (var streamFs = new FileStream(
                Path.Combine(csvModel.DirName, csvModel.FolderName, csvModel.FileName),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None,
                bufferSize: csvModel.BufferSize,
                useAsync: true))
            {
                await streamFs.CopyToAsync(memory);
            }
            memory.Position = 0;
            return memory;
        }
    }
}
