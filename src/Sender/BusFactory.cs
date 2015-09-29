using System;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Sender
{
    public static class BusFactory
    {
        public static IStartableBus GetBus()
        {
            DefaultFactory defaultFactory = LogManager.Use<DefaultFactory>(); 
            defaultFactory.Level(LogLevel.Fatal);

            var configuration = new BusConfiguration();
            configuration.UseTransport<RabbitMQTransport>();
            configuration.UsePersistence<InMemoryPersistence>(); 

            configuration.AutoSubscribe(); 
             
            var bus = Bus.Create(configuration); 
            return bus;
        }
    }
}