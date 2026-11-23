using AddressStatistics.Services;
using AddressStatistics.UI;

CsvAddressReader csvReader = new CsvAddressReader();
XmlAddressReader xmlReader = new XmlAddressReader();
AddressStatisticsService service = new AddressStatisticsService();
ConsoleOutput output = new ConsoleOutput();
ConsoleMenu menu = new ConsoleMenu(csvReader, xmlReader, service, output);

menu.Run();
