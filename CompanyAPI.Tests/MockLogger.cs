using System;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Tests
{
    public abstract class MockLogger<T> : ILogger<T>
    {
        public abstract IDisposable BeginScope<TState>(TState state);

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(logLevel, eventId, state, exception);
        }

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception ex)
        {

        }
    }
}
