using System;
using System.Collections.Generic;
using System.Text;
using Uniframework.Services;

namespace Uniframework.Tests
{
    public class MockLoggerFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        public ILogger CreateLogger<T>(string loggingPath)
        {
            return new MockLogger();
        }

        public ILogger CreateLogger<T>()
        { return new MockLogger(); }

        public ILogger CreateLogger(Type type, string loggingPath) { return new MockLogger(); }

        public ILogger CreateLogger(Type type) { return new MockLogger(); }

        #endregion
    }

    public class MockLogger : ILogger
    {
        #region ILogger Members

        public void Debug(string message)
        {

        }

        public void Debug(string message, Exception ex)
        {

        }

        public void Info(string message)
        {

        }

        public void Info(string message, Exception ex)
        {

        }

        public void Warn(string message)
        {

        }

        public void Warn(string message, Exception ex)
        {

        }

        public void Error(string message)
        {

        }

        public void Error(string message, Exception ex)
        {

        }

        public void Fatal(string message)
        {

        }

        public void Fatal(string message, Exception ex)
        {

        }

        #endregion
    }
}
