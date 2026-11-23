using AddressStatistics.Models;

namespace AddressStatistics.UI;

/// <summary>
/// Выводит статистику и сообщения программы.
/// </summary>
public class ConsoleOutput
{
    public void PrintReport(StatisticsReport report, TimeSpan elapsed)
    {
        Console.WriteLine();
        PrintDuplicates(report.Duplicates);
        Console.WriteLine();
        PrintFloorsByCity(report.FloorsByCity);
        Console.WriteLine();
        Console.WriteLine($"Время обработки файла: {elapsed.TotalSeconds:F3} с.");
        Console.WriteLine();
    }

    private static void PrintDuplicates(IReadOnlyDictionary<AddressRecord, int> duplicates)
    {
        Console.WriteLine("Дублирующиеся записи:");
        if (duplicates.Count == 0)
        {
            Console.WriteLine("Дубли не найдены.");
            return;
        }

        foreach ((AddressRecord address, int count) in duplicates)
        {
            Console.WriteLine($"{address.City}, {address.Street}, дом {address.House}, этажей: {address.Floor} — повторений: {count}");
        }
    }

    private static void PrintFloorsByCity(IReadOnlyDictionary<string, int[]> floorsByCity)
    {
        Console.WriteLine("Количество зданий по этажности:");
        if (floorsByCity.Count == 0)
        {
            Console.WriteLine("В справочнике нет записей.");
            return;
        }

        foreach ((string city, int[] floors) in floorsByCity.OrderBy(item => item.Key, StringComparer.Ordinal))
        {
            Console.WriteLine($"{city}: 1 эт. — {floors[0]}, 2 эт. — {floors[1]}, 3 эт. — {floors[2]}, "
                + $"4 эт. — {floors[3]}, 5 эт. — {floors[4]}");
        }
    }
}
