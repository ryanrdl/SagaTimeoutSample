using System;
using NServiceBus.Saga;

namespace SagaService
{
    public class RmaSagaData : ContainSagaData
    {
        public Guid RequestId { get; set; }

        public DateTime Timeout1Expires { get; set; }
        public DateTime Timeout2Expires { get; set; }
        
    }
}