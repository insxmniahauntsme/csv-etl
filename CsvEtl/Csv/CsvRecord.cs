using CsvHelper.Configuration.Attributes;

namespace CsvEtl.Csv;

public sealed record CsvRecord
{
    [Name("tpep_pickup_datetime")]
    public DateTime TpepPickupDateTime { get; init; }

    [Name("tpep_dropoff_datetime")]
    public DateTime TpepDropoffDateTime { get; init; }

    [Name("passenger_count")]
    public int? PassengerCount { get; init; }

    [Name("trip_distance")]
    public decimal TripDistance { get; init; }

    [Name("store_and_fwd_flag")]
    public string StoreAndFwdFlag { get; init; } = string.Empty;

    [Name("PULocationID")]
    public int PuLocationId { get; init; }

    [Name("DOLocationID")]
    public int DoLocationId { get; init; }

    [Name("fare_amount")]
    public decimal FareAmount { get; init; }

    [Name("tip_amount")]
    public decimal TipAmount { get; init; }
}