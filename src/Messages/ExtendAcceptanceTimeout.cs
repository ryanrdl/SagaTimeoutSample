using System;
using NServiceBus;

namespace Messages
{
    public class ExtendAcceptanceTimeout : ICommand
    {
        public Guid RequestId { get; set; }
        public int ExtendBySeconds { get; set; }
    }
}
