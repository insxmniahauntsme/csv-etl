using CsvEtl.Csv;

namespace CsvEtl;

public class RecordsProcessor(CsvFileWriter<CsvRecord> csvHelper)
{
    public RecordProcessingResult Process(IEnumerable<CsvRecord> records)
    {
        var groups = records
            .GroupBy(r => new
            {
                r.TpepPickupDateTime,
                r.TpepDropoffDateTime,
                r.PassengerCount
            }).ToList();

        var uniqueRecords = groups
            .Select(g =>
            {
                var record = g.First();

                return record with
                {
                    TpepPickupDateTime = ConvertEasternToUtc(record.TpepPickupDateTime),
                    TpepDropoffDateTime = ConvertEasternToUtc(record.TpepDropoffDateTime),
                    StoreAndFwdFlag = record.StoreAndFwdFlag.Trim() == "Y" ? "Yes" : "No"
                };
            })
            .ToList();
        
        var duplicates = groups
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.Skip(1))
            .ToList();
        
        csvHelper.Write("duplicates.csv", duplicates);
        
        return new RecordProcessingResult(uniqueRecords, duplicates);
    }
    
    private static readonly TimeZoneInfo EasternTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    private static DateTime ConvertEasternToUtc(DateTime value)
    {
        var unspecified = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, EasternTimeZone);
    }
}

public sealed record RecordProcessingResult(IReadOnlyList<CsvRecord> UniqueRecords, IReadOnlyList<CsvRecord> Duplicates);