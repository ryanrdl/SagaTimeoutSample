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

            Data.Timeout1Expires = DateTime.Now.AddSeconds(message.Timeout1Seconds)
                //add extra time to avoid boundary condition
                .AddSeconds(1);

            RequestTimeout<AcceptanceTimeout>(Data.Timeout1Expires);

            Data.Timeout2Expires = DateTime.Now.AddSeconds(message.Timeout2Seconds)
                //add extra time to avoid boundary condition
                .AddSeconds(1);

            RequestTimeout<RejectionTimeout>(Data.Timeout2Expires);



            using (Colr.Green())
            {
                Console.WriteLine("Current Time = {0}", DateTime.Now.ToLongTimeString());

                Console.WriteLine("Saga timeout 1 set to " + Data.Timeout1Expires.ToLongTimeString());

                Console.WriteLine("Saga timeout 2 set to " + Data.Timeout2Expires.ToLongTimeString());
            }
        }


        public void Timeout(AcceptanceTimeout state)
        {
            if (Data.Timeout1Expires < DateTime.Now)
            {
                using (Colr.Red())
                    Console.WriteLine("Approving rma request because timeout 1 timed out at {0}",
                        DateTime.Now.ToLongTimeString());

                Bus.Send(new ApproveRmaRequest {RequestId = Data.RequestId});
            }
            else
            {
                using (Colr.Blue())
                    Console.WriteLine(
                        "Saga not terminated because it has not exceeded timeout 1, it will expire at {0} (CurrentTime = {1})",
                        Data.Timeout1Expires.ToLongTimeString(),
                        DateTime.Now.ToLongTimeString());
            }
        }

        public void Handle(ExtendAcceptanceTimeout message)
        {
            Data.Timeout1Expires = Data.Timeout1Expires.AddSeconds(message.ExtendBySeconds);
            RequestTimeout<AcceptanceTimeout>(Data.Timeout1Expires);

            using (Colr.Green())
                Console.WriteLine("Saga timeout 1 extended to {0} at {1}",
                    Data.Timeout1Expires.ToLongTimeString(),
                    DateTime.Now.ToLongTimeString());
        }

        public void Timeout(RejectionTimeout state)
        {
            if (Data.Timeout2Expires < DateTime.Now)
            {
                using (Colr.Red())
                    Console.WriteLine("Rejecting rma request because timeout 2 timed out at {0}",
                        DateTime.Now.ToLongTimeString());

                Bus.Send(new RejectRmaRequest {RequestId = Data.RequestId});
            }
            else
            {
                using (Colr.Blue())
                    Console.WriteLine(
                        "Saga not terminated because it has not exceeded timeout 2, it will expire at {0} (CurrentTime = {1})",
                        Data.Timeout2Expires.ToLongTimeString(),
                        DateTime.Now.ToLongTimeString());
            }
        }

        public void Handle(ReduceRejectionTimeout message)
        {
            Data.Timeout2Expires = Data.Timeout2Expires.AddSeconds(message.ReduceBySeconds*-1);
            RequestTimeout<RejectionTimeout>(Data.Timeout2Expires);

            using (Colr.Green())
                Console.WriteLine("Saga timeout 2 reduced to {0} at {1}", Data.Timeout2Expires.ToLongTimeString(),
                    DateTime.Now.ToLongTimeString());
        }

        public void Handle(object message)
        {
            using (Colr.Red())
            {
                Console.WriteLine("Saga not found so it must have been expired, approved, or rejected.  Current Time = ({0}).",
                    DateTime.Now.ToLongTimeString());
                Console.WriteLine(message);
            }
        }

        public void Handle(RmaRequestApproved message)
        {
            using (Colr.Green())
                Console.WriteLine("Completing saga because RMA request {0} was approved at {1}", message.RequestId,
                    DateTime.Now.ToLongTimeString());
            MarkAsComplete();
        }

        public void Handle(RmaRequestRejected message)
        {
            using (Colr.Red())
                Console.WriteLine("Completing saga because RMA request {0} was rejected at {1}", message.RequestId,
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