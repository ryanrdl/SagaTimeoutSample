using System;
using NServiceBus;

namespace Messages
{
    public class ReduceTimeout2 : ICommand
    {
        public Guid RequestId { get; set; }
        public int ReduceBySeconds { get; set; }
    }
}