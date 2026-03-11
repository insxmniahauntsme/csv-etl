using CsvEtl.Csv;
using CsvEtl.Db;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CsvEtl;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a path to the CSV file.");
            return;
        }

        if (!File.Exists(args[0]) && string.IsNullOrWhiteSpace(args[0]))
        {
            Console.WriteLine("File does not exist.");
            return;
        }

        var filePath = args[0];

        var csvReader = new CsvFileReader<CsvRecord>();
        var records = csvReader.Read(filePath).ToList();

        var processor = new RecordsProcessor(new CsvFileWriter<CsvRecord>());

        var processingResult = processor.Process(records);

        var masterConnection = configuration.GetConnectionString("Master");
        var csvEtlConnection = configuration.GetConnectionString("CsvEtl");
        
        var dbInitializer = new DbInitializer(masterConnection!, csvEtlConnection!);
        await dbInitializer.InitializeAsync();
        var dbWriter = new DbWriter(configuration.GetConnectionString("CsvEtl")!);

        var rowsInserted = await dbWriter.InsertAsync(processingResult.UniqueRecords);
        
        Console.WriteLine($"Rows parsed: {records.Count}");
        Console.WriteLine($"Rows inserted: {rowsInserted}");
        Console.WriteLine($"Unique records: {processingResult.UniqueRecords.Count}");
        Console.WriteLine($"Duplicates: {processingResult.Duplicates.Count}");
    }
}