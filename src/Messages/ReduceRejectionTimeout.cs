using System;
using NServiceBus;

namespace Messages
{
    public class ReduceRejectionTimeout : ICommand
    {
        public Guid RequestId { get; set; }
        public int ReduceBySeconds { get; set; }
    }
}