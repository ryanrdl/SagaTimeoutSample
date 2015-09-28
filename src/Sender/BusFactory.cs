using NServiceBus;
using NServiceBus.Logging;

namespace Sender
{
    public static class BusFactory
    {
        public static IStartableBus GetBus()
        {
            var configuration = new BusConfiguration();
            configuration.UseTransport<RabbitMQTransport>();
            configuration.UsePersistence<InMemoryPersistence>(); 

            configuration.AutoSubscribe();
           
            DefaultFactory defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Fatal);
             
            var bus = Bus.Create(configuration); 
            return bus;
        }
    }
}