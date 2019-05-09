namespace Techies.Clients.DTOs.Responses
{
    public class OperationResult
    {
        public OperationResult(bool isCorrect, string message=null)
        {
            IsCorrect = isCorrect;
            Message = null;
        }

        public bool IsCorrect { get; set; }
        public string Message { get; set; }

        public static OperationResult Correct()
        {
            return new OperationResult(true);
        }

        public static OperationResult WithError(string error)
        {
            return new OperationResult(false,error);
        }
        public static OperationResult<T> WithError<T>(string message)
        {
            return new OperationResult<T>(false, default(T), message);
        }
        public static OperationResult<T> Correct<T>(T data)
        {
            return new OperationResult<T>(true,data);
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public OperationResult(bool isCorrect,T data, string message = null):base(isCorrect,message)
        {
            Data = data;
        }

        public T Data { get;set;}        
    }
}
