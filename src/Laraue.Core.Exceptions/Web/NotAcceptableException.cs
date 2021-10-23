using System;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 406 code
    /// </summary>
    public class NotAcceptableException : Exception
    {
        public NotAcceptableException()
        {
        }
        
        public NotAcceptableException(string message) : base(message)
        {
        }
    }
}