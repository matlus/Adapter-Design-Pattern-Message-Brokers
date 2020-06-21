using System;
using System.Threading.Tasks;

namespace MessageBrokerAdapterTestHarness
{
    internal abstract class MessageBrokerSubscriberBase : IDisposable
    {
        public void Subscribe(Action<MessageReceivedEventArgs> receiveCallback)
        {
            SubscribeCore(receiveCallback);
        }

        public void Acknowledge(string acknowledgetoken)
        {
            AcknowledgeCore(acknowledgetoken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void SubscribeCore(Action<MessageReceivedEventArgs> receiveCallback);
        protected abstract void AcknowledgeCore(string acknowledgetoken);
        protected abstract void Dispose(bool disposing);
    }
}
