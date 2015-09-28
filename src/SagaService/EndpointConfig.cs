
using NServiceBus.Logging;

namespace SagaService
{
    using NServiceBus; 
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            configuration.UseTransport<RabbitMQTransport>();
            configuration.UsePersistence<InMemoryPersistence>(); 

            configuration.AutoSubscribe();
           
            DefaultFactory defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Fatal);
        }
    }
}
