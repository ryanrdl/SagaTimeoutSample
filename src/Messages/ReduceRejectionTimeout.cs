using System;
using NServiceBus;

namespace Messages
{
    public class ReduceRejectionTimeout : ICommand
    {
        public Guid CustomerId { get; set; }
        public Guid RequestId { get; set; }
        public int ReduceBySeconds { get; set; }
    }
}