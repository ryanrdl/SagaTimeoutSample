using NServiceBus;
using NServiceBus.Logging;

namespace SagaService
{
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            DefaultFactory defaultFactory = LogManager.Use<DefaultFactory>(); 
            defaultFactory.Level(LogLevel.Fatal); 

            configuration.UseTransport<RabbitMQTransport>();
            configuration.UsePersistence<InMemoryPersistence>(); 

            configuration.AutoSubscribe();
        }
    }
}
