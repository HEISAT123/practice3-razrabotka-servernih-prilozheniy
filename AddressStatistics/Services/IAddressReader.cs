using AddressStatistics.Models;

namespace AddressStatistics.Services;

/// <summary>
/// Читает записи справочника по одной.
/// </summary>
public interface IAddressReader
{
    IEnumerable<AddressRecord> Read(string path);
}
