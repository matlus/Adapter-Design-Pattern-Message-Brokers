using System;

namespace MessageBrokerAdapterTestHarness
{

    [Serializable]
    public sealed class MessageBrokerMessageReceiveException : Exception
    {
        public MessageBrokerMessageReceiveException() { }
        public MessageBrokerMessageReceiveException(string message) : base(message) { }
        public MessageBrokerMessageReceiveException(string message, Exception inner) : base(message, inner) { }
        private MessageBrokerMessageReceiveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
