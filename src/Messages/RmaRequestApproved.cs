using System;
using NServiceBus;

namespace Messages
{
    public class RmaRequestApproved : IEvent
    {
        public Guid RequestId { get; set; }
    }
}
