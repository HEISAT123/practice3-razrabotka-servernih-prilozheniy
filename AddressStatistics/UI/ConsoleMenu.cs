using System.Diagnostics;
using System.Text;
using System.Xml;
using AddressStatistics.Models;
using AddressStatistics.Services;

namespace AddressStatistics.UI;

/// <summary>
/// Принимает путь к справочнику и запускает подсчёт статистики.
/// </summary>
public class ConsoleMenu
{
    private readonly IAddressReader _csvReader;
    private readonly IAddressReader _xmlReader;
    private readonly AddressStatisticsService _service;
    private readonly ConsoleOutput _output;

    public ConsoleMenu(IAddressReader csvReader, IAddressReader xmlReader,
        AddressStatisticsService service, ConsoleOutput output)
    {
        _csvReader = csvReader;
        _xmlReader = xmlReader;
        _service = service;
        _output = output;
    }

    public void Run()
    {
        Console.WriteLine("===== Статистика адресов =====");

        while (true)
        {
            Console.Write("Введите путь к CSV или XML (0 — выход): ");
            string? input = Console.ReadLine()?.Trim();
            if (input == null || input == "0")
            {
                break;
            }

            string path = RemoveOuterQuotes(input);
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Путь к файлу не может быть пустым.");
                continue;
            }

            TryProcessFile(path);
        }

        Console.WriteLine("Работа завершена.");
    }

    private void TryProcessFile(string path)
    {
        try
        {
            ProcessFile(path);
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            Console.WriteLine("Файл не найден. Проверьте указанный путь.");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Нет доступа к файлу.");
        }
        catch (DecoderFallbackException)
        {
            Console.WriteLine("Не удалось прочитать текст файла. Проверьте кодировку.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (XmlException ex)
        {
            string position = ex.LineNumber > 0 ? $" Строка {ex.LineNumber}, позиция {ex.LinePosition}." : "";
            Console.WriteLine($"Неверный формат XML.{position}");
        }
        catch (IOException)
        {
            Console.WriteLine("Не удалось прочитать файл. Проверьте, доступен ли он.");
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            Console.WriteLine("Некорректный путь к файлу.");
        }
    }

    private void ProcessFile(string path)
    {
        IAddressReader reader = GetReader(path);
        Stopwatch timer = Stopwatch.StartNew();
        StatisticsReport report = _service.Calculate(reader.Read(path));
        timer.Stop();

        _output.PrintReport(report, timer.Elapsed);
    }

    private IAddressReader GetReader(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".csv" => _csvReader,
            ".xml" => _xmlReader,
            _ => throw new FormatException("Выберите файл с расширением .csv или .xml.")
        };
    }

    private static string RemoveOuterQuotes(string path)
    {
        if (path.Length >= 2 && path[0] == '"' && path[^1] == '"')
        {
            return path[1..^1];
        }

        return path;
    }
}
