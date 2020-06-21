using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageBrokerAdapterTestHarness
{
    internal sealed class MessageBrokerPublisherRabbitMq : MessageBrokerPublisherBase
    {
        private bool _disposed;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly string _topicExchange;

        public MessageBrokerPublisherRabbitMq(string connectionString, string topicExchange)
        {
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(connectionString),
            };

            _connection = _connectionFactory.CreateConnection();
            _topicExchange = topicExchange;
        }

        protected override Task PublishCore(Message message)
        {
            using (var channel = _connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = message.ContentType;
                properties.MessageId = message.MessageId;
                properties.AppId = message.ApplicationId;
                properties.CorrelationId = message.CorrelationId;

                var propertiesDictionary = new Dictionary<string, object>();
                properties.Headers = propertiesDictionary;
                propertiesDictionary.Add("CreationDateTime", message.CreationDateTime.ToString("o"));
                channel.BasicPublish(_topicExchange, routingKey: string.Empty, properties, message.Body);
            }

            return Task.CompletedTask;
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
