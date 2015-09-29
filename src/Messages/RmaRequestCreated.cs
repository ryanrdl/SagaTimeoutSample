using System;
using NServiceBus;

namespace Messages
{
    public class RmaRequestCreated : IEvent
    {
        public Guid CustomerId { get; set; }
        public Guid RequestId { get; set; } 
        public int Timeout1Seconds { get; set; }
        public int Timeout2Seconds { get; set; }  
    }
}