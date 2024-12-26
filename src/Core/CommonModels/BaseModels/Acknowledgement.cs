using System.Net;

namespace Core.CommonModels.BaseModels
{
    public class Acknowledgement
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

        public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 300;

        public List<string> ErrorMessageList { get; set; }

        public List<string> SuccessMessageList { get; set; }

        public Acknowledgement()
        {
            ErrorMessageList = new List<string>();
            SuccessMessageList = new List<string>();
        }
        public Acknowledgement(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            ErrorMessageList = new List<string>();
            SuccessMessageList = new List<string>();
        }

        public void AddMessage(string message)
        {
            ErrorMessageList.Add(message);
        }

        public void AddMessages(params string[] messages)
        {
            ErrorMessageList.AddRange(messages);
        }

        public void AddMessages(IEnumerable<string> messages)
        {
            ErrorMessageList.AddRange(messages);
        }

        public void AddSuccessMessages(params string[] messages)
        {
            SuccessMessageList.AddRange(messages);
        }

        public Exception ToException()
        {
            if (!IsSuccess)
            {
                return new Exception(string.Join(Environment.NewLine, ErrorMessageList));
            }

            return null;
        }

        public void ExtractMessage(Exception ex)
        {
            AddMessage(ex.Message);
            if (ex.InnerException != null)
            {
                ExtractMessage(ex.InnerException);
            }
        }
    }
    public class Acknowledgement<T> : Acknowledgement
    {
        public T Data { get; set; }
    }
}
