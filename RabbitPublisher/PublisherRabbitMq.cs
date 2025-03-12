using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using static Shared.BookingSharedDto;
using static Shared.QueueConfiguation;


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
        try
        {
            Console.WriteLine($"[x] Waiting message to sent");

            var bookingJson = JsonSerializer.Serialize(booking);
            var body = Encoding.UTF8.GetBytes(bookingJson);

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: NewBookingRouting,
                body: body
                );
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Erro ao processar envio de email: {ex.Message}");
        }
        finally
        {
            if(channel != null && channel.IsOpen)
                await channel.CloseAsync( );
        }
    }

    public async Task StatusUpdate(BookingShared booking)
    {
        var factory = new ConnectionFactory { HostName = _hostName };

        using var connection = await factory.CreateConnectionAsync( );
        using var channel = await connection.CreateChannelAsync( );

        await channel.ExchangeDeclareAsync("booking_exchange",ExchangeType.Topic);
        try
        {
            Console.WriteLine($"[x] Waiting for status update message to send");

            var bookingJson = JsonSerializer.Serialize(booking);
            var body = Encoding.UTF8.GetBytes(bookingJson);

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: StatusUpdateRouting,
                body: body
                );
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Erro ao processar envio de email: {ex.Message}");
        }
        finally
        {
            if(channel != null && channel.IsOpen)
                await channel.CloseAsync( );
        }
    }
}
