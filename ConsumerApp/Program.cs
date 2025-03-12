using ConsumerApp;
using ConsumerApp.Email_Templates;
using HotelBookingAPI.Messenger.XmlHelpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Xsl;
using static Shared.BookingSharedDto;
using static Shared.QueueConfiguation;


var factory = new ConnectionFactory { HostName = HostName };
var connection = await factory.CreateConnectionAsync( );
var channel = await connection.CreateChannelAsync( );

await channel.ExchangeDeclareAsync(ExchangeName,ExchangeType.Topic);

await channel.QueueDeclareAsync(queue: QueueName,durable: false,exclusive: false,autoDelete: false,arguments: null);

await channel.QueueBindAsync(QueueName, ExchangeName, NewBookingRouting);
await channel.QueueBindAsync(QueueName, ExchangeName, StatusUpdateRouting);

Console.WriteLine("[*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray( );
    var message = Encoding.UTF8.GetString(body);
    var booking = JsonSerializer.Deserialize<BookingShared>(message)!;

    string emailSubject;
    string emailContent;

    if(ea.RoutingKey == NewBookingRouting)
    {
        emailSubject = $"Reserva {booking.BookingId} criada com sucesso!";
        emailContent = GenerateBookingHtml(booking!);
    }
    else if(ea.RoutingKey == StatusUpdateRouting)
    {
        emailSubject = $"Status atualizado - Voucher: {booking.BookingId}";
        emailContent = GenerateStatusUpdateHtml(booking!);
    }
    else
    {
        emailSubject = "Assunto desconhecido";
        emailContent = "Não foi possível gerar o conteúdo do email.";
    }

    SendEmail(booking.TravelerEmail,emailSubject,emailContent);

    Console.WriteLine($"Email enviado para {booking.TravelerEmail}");

    return Task.CompletedTask;
};
await channel.BasicConsumeAsync(queue: QueueName, autoAck: true, consumer: consumer);

Console.WriteLine("press enter to exit");
Console.ReadLine( );
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
    var generate = new NewBooking( );
    string xml = XmlHelper.GenerateXml(booking);
    string xslt = generate.NewBookingXlst();

    return TransformXmlToHtml(xml,xslt);
}

static string GenerateStatusUpdateHtml(BookingShared booking)
{
    var generate = new StatusUpdate( );
    string xml = XmlHelper.GenerateXml(booking);
    string xslt = generate.GenerateXsltStatusUpdate();

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
