using System;
using NServiceBus;

namespace Messages
{
    public class CreateRmaRequest : ICommand
    {
        public int CustomerId { get; set; }
        public Guid RequestId { get; set; } 
        public int Timeout1Seconds { get; set; }
        public int Timeout2Seconds { get; set; }
        
    }
}
