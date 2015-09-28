using System;
using NServiceBus;

namespace Messages
{
    public class RejectRmaRequest : ICommand
    {
        public Guid RequestId { get; set; }        
    }
}
