using System;
using NServiceBus;

namespace Messages
{
    public class ReduceAcceptanceTimeout : ICommand
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; }
        public int ReduceBySeconds { get; set; }
    }
}