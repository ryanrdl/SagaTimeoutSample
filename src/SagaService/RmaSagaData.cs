using System;
using NServiceBus.Saga;

namespace SagaService
{
    public class RmaSagaData : ContainSagaData
    {
        public Guid RequestId { get; set; }
        public int CustomerId { get; set; }
        

        public DateTime AcceptanceTimeout { get; set; }
        public DateTime RejectionTimeout { get; set; }
        
    }
}