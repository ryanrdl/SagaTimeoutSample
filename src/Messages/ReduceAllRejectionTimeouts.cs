using System;
using NServiceBus;

namespace Messages
{
    public class ReduceAllRejectionTimeouts : ICommand
    {
        public int CustomerId { get; set; }
        
        public int ReduceBySeconds { get; set; }
    }
}