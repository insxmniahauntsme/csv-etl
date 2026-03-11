# CSV ETL CLI Tool

A simple CLI ETL application written in **C#** that processes data from a CSV file and loads it into **Microsoft SQL Server**.

The application performs data cleaning, deduplication, transformation and efficient bulk insertion into the database.

---
## Running the application

Run the program with the CSV file path as an argument:
```
    cd CsvEtl
    dotnet run -- sample-cab-data.csv
```

---

## Database schema

The following columns are stored:

- `tpep_pickup_datetime`
- `tpep_dropoff_datetime`
- `passenger_count`
- `trip_distance`
- `store_and_fwd_flag`
- `PULocationID`
- `DOLocationID`
- `fare_amount`
- `tip_amount`

A computed column is added:

- `travel_time_seconds = DATEDIFF(SECOND, pickup_datetime, dropoff_datetime)`

## Number of rows after processing

For the provided dataset:
```
    Input rows: 30001
    Unique rows inserted: 29889
    Duplicates removed: 112
```

---

## Handling Large Input Files

If the application had to process very large CSV files (e.g., 10GB), 
I would avoid loading the entire dataset into memory. Instead, I would process the file in a streaming manner 
using `CsvHelper`'s lazy enumeration.
Deduplication would be performed using a `HashSet` of composite keys while processing records sequentially.
Records would be accumulated in small batches and inserted into the database using `SqlBulkCopy` periodically,
rather than building a full `DataTable` in memory. This approach would significantly reduce memory usage and allow
the application to handle much larger datasets efficiently.

---

## Comments

- Duplicate records are identified by the combination of `tpep_pickup_datetime`, `tpep_dropoff_datetime`, and `passenger_count`, as required by the task.
- Input timestamps are treated as Eastern Time and converted to UTC before insertion into the database.
- `store_and_fwd_flag` values are normalized from `Y/N` to `Yes/No`.
- The solution is optimized for the provided dataset size; for significantly larger files, a streaming pipeline and batched processing would be preferable.
- SQL Server is expected to be accessible through the connection strings defined in `appsettings.json`.