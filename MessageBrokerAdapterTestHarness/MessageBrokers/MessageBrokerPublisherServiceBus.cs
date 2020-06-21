using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
using SbMessage = Microsoft.Azure.ServiceBus.Message;

namespace MessageBrokerAdapterTestHarness
{
    internal sealed class MessageBrokerPublisherServiceBus : MessageBrokerPublisherBase
    {
        private bool _disposed;
        private readonly TopicClient _topicClient;

        public MessageBrokerPublisherServiceBus(string connectionString, string topicExchange)
        {
            _topicClient = new TopicClient(connectionString, topicExchange);
        }

        protected override async Task PublishCore(Message message)
        {
            var sbMessage = new SbMessage(message.Body);
            sbMessage.UserProperties.Add("ApplicationId", message.ApplicationId);
            sbMessage.CorrelationId = message.CorrelationId;
            sbMessage.ContentType = message.ContentType;
            sbMessage.MessageId = message.MessageId;
            sbMessage.UserProperties.Add("CreationDateTime", message.CreationDateTime.ToString("o"));
            await _topicClient.SendAsync(sbMessage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_topicClient != null)
                {
                    _topicClient.CloseAsync().ContinueWith(continuationTask =>
                    {
                        continuationTask.Wait();
                    });
                }

                _disposed = true;
            }
        }
    }
}
