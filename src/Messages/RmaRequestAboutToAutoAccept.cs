using System;
using NServiceBus;

namespace Messages
{
    public class RmaRequestAboutToAutoAccept : IEvent
    {
        public Guid RequestId { get; set; }
        public int CustomerId { get; set; }
        public DateTime AutoAcceptAt { get; set; }
        
    }
}
