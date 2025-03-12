namespace Shared
{
    public class QueueConfiguation
    {
        public const string QueueName = "booking_queue";
        public const string ExchangeName = "hotel_exchange";
        public const string HostName = "localhost";

        public const string NewBookingRouting = "booking.new";
        public const string StatusUpdateRouting = "booking.status_update";
    }
}
