namespace Business.FormsApp.Droid.Components.Log
{
    using System;

    using Business.FormsApp.Components.Log;

    public sealed class Logger : ILogger
    {
        private readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            logger.Debug(message, args);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        public void Warn(string message)
        {
            logger.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            logger.Warn(message, args);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
        }

        public void Error(Exception e, string message)
        {
            logger.Error(e, message);
        }

        public void Error(Exception e, string message, params object[] args)
        {
            logger.Error(e, message, args);
        }
    }
}