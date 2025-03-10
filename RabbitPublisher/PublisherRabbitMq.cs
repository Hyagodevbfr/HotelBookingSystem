using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using static Shared.BookingSharedDto;

namespace RabbitPublisher;

public class PublisherRabbitMq
{
    private readonly string _hostName;

    public PublisherRabbitMq(string hostName = "localhost")
    {
        _hostName = hostName;
    }

    public async Task SendBookingAsync(BookingShared booking)
    {
        var factory = new ConnectionFactory { HostName = _hostName };

        using var connection = await factory.CreateConnectionAsync( );
        using var channel = await connection.CreateChannelAsync( );

        await channel.ExchangeDeclareAsync("booking_exchange",ExchangeType.Topic);

        Console.WriteLine($"[x] Waiting message to sent");

        var bookingJson = JsonSerializer.Serialize(booking);
        var body = Encoding.UTF8.GetBytes(bookingJson);

        await channel.BasicPublishAsync(
            exchange: "booking_exchange",
            routingKey: "booking.new",
            body: body
            );
    }
}
