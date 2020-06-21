
namespace MessageBrokerAdapterTestHarness
{
    internal static class MessageBrokerFactory
    {
        const string brokerConnectionStringRabbitMq = "amqp://transcode_user:password@localhost/video.transcode.vhost";
        const string brokerConnectionStringServiceBus = "Endpoint=sb://movieservice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=M4inzMo8wfHZitQGNQJ4ePT7j00jitJauES3SAWRK6U=";
        const string topicExchange = "videoreceived.exchange";
        const string queueName = "videoreceived.queue";

        public static (
            MessageBrokerPublisherBase messageBrokerPublisher,
            MessageBrokerSubscriberBase messageBrokerSubscriber) Create(MessageBrokerType messageBrokerType)
        {
            switch (messageBrokerType)
            {
                case MessageBrokerType.RabbitMq:
                    return (
                        new MessageBrokerPublisherRabbitMq(brokerConnectionStringRabbitMq, topicExchange),
                        new MessageBrokerSubscriberRabbitMq(brokerConnectionStringRabbitMq, topicExchange, queueName));
                case MessageBrokerType.ServiceBus:
                    return (
                        new MessageBrokerPublisherServiceBus(brokerConnectionStringServiceBus, topicExchange),
                        new MessageBrokerSubscriberServiceBus(brokerConnectionStringServiceBus, topicExchange, queueName));
            }

            throw new MessageBrokerTypeNotSupportedException($"The MessageBrokerType: {messageBrokerType}, is not supported yet");
        }
    }

    internal enum MessageBrokerType {  RabbitMq, ServiceBus }
}
