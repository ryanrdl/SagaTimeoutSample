using System;
using NServiceBus;

namespace Messages
{
    public class ApproveRmaRequest : ICommand
    {
        public Guid RequestId { get; set; }
        
    }
}