using System;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 404 code
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }
        
        public NotFoundException(string message) : base(message)
        {
        }
    }
}