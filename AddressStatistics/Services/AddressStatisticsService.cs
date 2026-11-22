using AddressStatistics.Models;

namespace AddressStatistics.Services;

/// <summary>
/// Подсчитывает дубли и этажность за один проход по записям.
/// </summary>
public class AddressStatisticsService
{
    private const int MaxReportedFloor = 5;

    public StatisticsReport Calculate(IEnumerable<AddressRecord> records)
    {
        Dictionary<AddressRecord, int> addressCounts = new();
        Dictionary<string, int[]> floorsByCity = new(StringComparer.Ordinal);

        foreach (AddressRecord record in records)
        {
            addressCounts.TryGetValue(record, out int count);
            addressCounts[record] = count + 1;

            if (!floorsByCity.TryGetValue(record.City, out int[]? floors))
            {
                floors = new int[MaxReportedFloor];
                floorsByCity.Add(record.City, floors);
            }

            // В отчёте по этажности учитывается каждая строка справочника.
            if (record.Floor >= 1 && record.Floor <= MaxReportedFloor)
            {
                floors[record.Floor - 1]++;
            }
        }

        Dictionary<AddressRecord, int> duplicates = new();
        foreach ((AddressRecord address, int count) in addressCounts)
        {
            if (count > 1)
            {
                duplicates.Add(address, count);
            }
        }

        return new StatisticsReport(duplicates, floorsByCity);
    }
}
