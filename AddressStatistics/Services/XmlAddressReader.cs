using System.Xml;
using AddressStatistics.Models;

namespace AddressStatistics.Services;

/// <summary>
/// Читает адреса из элементов item внутри root.
/// </summary>
public class XmlAddressReader : IAddressReader
{
    public IEnumerable<AddressRecord> Read(string path)
    {
        XmlReaderSettings settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            IgnoreComments = true,
            IgnoreWhitespace = true
        };

        using FileStream stream = File.OpenRead(path);
        using XmlReader reader = XmlReader.Create(stream, settings);
        if (reader.MoveToContent() != XmlNodeType.Element || reader.Name != "root" || reader.NamespaceURI != "")
        {
            throw new FormatException("В XML ожидается корневой элемент root.");
        }

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Depth != 1 || reader.Name != "item" || reader.NamespaceURI != "")
                {
                    throw new FormatException($"XML, строка {GetLineNumber(reader)}: ожидается элемент item внутри root.");
                }

                yield return ParseRecord(reader);
            }
            else if (reader.NodeType is XmlNodeType.Text or XmlNodeType.CDATA)
            {
                throw new FormatException($"XML, строка {GetLineNumber(reader)}: данные адреса должны быть в атрибутах item.");
            }
        }
    }

    private static AddressRecord ParseRecord(XmlReader reader)
    {
        try
        {
            return AddressRecord.Parse(reader.GetAttribute("city"), reader.GetAttribute("street"),
                reader.GetAttribute("house"), reader.GetAttribute("floor"));
        }
        catch (FormatException ex)
        {
            throw new FormatException($"XML, строка {GetLineNumber(reader)}: {ex.Message}", ex);
        }
    }

    private static int GetLineNumber(XmlReader reader)
    {
        return ((IXmlLineInfo)reader).LineNumber;
    }
}
