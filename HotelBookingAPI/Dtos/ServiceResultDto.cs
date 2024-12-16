namespace HotelBookingAPI.Dtos;

public class ServiceResultDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; } = [];

    public static ServiceResultDto<T> Fail(string message,IEnumerable<string>? errors = null)
    {
        return new ServiceResultDto<T>
        {
            Data = default,
            Success = false,
            Message = message,
            Errors = errors ?? [message]
        };
    }
    public static ServiceResultDto<T> SuccessResult(T? data, string message)
    {
        return new ServiceResultDto<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }
    public static ServiceResultDto<T> NullContent(string message)
    {
        return new ServiceResultDto<T>
        {
            Success = false,
            Message = message,
        };
    }
}
