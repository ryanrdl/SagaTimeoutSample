using System;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

namespace SagaService
{
    public class RmaSaga : Saga<RmaSagaData>,
        IAmStartedByMessages<RmaRequestCreated>,
        IHandleMessages<ExtendAcceptanceTimeout>,
        IHandleMessages<ReduceRejectionTimeout>,
        IHandleMessages<RmaRequestApproved>,
        IHandleMessages<RmaRequestRejected>,

        IHandleTimeouts<AcceptanceTimeout>,
        IHandleTimeouts<RejectionTimeout>,

        IHandleSagaNotFound
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<RmaSagaData> mapper)
        {
            mapper.ConfigureMapping<RmaRequestCreated>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<ExtendAcceptanceTimeout>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<ReduceRejectionTimeout>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<RmaRequestApproved>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<RmaRequestRejected>(o => o.RequestId).ToSaga(o => o.RequestId);
        }

        public void Handle(RmaRequestCreated message)
        {
            Data.RequestId = message.RequestId;

            Data.AcceptanceTimeout = DateTime.Now.AddSeconds(message.Timeout1Seconds)
                //add extra time to avoid boundary condition
                .AddSeconds(1);

            RequestTimeout<AcceptanceTimeout>(Data.AcceptanceTimeout);

            Data.RejectionTimeout = DateTime.Now.AddSeconds(message.Timeout2Seconds)
                //add extra time to avoid boundary condition
                .AddSeconds(1);

            RequestTimeout<RejectionTimeout>(Data.RejectionTimeout);



            using (Colr.Green())
            {
                Console.WriteLine("Current Time = {0}", DateTime.Now.ToLongTimeString());

                Console.WriteLine("Acceptance timeout set to {0} for requestId {1}",
                    Data.AcceptanceTimeout.ToLongTimeString(), message.RequestId);

                Console.WriteLine("Rejection timeout set to {0} for requestId {1}",
                    Data.RejectionTimeout.ToLongTimeString(), message.RequestId);
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
                        Console.WriteLine("Approving rma request {0} because acceptance timer timed out at {1}",
                            Data.RequestId,
                            DateTime.Now.ToLongTimeString());

                    Bus.SendLocal(new ApproveRmaRequest {RequestId = Data.RequestId});
                }
                else
                {
                    using (Colr.Blue())
                        Console.WriteLine(
                            "Saga not terminated because it has not exceeded timeout 1, it will expire at {0} (CurrentTime = {1})",
                            Data.AcceptanceTimeout.ToLongTimeString(),
                            DateTime.Now.ToLongTimeString());
                }
            }
            else
            {
                using (Colr.Yellow())
                    Console.WriteLine("Ignoring acceptance timeout for request {0} because its state is {1}",
                        Data.RequestId,
                        Enum.GetName(typeof (RequestModel.RequestState), request.State));
            }
        }

        public void Handle(ExtendAcceptanceTimeout message)
        {
            Data.AcceptanceTimeout = Data.AcceptanceTimeout.AddSeconds(message.ExtendBySeconds);
            RequestTimeout<AcceptanceTimeout>(Data.AcceptanceTimeout);

            using (Colr.Green())
                Console.WriteLine("Request {0} acceptance timeout extended to {1} at {2}",
                    Data.RequestId,
                    Data.AcceptanceTimeout.ToLongTimeString(),
                    DateTime.Now.ToLongTimeString());
        }

        public void Timeout(RejectionTimeout state)
        {
            RequestModel request = Db.Get(Data.RequestId);
            if (request.State == RequestModel.RequestState.Pending)
            {
                if (Data.RejectionTimeout < DateTime.Now)
                {
                    using (Colr.Red())
                        Console.WriteLine("Rejecting rma request {0} because rejection timer timed out at {1}",
                            Data.RequestId,
                            DateTime.Now.ToLongTimeString());

                    Bus.SendLocal(new RejectRmaRequest {RequestId = Data.RequestId});
                }
                else
                {
                    using (Colr.Blue())
                        Console.WriteLine(
                            "Saga not terminated because it has not exceeded timeout 2, it will expire at {0} (CurrentTime = {1})",
                            Data.RejectionTimeout.ToLongTimeString(),
                            DateTime.Now.ToLongTimeString());
                }
            }
            else
            {
                using (Colr.Yellow())
                    Console.WriteLine("Ignoring rejection timeout for request {0} because its state is {1}",
                        Data.RequestId,
                        Enum.GetName(typeof (RequestModel.RequestState), request.State));
            }
        }

        public void Handle(ReduceRejectionTimeout message)
        {
            Data.RejectionTimeout = Data.RejectionTimeout.AddSeconds(message.ReduceBySeconds*-1);
            RequestTimeout<RejectionTimeout>(Data.RejectionTimeout);

            using (Colr.Green())
                Console.WriteLine("Request {0} rejection timeout reduced to {1} at {2}",
                    Data.RequestId,
                    Data.RejectionTimeout.ToLongTimeString(),
                    DateTime.Now.ToLongTimeString());
        }

        public void Handle(object message)
        {
            using (Colr.Red())
            {
                Console.WriteLine(
                    "Saga not found so it must have been expired, approved, or rejected.  Current Time = ({0}).",
                    DateTime.Now.ToLongTimeString());
                Console.WriteLine(message);
            }
        }

        public void Handle(RmaRequestApproved message)
        {
            using (Colr.Green())
                Console.WriteLine("Completing saga because RMA request {0} was approved at {1}",
                    Data.RequestId,
                    DateTime.Now.ToLongTimeString());
            MarkAsComplete();
        }

        public void Handle(RmaRequestRejected message)
        {
            using (Colr.Red())
                Console.WriteLine("Completing saga because RMA request {0} was rejected at {1}",
                    Data.RequestId,
                    DateTime.Now.ToLongTimeString());
            MarkAsComplete();
        }
    }

    public class AcceptanceTimeout
    {

    }

    public class RejectionTimeout
    {

    }
}