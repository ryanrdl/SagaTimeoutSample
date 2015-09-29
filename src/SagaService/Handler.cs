using System;
using System.Linq;
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

        IHandleMessages<ExtendAllAcceptanceTimeouts>,
        IHandleMessages<ReduceAllAcceptanceTimeouts>

    {
        private readonly IBus _bus;

        public Handler(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(CreateRmaRequest message)
        {
            //using (Colr.White())
            //    Console.WriteLine("RMA request {0} created for customer {1}", message.RequestId, message.CustomerId);
            
            Db.Save(message.RequestId, message.CustomerId);

            _bus.Publish(new RmaRequestCreated
            {
                RequestId = message.RequestId,
                AcceptanceTimeout = message.AcceptTimeoutSeconds
            });    
        }

        public void Handle(ApproveRmaRequest message)
        {
            //using (Colr.Green())
            //    Console.WriteLine("RMA request {0} approved for customer {1}", message.RequestId, message.CustomerId);

            Db.Approve(message.RequestId);

            _bus.Publish(new RmaRequestApproved
            {
                RequestId = message.RequestId,
                CustomerId = message.CustomerId
            });
        } 
         
        /// <summary>
        /// A saga needs a single message per saga and trying to make one message service multiple
        /// sagas ended up being hacky.
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ExtendAllAcceptanceTimeouts message)
        {
            using (Colr.Yellow())
                Console.WriteLine("Extending all active RMA requests acceptance timeout for customer {0}", message.CustomerId);

            foreach (var model in Db.GetAll(message.CustomerId).Where(o => o.State == RequestModel.RequestState.Pending))
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
        public void Handle(ReduceAllAcceptanceTimeouts message)
        {
            using (Colr.Yellow())
                Console.WriteLine("Reducing all active RMA requests rejection timeout for customer {0}", message.CustomerId);

            foreach (var model in Db.GetAll(message.CustomerId).Where(o => o.State == RequestModel.RequestState.Pending))
            {
                _bus.SendLocal(new ReduceAcceptanceTimeout
                {
                    CustomerId = model.CustomerId,
                    RequestId = model.Id,
                    ReduceBySeconds = message.ReduceBySeconds
                });
            }
        }
    }
}
