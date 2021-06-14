using System;

namespace BowlingScoreCalculator.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public CustomException(string _message, int _code) : base(_message)
        {
            StatusCode = _code;
            StatusMessage = _message;
        }
    }
}
