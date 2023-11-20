using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace RanseiLink.Core.Util;

public class XmlUtilException : Exception
{
    public XmlUtilException(string message) : base(message) { }
}

public static class XmlUtil
{
    public static XAttribute AttributeRequired(this XElement element, XName name)
    {
        var a = element.Attribute(name);
        if (a == null)
        {
            ThrowXmlUtilException($"Element '{element.Name}' missing required attribute '{name}'");
        }
        return a;
    }

    public static bool AttributeBool(this XElement element, XName name)
    {
        var a = element.AttributeRequired(name);
        if (!bool.TryParse(a.Value, out var result))
        {
            ThrowXmlUtilException($"Element '{element.Name}' attribute '{name}' value must be valid boolean, but was '{a.Value}'");
        }
        return result;
    }

    public static bool AttributeBool(this XElement element, XName name, bool defaultValue)
    {
        var a = element.Attribute(name);
        if (a == null)
        {
            return defaultValue;
        }
        if (!bool.TryParse(a.Value, out var result))
        {
            return defaultValue;
        }
        return result;
    }

    public static int AttributeInt(this XElement element, XName name)
    {
        var a = element.AttributeRequired(name);
        if (!int.TryParse(a.Value, out var result))
        {
            ThrowXmlUtilException($"Element '{element.Name}' attribute '{name}' value must be valid integer number, but was '{a.Value}'");
        }
        return result;
    }

    public static int AttributeInt(this XElement element, XName name, int defaultValue)
    {
        var a = element.Attribute(name);
        if (a == null)
        {
            return defaultValue;
        }
        if (!int.TryParse(a.Value, out var result))
        {
            return defaultValue;
        }
        return result;
    }

    public static string AttributeStringNonEmpty(this XElement element, XName name)
    {
        var a = element.AttributeRequired(name);
        if (string.IsNullOrEmpty(a.Value))
        {
            ThrowXmlUtilException($"Element '{element.Name}' attribute {name} value must be non-empty");
        }
        return a.Value;
    }

    public static XElement ElementRequired(this XContainer container, XName name)
    {
        var e = container.Element(name);
        if (e == null)
        {
            if (container is XDocument)
            {
                ThrowXmlUtilException($"Document missing required root element '{name}'");
            }
            else if (container is XElement owner)
            {
                ThrowXmlUtilException($"Element '{owner.Name}' missing required element '{name}'");
            }
            else
            {
                ThrowXmlUtilException($"Container missing required element '{name}'");
            }
        }
        return e;
    }

    [DoesNotReturn]
    private static void ThrowXmlUtilException(string message)
    {
        throw new XmlUtilException(message);
    }
}
