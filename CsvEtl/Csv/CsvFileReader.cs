using System.Globalization;
using CsvHelper;

namespace CsvEtl.Csv;

public class CsvFileReader<T>
{
    public IEnumerable<T> Read(string filePath)
    {
        try
        {
            using var streamReader = new StreamReader(filePath);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            return csvReader.GetRecords<T>().ToList();
        }
        catch (CsvHelperException ex)
        {
            throw new InvalidOperationException("Failed to parse CSV file.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Failed to read CSV file.", ex);
        }
    }
}