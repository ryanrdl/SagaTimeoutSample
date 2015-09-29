
using System;
using System.IO;
using Messages;
using NServiceBus.Logging;

namespace SagaService
{
    using NServiceBus; 
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            DefaultFactory defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Directory(Logs.Get(Environment.CurrentDirectory));
            defaultFactory.Level(LogLevel.Error); 

            configuration.UseTransport<RabbitMQTransport>();
            configuration.UsePersistence<InMemoryPersistence>(); 

            configuration.AutoSubscribe();
        }
    }
}
