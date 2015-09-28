using System;
using NServiceBus;

namespace Messages
{
    public class CreateRmaRequest : ICommand
    {
        public Guid RequestId { get; set; } 
        public int Timeout1Seconds { get; set; }
        public int Timeout2Seconds { get; set; }
        
    }
}
