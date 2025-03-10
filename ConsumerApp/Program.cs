using ConsumerApp;
using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Messenger.XmlHelpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Xsl;
using static Shared.BookingSharedDto;


var factory = new ConnectionFactory { HostName = QueueConfig.HostName };
var connection = await factory.CreateConnectionAsync( );
var channel = await connection.CreateChannelAsync( );

await channel.ExchangeDeclareAsync("booking_exchange",ExchangeType.Topic);
await channel.QueueDeclareAsync(queue: QueueConfig.QueueName,durable: false,exclusive: false,autoDelete: false,arguments: null);
await channel.QueueBindAsync("booking_queue","booking_exchange","booking.new");

Console.WriteLine("[*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray( );
    var message = Encoding.UTF8.GetString(body);

    var booking = JsonSerializer.Deserialize<BookingShared>(message)!;

    Console.WriteLine($"[x] Processando reserva.");

    var bookingProcessed = GenerateBookingHtml(booking!);

    SendEmail($"{booking.TravelerEmail}",$"Reserva {booking.BookingId} criada com sucesso!", bookingProcessed);

    Console.WriteLine($"Email enviado para {booking.TravelerEmail}");

    return Task.CompletedTask;
};
await channel.BasicConsumeAsync(queue: QueueConfig.QueueName, autoAck: true, consumer: consumer);

Console.WriteLine("press enter to exit");
Console.ReadLine( );

static string GenerateXslt()
{
    return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
    <xsl:template match=""/"">
        <html>
            <head>
                <title>Reserva XML</title>
                <style>
                    body {
                        margin: 0;
                        padding: 0;
                        -webkit-text-size-adjust: 100%;
                        -ms-text-size-adjust: 100%;
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        text-align: center;
                        }
                    .content {
                        width: 50%;
                        -webkit-text-size-adjust: 100%;
                        -ms-text-size-adjust: 100%;
                        background-color: #f9f9f9;
                        padding: 20px;
                        border: 1px solid #ddd;
                        border-radius: 10px;
                        text-align: left
                    }
                    .title {
                        font-size: 20px;
                        font-weight: bold;
                        margin-bottom: 10px;
                        color: #007BFF;
                    }
                    .item {
                        font-size: 16px;
                        margin: 8px 0;
                        padding: 10px;
                        border-bottom: 1px solid #ddd;
                    }
                    .footer {
                        margin-top: 20px;
                        font-style: italic;
                        color: #555;
                    }   
                </style>
            </head>
            <body>                
                <div class=""content"">
                    <div class=""title"">Detalhes da Reserva - <xsl:value-of select=""BookingShared/BookingId""/></div>
                
                    <div class=""item""><b>Voucher: </b> <xsl:value-of select=""BookingShared/BookingId""/></div>
                    <div class=""item""><b>Nome do Viajante: </b> <xsl:value-of select=""BookingShared/TravelerFullName""/></div>
                    <div class=""item""><b>Nome do Quarto: </b> <xsl:value-of select=""BookingShared/RoomName""/></div>
                    <div class=""item""><b>Tipo do Quarto: </b> <xsl:value-of select=""BookingShared/TypeRoom""/></div>
                    <div class=""item""><b>Check-in: </b> <xsl:value-of select=""BookingShared/CheckIn""/></div>
                    <div class=""item""><b>Check-out: </b> <xsl:value-of select=""BookingShared/CheckOut""/></div>
                    <div class=""item""><b>Total: </b> R$ <xsl:value-of select=""BookingShared/TotalPrice""/></div>
                    <div class=""item""><b>Status: </b> <xsl:value-of select=""BookingShared/Status""/></div>
                </div>
                <p class=""footer"">Se possível, por favor, imprima este e-mail para referência futura.</p>
            </body>
            </html>
    </xsl:template>
</xsl:stylesheet>
";
}
static string TransformXmlToHtml(string xmlContent,string xsltContent)
{
    var xmlDocument = new XmlDocument( );
    xmlDocument.LoadXml(xmlContent);

    var xslt = new XslCompiledTransform( );
    using var xsltReader = XmlReader.Create(new StringReader(xsltContent));
    xslt.Load(xsltReader);

    using var stringWriter = new StringWriter( );
    using var xmlWriter = XmlWriter.Create(stringWriter,xslt.OutputSettings);
    xslt.Transform(xmlDocument,null,xmlWriter);

    return stringWriter.ToString( );
}
static string GenerateBookingHtml(BookingShared booking)
{
    string xml = XmlHelper.GenerateXml(booking); 
    string xslt = GenerateXslt( );

    return TransformXmlToHtml(xml,xslt);
}
static void SendEmail(string toEmail,string subject,string htmlContent)
{
    var configSmtp = new ConfigSmtp();
    var smtpClient = new SmtpClient(configSmtp.smtpClient)
    {
        Port = 587,
        Credentials = new NetworkCredential(configSmtp.Email, configSmtp.PasswordApp),
        EnableSsl = true,
    };

    var mailMessage = new MailMessage
    {
        From = new MailAddress(configSmtp.Email, "Lorem Ipsum"),
        Subject = subject,
        Body = htmlContent,
        IsBodyHtml = true
    };

    mailMessage.To.Add(toEmail);
    smtpClient.Send(mailMessage);
}
