
namespace Common.Responses.Wrappers
{
    public class ResponseWrapper : IResponseWrapper
    {
        public ResponseWrapper() { }
        public List<string> Message { get; set; } = new();
        public bool IsSuccessful { get; set; }

        public static IResponseWrapper Fail()
        {
            return new ResponseWrapper { IsSuccessful = false };
        }

        public static Task<IResponseWrapper> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static IResponseWrapper Fail(string message)
        {
            return new ResponseWrapper { IsSuccessful = false, Message = [message] };
        }

        public static Task<IResponseWrapper> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static IResponseWrapper Fail(List<string> message)
        {
            return new ResponseWrapper { IsSuccessful = false, Message = message };
        }

        public static Task<IResponseWrapper> FailAsync(List<string> message)
        {
            return Task.FromResult(Fail(message));
        }

        public static IResponseWrapper Success()
        {
            return new ResponseWrapper { IsSuccessful = true };
        }

        public static Task<IResponseWrapper> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static IResponseWrapper Success(string message)
        {
            return new ResponseWrapper
            {
                IsSuccessful = true,
                Message = [message]
            };
        }

        public static Task<IResponseWrapper> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }
    }

    public class ResponseWrapper<T> : ResponseWrapper, IResponseWrapper<T>
    {
        public ResponseWrapper()
        {

        }
        public T Data { get; set; }

        public new static ResponseWrapper<T> Fail()
        {
            return new ResponseWrapper<T>() { IsSuccessful = false };
        }

        public new static Task<ResponseWrapper<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public new static ResponseWrapper<T> Fail(string message)
        {
            return new ResponseWrapper<T>() { IsSuccessful = false, Message = [message] };
        }

        public new static Task<ResponseWrapper<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static ResponseWrapper<T> Fail(List<string> message)
        {
            return new ResponseWrapper<T>() { IsSuccessful = false, Message = message };
        }

        public new static Task<ResponseWrapper<T>> FailAsync(List<string> message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static ResponseWrapper<T> Success()
        {
            return new ResponseWrapper<T>() { IsSuccessful = true };
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public new static ResponseWrapper<T> Success(string message)
        {
            return new ResponseWrapper<T>() { IsSuccessful = true, Message = [message] };
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public new static ResponseWrapper<T> Success(T data)
        {
            return new ResponseWrapper<T>() { IsSuccessful = true, Data = data };
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }

        public new static ResponseWrapper<T> Success(T data, string message)
        {
            return new ResponseWrapper<T>() { IsSuccessful = true, Data = data, Message = [message] };
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(T data, string message)
        {
            return Task.FromResult(Success(data,message));
        }

        public new static ResponseWrapper<T> Success(T data, List<string> message)
        {
            return new ResponseWrapper<T>() { IsSuccessful = true, Data = data, Message = message };
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(T data, List<string> message)
        {
            return Task.FromResult(Success(data, message));
        }
    }
}