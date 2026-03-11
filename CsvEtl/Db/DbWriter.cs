using System.Data;
using CsvEtl.Csv;
using Microsoft.Data.SqlClient;

namespace CsvEtl.Db;

public class DbWriter(string connectionString)
{
    public async Task<int> InsertAsync(IReadOnlyList<CsvRecord> records)
    {
        if (records.Count == 0) return 0;

        var table = CreateDataTable(records);
        
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        using var sqlBulkCopy = new SqlBulkCopy(connection);
        
        sqlBulkCopy.DestinationTableName = "dbo.records";
        
        sqlBulkCopy.ColumnMappings.Add("tpep_pickup_datetime", "tpep_pickup_datetime");
        sqlBulkCopy.ColumnMappings.Add("tpep_dropoff_datetime", "tpep_dropoff_datetime");
        sqlBulkCopy.ColumnMappings.Add("passenger_count", "passenger_count");
        sqlBulkCopy.ColumnMappings.Add("trip_distance", "trip_distance");
        sqlBulkCopy.ColumnMappings.Add("store_and_fwd_flag", "store_and_fwd_flag");
        sqlBulkCopy.ColumnMappings.Add("PULocationID", "PULocationID");
        sqlBulkCopy.ColumnMappings.Add("DOLocationID", "DOLocationID");
        sqlBulkCopy.ColumnMappings.Add("fare_amount", "fare_amount");
        sqlBulkCopy.ColumnMappings.Add("tip_amount", "tip_amount");
        
        await sqlBulkCopy.WriteToServerAsync(table);
        
        return table.Rows.Count;
    }

    private static DataTable CreateDataTable(IReadOnlyList<CsvRecord> records)
    {
        var table = new DataTable();
        
        table.Columns.Add("tpep_pickup_datetime", typeof(DateTime));
        table.Columns.Add("tpep_dropoff_datetime", typeof(DateTime));
        table.Columns.Add("passenger_count", typeof(int));
        table.Columns.Add("trip_distance", typeof(decimal));
        table.Columns.Add("store_and_fwd_flag", typeof(string));
        table.Columns.Add("PULocationID", typeof(int));
        table.Columns.Add("DOLocationID", typeof(int));
        table.Columns.Add("fare_amount", typeof(decimal));
        table.Columns.Add("tip_amount", typeof(decimal));

        foreach (var record in records)
        {
            table.Rows.Add(
                record.TpepPickupDateTime,
                record.TpepDropoffDateTime,
                record.PassengerCount.HasValue,
                record.TripDistance,
                record.StoreAndFwdFlag,
                record.PuLocationId,
                record.DoLocationId,
                record.FareAmount,
                record.TipAmount
            );
        }
        
        return table;
    }
}