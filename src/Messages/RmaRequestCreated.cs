using System;
using NServiceBus;

namespace Messages
{
    public class RmaRequestCreated : IEvent
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; } 
        public int AcceptanceTimeout { get; set; }
        public int RejectionTimeout { get; set; }  
    }
}