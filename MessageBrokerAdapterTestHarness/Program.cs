using System;
using System.Text;
using System.Threading.Tasks;

namespace MessageBrokerAdapterTestHarness
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var (messageBrokerPublisher, messageBrokerSubscriber) = MessageBrokerFactory.Create(MessageBrokerType.ServiceBus);
            try
            {
                var body = Encoding.UTF8.GetBytes("This is the message body");
                await messageBrokerPublisher.Publish(new Message(body, Guid.NewGuid().ToString("N"), "application/json", "My MessageBroker", "corr_" + Guid.NewGuid().ToString("N")));
                messageBrokerSubscriber.Subscribe(mrea =>
                {
                    Console.WriteLine(mrea.Message.CreationDateTime);
                    Console.WriteLine(mrea.Message.ApplicationId);
                    Console.WriteLine(mrea.Message.ContentType);
                    Console.WriteLine(mrea.Message.CorrelationId);
                    Console.WriteLine(mrea.Message.MessageId);
                    Console.WriteLine(Encoding.UTF8.GetString(mrea.Message.Body));

                    messageBrokerSubscriber.Acknowledge(mrea.AcknowledgeToken);
                });

                Console.ReadLine();
            }
            finally
            {
                messageBrokerPublisher.Dispose();
                messageBrokerSubscriber.Dispose();
            }            
        }
    }
}
