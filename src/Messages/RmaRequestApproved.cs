using System;
using NServiceBus;

namespace Messages
{
    public class RmaRequestApproved : IEvent
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; }
    }
}
