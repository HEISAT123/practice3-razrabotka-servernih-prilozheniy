using System.Globalization;

namespace AddressStatistics.Models;

/// <summary>
/// Запись справочника. Для сравнения используются все четыре поля.
/// </summary>
public readonly record struct AddressRecord(string City, string Street, int House, int Floor)
{
    internal static AddressRecord Parse(string? city, string? street, string? house, string? floor)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street))
        {
            throw new FormatException("Город и улица должны быть указаны.");
        }
        if (!int.TryParse(house, NumberStyles.Integer, CultureInfo.InvariantCulture, out int houseNumber)
            || houseNumber <= 0)
        {
            throw new FormatException("Номер дома должен быть положительным целым числом.");
        }
        if (!int.TryParse(floor, NumberStyles.Integer, CultureInfo.InvariantCulture, out int floorCount)
            || floorCount <= 0)
        {
            throw new FormatException("Этажность должна быть положительным целым числом.");
        }

        return new AddressRecord(city.Trim(), street.Trim(), houseNumber, floorCount);
    }
}
