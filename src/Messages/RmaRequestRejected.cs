using System;
using NServiceBus;

namespace Messages
{
    public class RmaRequestRejected : IEvent
    {
        public Guid RequestId { get; set; }
         
    }
}