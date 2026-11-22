namespace AddressStatistics.Models;

/// <summary>
/// Дубли и количество зданий каждой этажности по городам.
/// </summary>
public class StatisticsReport
{
    public IReadOnlyDictionary<AddressRecord, int> Duplicates { get; }
    public IReadOnlyDictionary<string, int[]> FloorsByCity { get; }

    public StatisticsReport(IReadOnlyDictionary<AddressRecord, int> duplicates,
        IReadOnlyDictionary<string, int[]> floorsByCity)
    {
        Duplicates = duplicates;
        FloorsByCity = floorsByCity;
    }
}
