using System;
using NServiceBus;

namespace Messages
{
    public class ApproveRmaRequest : ICommand
    {
        public Guid CustomerId { get; set; }
        public Guid RequestId { get; set; }
        
    }
}