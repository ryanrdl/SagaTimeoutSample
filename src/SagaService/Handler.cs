using System;
using Messages;
using NServiceBus;

namespace SagaService
{
    /// <summary>
    /// The reason this is not in the saga is because the only purpose of the saga is to handle state machine
    /// state transitions.  Do not put any business logic (like CRUD, domain logic, etc...) in the state machine.
    /// That should live outside the state machine and create events that the saga can listen for and transition
    /// state based off.  
    /// </summary>
    public class Handler : 
        IHandleMessages<CreateRmaRequest>,
        IHandleMessages<ApproveRmaRequest>
    {
        private readonly IBus _bus;

        public Handler(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(CreateRmaRequest message)
        {
            using (Colr.Blue())
                Console.WriteLine("RMA request {0} approved", message.RequestId);

            _bus.Publish(new RmaRequestCreated
            {
                RequestId = message.RequestId,
                Timeout1Seconds = message.Timeout1Seconds,
                Timeout2Seconds = message.Timeout2Seconds
            });    
        }

        public void Handle(ApproveRmaRequest message)
        {
            using (Colr.Green())
                Console.WriteLine("RMA request {0} approved", message.RequestId);

            _bus.Publish(new RmaRequestApproved
            {
                RequestId = message.RequestId
            });
        }
    }
}
