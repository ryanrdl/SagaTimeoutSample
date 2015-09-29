using System;
using NServiceBus;

namespace Messages
{
    public class ExtendAllAcceptanceTimeouts : ICommand
    {
        public Guid CustomerId { get; set; }
        public int ExtendBySeconds { get; set; }
    } 
}