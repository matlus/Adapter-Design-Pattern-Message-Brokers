
using System;

namespace MessageBrokerAdapterTestHarness
{
    internal sealed class Message
    {
        public byte[] Body { get; }
        public string MessageId { get; }
        public string ContentType { get; }
        public string ApplicationId { get; }
        public string CorrelationId { get; }
        public DateTime CreationDateTime { get; }
        public Message(byte[] body)
            : this(body, null, null, null, null)
        {
        }

        public Message(byte[] body, string messageId, string contentType, string applicationId, string correlationId)
            :this(body, messageId, contentType, applicationId, correlationId, DateTime.UtcNow)
        {
        }

        public Message(byte[] body, string messageId, string contentType, string applicationId, string correlationId, DateTime creationDateTime)
        {
            Body = body;
            MessageId = messageId;
            ContentType = contentType;
            ApplicationId = applicationId;
            CorrelationId = correlationId;
            CreationDateTime = creationDateTime;
        }
    }
}
