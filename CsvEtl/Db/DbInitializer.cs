using Dapper;
using Microsoft.Data.SqlClient;

namespace CsvEtl.Db;

public sealed class DbInitializer
{
    private readonly string _masterConnectionString;
    private readonly string _dbConnectionString;

    public DbInitializer(string masterConnectionString, string dbConnectionString)
    {
        _masterConnectionString = masterConnectionString;
        _dbConnectionString = dbConnectionString;
    }

    public async Task InitializeAsync()
    {
        const string createDatabaseSql = """
            IF DB_ID('csv_etl') IS NULL
            BEGIN
                CREATE DATABASE csv_etl;
            END;
            """;

        await using (var masterConnection = new SqlConnection(_masterConnectionString))
        {
            await masterConnection.ExecuteAsync(createDatabaseSql);
        }

        const string createTableSql = """
            IF OBJECT_ID('dbo.records', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.records
                (
                    id BIGINT IDENTITY(1,1) PRIMARY KEY,
                    tpep_pickup_datetime DATETIME2 NOT NULL,
                    tpep_dropoff_datetime DATETIME2 NOT NULL,
                    passenger_count INT NULL,
                    trip_distance DECIMAL(10,2) NOT NULL,
                    store_and_fwd_flag NVARCHAR(3) NOT NULL,
                    PULocationID INT NOT NULL,
                    DOLocationID INT NOT NULL,
                    fare_amount DECIMAL(10,2) NOT NULL,
                    tip_amount DECIMAL(10,2) NOT NULL,

                    travel_time_seconds 
                        AS DATEDIFF(SECOND, tpep_pickup_datetime, tpep_dropoff_datetime) PERSISTED
                );

                CREATE NONCLUSTERED INDEX IX_records_PULocationID
                    ON dbo.records (PULocationID);

                CREATE NONCLUSTERED INDEX IX_records_PULocationID_tip_amount
                    ON dbo.records (PULocationID) INCLUDE (tip_amount);

                CREATE NONCLUSTERED INDEX IX_records_trip_distance
                    ON dbo.records (trip_distance DESC);

                CREATE NONCLUSTERED INDEX IX_records_travel_time_seconds
                    ON dbo.records (travel_time_seconds DESC);
            END;
            """;

        await using (var dbConnection = new SqlConnection(_dbConnectionString))
        {
            await dbConnection.ExecuteAsync(createTableSql);
        }
    }
}