using System.Xml.Serialization;
using System.Xml;

namespace Nerd.Domain.Extensions;

public static class XmlExtensions
{
    public static string SerializeDictionaryToXml(this Dictionary<string, object>? dictionary)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary), "Dictionary can't be null.");
        }

        try
        {
            Controls dictionaryWrapper = new()
            {
                Items = new List<Control>()
            };

            foreach (var kvp in dictionary)
            {
                dictionaryWrapper.Items.Add(new Control
                {
                    Key = kvp.Key,
                    Value = kvp.Value
                });
            }

            Type type = typeof(Controls);
            XmlSerializer xmlSerializer = new(type);

            XmlWriterSettings writerOptions = new()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using StringWriter stringWriter = new();

            using var xmlWriter = XmlWriter.Create(stringWriter, writerOptions);

            xmlSerializer.Serialize(xmlWriter, dictionaryWrapper);

            return stringWriter.ToString();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Occured with serialization XML.", ex);
        }
    }

    public static Dictionary<string, object> DeserializeXmlToDictionary(this string xmlString)
    {
        if (string.IsNullOrEmpty(xmlString))
        {
            throw new ArgumentNullException(nameof(xmlString), "XML string can't be null or empty.");
        }

        try
        {
            Type type = typeof(Controls);

            XmlSerializer xmlSerializer = new(type);

            using StringReader stringReader = new(xmlString);
            using XmlReader xmlReader = XmlReader.Create(stringReader);

            Controls dictionaryWrapper = (Controls)xmlSerializer.Deserialize(xmlReader)!;

            Dictionary<string, object> dictionary = new();

            foreach (var item in dictionaryWrapper.Items)
            {
                dictionary.Add(item.Key, item.Value);
            }

            return dictionary;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred during XML deserialization.", ex);
        }
    }

    public class Controls
    {
        [XmlElement("Controls")]
        public required List<Control> Items { get; init; }
    }

    public class Control
    {
        [XmlElement("Key")]
        public required string Key { get; init; }

        [XmlElement("Value")]
        public required object Value { get; init; }
    }
}
