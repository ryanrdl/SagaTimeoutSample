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
        IHandleMessages<ApproveRmaRequest>,
        IHandleMessages<RejectRmaRequest>,

        IHandleMessages<ExtendAllAcceptanceTimeouts>,
        IHandleMessages<ReduceAllRejectionTimeouts>

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


            Db.Save(message.RequestId, message.CustomerId);

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

            Db.Approve(message.RequestId);

            _bus.Publish(new RmaRequestApproved
            {
                RequestId = message.RequestId
            });
        }

        public void Handle(RejectRmaRequest message)
        {
            using (Colr.Red())
                Console.WriteLine("RMA request {0} rejected", message.RequestId);

            Db.Reject(message.RequestId);

            _bus.Publish(new RmaRequestRejected
            {
                RequestId = message.RequestId
            });
        }
         
        /// <summary>
        /// A saga needs a single message per saga and trying to make one message service multiple
        /// sagas ended up being hacky.
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ExtendAllAcceptanceTimeouts message)
        {
            foreach (var model in Db.GetAll())
            {
                _bus.SendLocal(new ExtendAcceptanceTimeout
                {
                    CustomerId = model.CustomerId,
                    RequestId = model.Id,
                    ExtendBySeconds = message.ExtendBySeconds
                });
            }
        }
        
        /// <summary>
        /// A saga needs a single message per saga and trying to make one message service multiple
        /// sagas ended up being hacky.
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ReduceAllRejectionTimeouts message)
        {
            foreach (var model in Db.GetAll())
            {
                _bus.SendLocal(new ReduceRejectionTimeout
                {
                    CustomerId = model.CustomerId,
                    RequestId = model.Id,
                    ReduceBySeconds = message.ReduceBySeconds
                });
            }
        }
    }
}
