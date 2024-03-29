﻿using System;

namespace Signals.Aspects.Logging
{
    /// <summary>
    /// Logger contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILoggerBase<in T> where T : new()
    {
        /// <summary>
        /// Log an message consisting of args with log level trace
        /// </summary>
        /// <param name="logEntry"></param>
        void Trace(T logEntry);

        /// <summary>
        /// Log an message consisting of args with log level trace
        /// </summary>
        /// <param name="message"></param>
        void Trace(string message);

        /// <summary>
        /// Log an message consisting of args with log level debug
        /// </summary>
        /// <param name="logEntry"></param>
        void Debug(T logEntry);

        /// <summary>
        /// Log an message with log level debug
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// Log an message consisting of args with log level info
        /// </summary>
        /// <param name="logEntry"></param>
        void Info(T logEntry);

        /// <summary>
        /// Log an message consisting of args with log level info
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Log an message consisting of args with log level warn
        /// </summary>
        /// <param name="logEntry"></param>
        void Warn(T logEntry);

        /// <summary>
        /// Log an message consisting of args with log level warn
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);

        /// <summary>
        /// Log an message consisting of args with log level error
        /// </summary>
        /// <param name="logEntry"></param>
        void Error(T logEntry);

        /// <summary>
        /// Log an message consisting of args with log level error
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// Log an exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Exception(string message, Exception exception);

        /// <summary>
        /// Log an message consisting of args with log level fatal
        /// </summary>
        /// <param name="logEntry"></param>
        void Fatal(T logEntry);

        /// <summary>
        /// Log an message consisting of args with log level fatal
        /// </summary>
        /// <param name="message"></param>
        void Fatal(string message);

        /// <summary>
        /// Create string description of the log entry
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        string DescribeLogEntry(T logEntry);
    }
}
