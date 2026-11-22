using System.Text;
using AddressStatistics.Models;

namespace AddressStatistics.Services;

/// <summary>
/// Читает CSV с разделителем «;» и поддержкой полей в кавычках.
/// </summary>
public class CsvAddressReader : IAddressReader
{
    private static readonly string[] Header = { "city", "street", "house", "floor" };

    public IEnumerable<AddressRecord> Read(string path)
    {
        using StreamReader reader = new StreamReader(path, new UTF8Encoding(false, true));
        bool headerRead = false;
        long lineNumber = 0;
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            long recordLineNumber = lineNumber;
            string[] fields = ReadFields(reader, line, ref lineNumber);
            if (!headerRead)
            {
                if (!fields.SequenceEqual(Header, StringComparer.OrdinalIgnoreCase))
                {
                    throw new FormatException("В заголовке CSV ожидаются столбцы city, street, house и floor.");
                }

                headerRead = true;
                continue;
            }

            yield return ParseRecord(fields, recordLineNumber);
        }

        if (!headerRead)
        {
            throw new FormatException("В CSV отсутствует строка с заголовками столбцов.");
        }
    }

    private static string[] ReadFields(StreamReader reader, string line, ref long lineNumber)
    {
        string[] fields = new string[Header.Length];
        int position = 0;
        long recordLineNumber = lineNumber;

        for (int i = 0; i < fields.Length; i++)
        {
            if (i > 0)
            {
                if (position == line.Length || line[position] != ';')
                {
                    throw new FormatException($"CSV, строка {recordLineNumber}: ожидаются четыре поля.");
                }

                position++;
            }

            SkipSpaces(line, ref position);
            fields[i] = position < line.Length && line[position] == '"'
                ? ReadQuotedField(reader, ref line, ref position, ref lineNumber)
                : ReadPlainField(line, ref position, lineNumber);
        }

        if (position != line.Length)
        {
            throw new FormatException($"CSV, строка {recordLineNumber}: ожидаются четыре поля.");
        }

        return fields;
    }

    private static string ReadPlainField(string line, ref int position, long lineNumber)
    {
        int separator = line.IndexOf(';', position);
        int end = separator >= 0 ? separator : line.Length;
        ReadOnlySpan<char> field = line.AsSpan(position, end - position).TrimEnd();
        if (field.Contains('"'))
        {
            throw new FormatException($"CSV, строка {lineNumber}: неверный формат полей.");
        }

        position = end;
        return field.ToString();
    }

    private static string ReadQuotedField(StreamReader reader, ref string line, ref int position, ref long lineNumber)
    {
        position++;
        long firstLine = lineNumber;
        StringBuilder? value = null;

        while (true)
        {
            int quote = line.IndexOf('"', position);
            if (quote < 0)
            {
                string? nextLine = reader.ReadLine();
                if (nextLine == null)
                {
                    throw new FormatException($"CSV, строка {firstLine}: неверный формат полей.");
                }

                value ??= new StringBuilder();
                value.Append(line.AsSpan(position)).Append('\n');
                line = nextLine;
                lineNumber++;
                position = 0;
                continue;
            }

            ReadOnlySpan<char> part = line.AsSpan(position, quote - position);
            position = quote + 1;
            if (position < line.Length && line[position] == '"')
            {
                value ??= new StringBuilder();
                value.Append(part).Append('"');
                position++;
                continue;
            }

            SkipSpaces(line, ref position);
            if (position < line.Length && line[position] != ';')
            {
                throw new FormatException($"CSV, строка {lineNumber}: неверный формат полей.");
            }

            return value == null ? part.ToString() : value.Append(part).ToString();
        }
    }

    private static void SkipSpaces(string line, ref int position)
    {
        while (position < line.Length && char.IsWhiteSpace(line[position]))
        {
            position++;
        }
    }

    private static AddressRecord ParseRecord(string[] fields, long lineNumber)
    {
        try
        {
            return AddressRecord.Parse(fields[0], fields[1], fields[2], fields[3]);
        }
        catch (FormatException ex)
        {
            throw new FormatException($"CSV, строка {lineNumber}: {ex.Message}", ex);
        }
    }
}
