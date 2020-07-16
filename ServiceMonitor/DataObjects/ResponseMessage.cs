using System;
using Nancy;

namespace ServiceMonitor.DataObjects
{
    /// <summary>
    /// Represents a response message
    /// </summary>
    internal sealed class ResponseMessage
    {
        /// <summary>
        /// Gets the status code
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Gets the message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage"/>
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="code">The http status code (optional)</param>
        public ResponseMessage(string message, HttpStatusCode code = HttpStatusCode.OK)
        {
            Code = (int) code;
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ResponseMessage"/>
        /// <para/>
        /// The code will be set to 500 (Internal Server Error)
        /// </summary>
        /// <param name="ex">The exception</param>
        public ResponseMessage(Exception ex)
        {
            Code = 500;
            Message = $"An error has occured: {ex.Message}";
        }
    }
}
