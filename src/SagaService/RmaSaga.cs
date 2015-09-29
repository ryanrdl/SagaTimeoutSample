using System;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

namespace SagaService
{
    public class RmaSaga : Saga<RmaSagaData>,
        IAmStartedByMessages<RmaRequestCreated>,
        IHandleMessages<ExtendAcceptanceTimeout>,
        IHandleMessages<ReduceAcceptanceTimeout>,
        IHandleMessages<RmaRequestApproved>, 

        IHandleTimeouts<AcceptanceTimeout>,
        IHandleTimeouts<WarningBeforeAcceptanceTimeout>,

        IHandleSagaNotFound
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<RmaSagaData> mapper)
        {
            mapper.ConfigureMapping<RmaRequestCreated>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<ExtendAcceptanceTimeout>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<ReduceAcceptanceTimeout>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<RmaRequestApproved>(o => o.RequestId).ToSaga(o => o.RequestId); 
        }

        private void SetAcceptanceTimeouts(DateTime acceptAt)
        {
            Data.AcceptanceTimeout = acceptAt;
            RequestTimeout<AcceptanceTimeout>(Data.AcceptanceTimeout);

            //warn 15 sec before auto acceptance
            Data.WarningBeforeAcceptanceTimeout = acceptAt.AddSeconds(-15); 

            RequestTimeout<WarningBeforeAcceptanceTimeout>(Data.WarningBeforeAcceptanceTimeout);
        }

        public void Handle(RmaRequestCreated message)
        {
            Data.RequestId = message.RequestId;
            Data.CustomerId = message.CustomerId;

            SetAcceptanceTimeouts(DateTime.Now.AddSeconds(message.AcceptanceTimeout));

            using (Colr.Green())
            {
                //Console.WriteLine("Current Time = {0}", DateTime.Now.ToLongTimeString());

                Console.WriteLine("Acceptance timeout set to {0} for requestId {1} for customer {2}",
                    Data.AcceptanceTimeout.ToLongTimeString(), 
                    message.RequestId,
                    message.CustomerId);

                Console.WriteLine("WarningBeforeAcceptance timeout set to {0} for requestId {1} for customer {2}",
                    Data.WarningBeforeAcceptanceTimeout.ToLongTimeString(),
                    message.RequestId,
                    message.CustomerId);
            }
        }


        public void Timeout(AcceptanceTimeout state)
        {
            RequestModel request = Db.Get(Data.RequestId);
            if (request.State == RequestModel.RequestState.Pending)
            {
                if (Data.AcceptanceTimeout < DateTime.Now)
                {
                    using (Colr.Green())
                        Console.WriteLine("Approving rma request {0} because acceptance timer timed out at {1} for customer {2}",
                            Data.RequestId,
                            DateTime.Now.ToLongTimeString(),
                            Data.CustomerId);

                    Bus.SendLocal(new ApproveRmaRequest {RequestId = Data.RequestId, CustomerId = Data.CustomerId});
                }
                else
                {
                    //using (Colr.Blue())
                    //    Console.WriteLine(
                    //        "Saga not terminated because it has not exceeded timeout 1, it will expire at {0} (CurrentTime = {1})",
                    //        Data.AcceptanceTimeout.ToLongTimeString(),
                    //        DateTime.Now.ToLongTimeString());
                }
            }
            else
            {
                //using (Colr.Yellow())
                //    Console.WriteLine("Ignoring acceptance timeout for request {0} because its state is {1}",
                //        Data.RequestId,
                //        Enum.GetName(typeof (RequestModel.RequestState), request.State));
            }
        }

        public void Handle(ExtendAcceptanceTimeout message)
        {
            Data.AcceptanceTimeout = Data.AcceptanceTimeout.AddSeconds(message.ExtendBySeconds);
            RequestTimeout<AcceptanceTimeout>(Data.AcceptanceTimeout);

            using (Colr.Green())
                Console.WriteLine("Request {0} acceptance timeout extended to {1} at {2} for customer {3}",
                    Data.RequestId,
                    Data.AcceptanceTimeout.ToLongTimeString(),
                    DateTime.Now.ToLongTimeString(),
                    Data.CustomerId);
        }

        public void Timeout(WarningBeforeAcceptanceTimeout state)
        {
            RequestModel request = Db.Get(Data.RequestId);
            if (request.State == RequestModel.RequestState.Pending)
            {
                if (Data.WarningBeforeAcceptanceTimeout < DateTime.Now)
                {
                    using (Colr.Red())
                        Console.WriteLine("Rma request {0} will auto accept at {1} for customer {2}",
                            Data.RequestId,
                            Data.AcceptanceTimeout,
                            Data.CustomerId);
                    
                    Bus.Publish(new RmaRequestAboutToAutoAccept
                    {
                        RequestId = Data.RequestId,
                        CustomerId = Data.CustomerId,
                        AutoAcceptAt = Data.AcceptanceTimeout
                    });
                } 
            } 
        }

        public void Handle(ReduceAcceptanceTimeout message)
        {
            Data.WarningBeforeAcceptanceTimeout = Data.WarningBeforeAcceptanceTimeout.AddSeconds(message.ReduceBySeconds*-1);
            RequestTimeout<WarningBeforeAcceptanceTimeout>(Data.WarningBeforeAcceptanceTimeout);

            using (Colr.Green())
                Console.WriteLine("Request {0} rejection timeout reduced to {1} at {2} for customer {3}",
                    Data.RequestId,
                    Data.WarningBeforeAcceptanceTimeout.ToLongTimeString(),
                    DateTime.Now.ToLongTimeString(), 
                    Data.CustomerId);
        }

        public void Handle(object message)
        {
            //using (Colr.Red())
            //{
            //    Console.WriteLine(
            //        "Saga not found so it must have been expired, approved, or rejected.  Current Time = ({0}).",
            //        DateTime.Now.ToLongTimeString());
            //    Console.WriteLine(message);
            //}
        }

        public void Handle(RmaRequestApproved message)
        {
            using (Colr.Green())
                Console.WriteLine("Completing saga because RMA request {0} was approved at {1} for customer {2}",
                    Data.RequestId,
                    DateTime.Now.ToLongTimeString(), 
                    message.CustomerId);
            MarkAsComplete();
        } 
    }

    public class AcceptanceTimeout
    {

    }

    public class WarningBeforeAcceptanceTimeout
    {

    }
}