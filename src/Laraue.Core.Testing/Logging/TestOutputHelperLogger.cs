using Microsoft.Extensions.Logging;
using System;
using System.Text;
using Xunit.Abstractions;

namespace Laraue.Core.Testing.Logging
{
    public class TestOutputHelperLogger<T> : ILogger<T> where T : class
    {
        public ITestOutputHelper _helper;

        public TestOutputHelperLogger(ITestOutputHelper helper)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var messageBulder = new StringBuilder($"{DateTime.Now} {logLevel} {typeof(T).FullName}: - {state}");
            if (exception != null)
                messageBulder.AppendLine($"{exception.Message} {exception.StackTrace}");
            _helper.WriteLine(messageBulder.ToString());
        }
    }
}