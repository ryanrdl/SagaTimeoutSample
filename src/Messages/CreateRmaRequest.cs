using System;
using NServiceBus;

namespace Messages
{
    public class CreateRmaRequest : ICommand
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; } 
        public int AcceptTimeoutSeconds { get; set; } 
        
    }
}
