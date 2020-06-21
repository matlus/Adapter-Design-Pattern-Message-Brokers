using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace MessageBrokerAdapterTestHarness
{
    internal sealed class MessageBrokerSubscriberRabbitMq : MessageBrokerSubscriberBase
    {
        private bool _disposed;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly string _queueName;
        private readonly IModel _channel;

        public MessageBrokerSubscriberRabbitMq(string connectionString, string topicExchange, string queueName)
        {
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(connectionString),
            };

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            //// the topicExchange parameter is not required by the RabbitMq Channel 
            _queueName = queueName;
        }

        protected override void SubscribeCore(Action<MessageReceivedEventArgs> receiveCallback)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var messageReceivedEventArgs = new MessageReceivedEventArgs(
                    new Message(
                        body: ea.Body.ToArray(),
                        messageId: ea.BasicProperties.MessageId,
                        contentType: ea.BasicProperties.ContentType,
                        applicationId: ea.BasicProperties.AppId,
                        correlationId: ea.BasicProperties.CorrelationId,
                        creationDateTime: DateTime.Parse(Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["CreationDateTime"]))),
                    ea.DeliveryTag.ToString(), new CancellationToken());

                receiveCallback(messageReceivedEventArgs);
            };

            _channel.BasicConsume(_queueName, autoAck: false, consumer);
        }

        protected override void AcknowledgeCore(string acknowledgetoken)
        {
            _channel.BasicAck(ulong.Parse(acknowledgetoken), multiple: false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
