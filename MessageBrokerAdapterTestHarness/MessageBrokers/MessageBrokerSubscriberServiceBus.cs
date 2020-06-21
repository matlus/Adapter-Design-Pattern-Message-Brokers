using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace MessageBrokerAdapterTestHarness
{
    internal sealed class MessageBrokerSubscriberServiceBus : MessageBrokerSubscriberBase
    {
        private bool _disposed;
        private readonly SubscriptionClient _subscriptionClient;

        public MessageBrokerSubscriberServiceBus(string connectionString, string topicExchange, string queueName)
        {
            _subscriptionClient = new SubscriptionClient(connectionString, topicExchange, queueName);
        }

        protected override void SubscribeCore(Action<MessageReceivedEventArgs> receiveCallback)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler);
            messageHandlerOptions.AutoComplete = false;

            _subscriptionClient.RegisterMessageHandler((sbMessage, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var messageReceivedEventArgs = new MessageReceivedEventArgs(
                    new Message(sbMessage.Body,
                                sbMessage.MessageId,
                                sbMessage.ContentType,
                                (string)sbMessage.UserProperties["ApplicationId"],
                                sbMessage.CorrelationId,
                                DateTime.Parse((string)sbMessage.UserProperties["CreationDateTime"])),
                    sbMessage.SystemProperties.LockToken,
                    cancellationToken);

                receiveCallback(messageReceivedEventArgs);
                return Task.CompletedTask;
            }, messageHandlerOptions);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            throw new MessageBrokerMessageReceiveException(exceptionReceivedEventArgs.Exception.Message, exceptionReceivedEventArgs.Exception);
        }

        protected override void AcknowledgeCore(string acknowledgetoken)
        {
            _subscriptionClient.CompleteAsync(acknowledgetoken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed && _subscriptionClient != null)
            {
                _subscriptionClient.CloseAsync().ContinueWith(continuationAction =>
                {
                    continuationAction.Wait();
                });

                _disposed = true;
            }
        }
    }
}
