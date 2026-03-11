using System.Globalization;
using CsvHelper;

namespace CsvEtl.Csv;

public class CsvFileWriter<T>
{
    public void Write(string filePath, IEnumerable<T> records)
    {
        using var streamWriter = new StreamWriter(filePath);
        using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
        
        csvWriter.WriteRecords(records);
    }
}