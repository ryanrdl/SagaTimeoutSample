using System;
using NServiceBus;

namespace Messages
{
    public class ApproveRmaRequest : ICommand
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; }
        
    }
}