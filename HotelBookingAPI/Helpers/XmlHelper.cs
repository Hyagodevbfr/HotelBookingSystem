using HotelBookingAPI.Dtos;
using Shared;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using static Shared.BookingSharedDto;

namespace HotelBookingAPI.Messenger.XmlHelpers;

public class XmlHelper
{
    public static string GenerateXml(BookingShared booking)
    {
        var serializer = new XmlSerializer(typeof(BookingShared));

        using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, booking);
        return stringWriter.ToString();
    }
}
