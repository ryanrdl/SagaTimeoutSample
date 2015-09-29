using System;
using NServiceBus;

namespace Messages
{
    public class ExtendAllAcceptanceTimeouts : ICommand
    {
        public int CustomerId { get; set; }
        public int ExtendBySeconds { get; set; }
    } 
}