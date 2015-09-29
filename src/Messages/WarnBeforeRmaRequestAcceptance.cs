using System;
using NServiceBus;

namespace Messages
{
    public class WarnBeforeRmaRequestAcceptance : ICommand
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; }        
    }
}
