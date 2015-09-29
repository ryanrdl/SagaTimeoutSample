using NServiceBus;

namespace Messages
{
    public class ReduceAllAcceptanceTimeouts : ICommand
    {
        public int CustomerId { get; set; }
        
        public int ReduceBySeconds { get; set; }
    }
}