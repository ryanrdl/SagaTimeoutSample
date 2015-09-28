using System;
using NServiceBus;

namespace Messages
{
    public class ExtendTimeout1 : ICommand
    {
        public Guid RequestId { get; set; }
        public int ExtendBySeconds { get; set; }
    }
}
