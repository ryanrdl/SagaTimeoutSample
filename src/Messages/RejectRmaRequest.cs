using System;
using NServiceBus;

namespace Messages
{
    public class RejectRmaRequest : ICommand
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; }        
    }
}
